using CaltexChallenge.Data;
using CaltexChallenge.Data.Repositories;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaltexChallenge.Services
{
    public class PurchaseService : IPurchaseService
    {
        private IProductRepository ProductRepository { get; }
        private IPointsPromotionRepository PointsPromotionRepository { get; }
        private IDiscountPromotionRepository DiscountPromotionRepository { get; }
        private IDiscountPromotionProductRepository DiscountPromotionProductRepository { get; }

        public PurchaseService(IProductRepository productRepository, 
            IPointsPromotionRepository pointsPromotionRepository,
            IDiscountPromotionRepository discountPromotionRepository,
            IDiscountPromotionProductRepository discountPromotionProductRepository)
        {
            ProductRepository = productRepository;
            PointsPromotionRepository = pointsPromotionRepository;
            DiscountPromotionRepository = discountPromotionRepository;
            DiscountPromotionProductRepository = discountPromotionProductRepository;

            // Again, in the real-world, structured logging should be used throughout the code base, e.g. inject ILogger<PurchaseService> here.
        }

        public async Task<Purchase> PurchaseAsync(string customerId, string loyaltyCard, DateTime transactionDate, IEnumerable<BasketItem> basketItems)
        {
            var calculateTotalAmountTask = CalculateTotalAmount(basketItems);
            var calculatePointsEarnedTask = CalculatePointsEarned(transactionDate, basketItems);
            var calculateDiscountAppliedTask = CalculateDiscountApplied(transactionDate, basketItems);

            // If the underlying mock repositories were asynchronous, all calculation tasks could run in parallel
            await Task.WhenAll(calculateTotalAmountTask, calculatePointsEarnedTask, calculateDiscountAppliedTask);

            var purchase = new Purchase
            {
                CustomerId = customerId,
                LoyaltyCard = loyaltyCard,
                TransactionDate = transactionDate,
                TotalAmount = calculateTotalAmountTask.Result,
                DiscountApplied = calculateDiscountAppliedTask.Result,
                GrandTotal = calculateTotalAmountTask.Result - calculateDiscountAppliedTask.Result,
                PointsEarned = calculatePointsEarnedTask.Result
            };

            // @todo: save purchase via IPurchaseRepository
            return purchase;
        }
        private async Task<decimal> CalculateTotalAmount(IEnumerable<BasketItem> basketItems)
        {
            // Instead of taking the unit prices from the BasketItem (which were passed in from the webapi) we should rather lookup the unit prices from our trusted product repository instead
            var products = await ProductRepository.GetAsync(basketItems.Select(item => item.ProductId));

            var totalAmount = basketItems.Join(products, basketItem => basketItem.ProductId, product => product.Id, (basketItem, product) => new
            {
                basketItem.Quantity,
                product.UnitPrice
            }).Sum(item => item.Quantity * item.UnitPrice);

            return totalAmount;
        }
        private async Task<int> CalculatePointsEarned(DateTime transactionDate, IEnumerable<BasketItem> basketItems)
        {
            // We assume only one points promo can run at any given time (no overlapping). Otherwise an exception is thrown.
            var pointsPromotion = (await PointsPromotionRepository.GetAsync()).SingleOrDefault(pp => transactionDate >= pp.StartDate && transactionDate <= pp.EndDate);
            if (pointsPromotion != null)
            {
                // Get the products for which the points promotion applies (same product category)
                var products = (await ProductRepository.GetAsync(basketItems.Select(item => item.ProductId))).Where(p => pointsPromotion.Category == "Any" || p.Category == pointsPromotion.Category);

                // Get total amount (rounded down) eligible for points earned 
                var totalAmount = (int) basketItems.Join(products, basketItem => basketItem.ProductId, product => product.Id, (basketItem, product) => new
                {
                    basketItem.Quantity,
                    product.UnitPrice
                }).Sum(item => item.Quantity * item.UnitPrice);

                // Calculate the total points for each dollar spent
                return totalAmount * pointsPromotion.Points; 
            }
            return 0;
        }
        private async Task<decimal> CalculateDiscountApplied(DateTime transactionDate, IEnumerable<BasketItem> basketItems)
        {
            // Get products in basket
            var products = await ProductRepository.GetAsync(basketItems.Select(item => item.ProductId));

            // Get discount promotions on transactionDate 
            var discountPromotions = (await DiscountPromotionRepository.GetAsync()).Where(dp => transactionDate >= dp.StartDate && transactionDate <= dp.EndDate);

            // Get discount promotions
            var discountPromotionProducts = await DiscountPromotionProductRepository.GetAsync();

            // Join DiscountPromotionProducts with Products and DiscountPromotion
            var discountPromotionProductsJoined = discountPromotionProducts.Join(products, discountPromotionProduct => discountPromotionProduct.ProductId, product => product.Id, (discountPromotionProduct, product) => new
            {
                discountPromotionProduct.DiscountPromotionId,
                Product = product
            })
            .Join(discountPromotions, discountPromotionProductsJoinedWithProducts => discountPromotionProductsJoinedWithProducts.DiscountPromotionId, discountPromotion => discountPromotion.Id, (discountPromotionProductsJoinedWithProducts, discountPromotion) => new
            {
                discountPromotionProductsJoinedWithProducts.Product,
                DiscountPromotion = discountPromotion,
            });

            // Now, we need to calculate the discount applied on the actual purchased items
            var discountApplied = basketItems.Join(discountPromotionProductsJoined, basketItem => basketItem.ProductId, discountPromotionProduct => discountPromotionProduct.Product.Id, (basketItem, discountPromotionProduct) => new
            {
                basketItem.Quantity,
                discountPromotionProduct.DiscountPromotion,
                discountPromotionProduct.Product
            }).Sum(item => item.Quantity * item.Product.UnitPrice * item.DiscountPromotion.DiscountPercent/100m);

            return discountApplied;
        }

    }
}
