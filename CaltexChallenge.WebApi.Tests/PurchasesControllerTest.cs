using CaltexChallenge.Data;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CaltexChallenge.WebApi.Tests
{

    // There seem to be number of mistakes in the task description pdf for the coding challenge:

    // 1. Product ids in sample requests don't match up with product ids in the product table
    // 2. TotalAmount in sample response doesn't match up with SUM(Qty*UnitPrice) over all products in sample request
    // 3. DiscountApplied in sample response cannot be > 0 as there is no discount promotion at the TransactionDate of the sample response
    // 4. Discount promotion products table is wrong (discount for the same product twice)

    [TestClass]
    public class PurchasesControllerTest
    {
        private static IWebHost TestWebApiHost { get; set; }
        private HttpClient Client { get; set; } = new HttpClient { BaseAddress = new Uri("http://localhost:5000") };

        [ClassInitialize]
        public static void InitTests(TestContext context)
        {
            // Alternatively, we could use the asp.net TestServer classes here. 
            TestWebApiHost = WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
            TestWebApiHost.Start();
        }
        [ClassCleanup]
        public static void Cleanup()
        {
            TestWebApiHost.StopAsync().GetAwaiter().GetResult();
        }

        [TestMethod]
        public async Task TestPurchaseWithNoPromotionsApplied()
        {
            var request = new PurchaseRequest
            {
                CustomerId = "8e4e8991-aaee-495b-9f24-52d5d0e509c5",
                LoyaltyCard = "CTX0000001",
                TransactionDate = DateTime.Parse("03 Apr 2020"),
                Basket = new[]
                    {
                        new BasketItem { ProductId = "PRD01", Quantity = 1, UnitPrice = 1.2m },
                        new BasketItem { ProductId = "PRD02", Quantity = 5, UnitPrice = 1.3m },
                        new BasketItem { ProductId = "PRD05", Quantity = 3, UnitPrice = 5.1m },
                    }.ToList()
            };

            var response = await Client.PostAsJsonAsync("/Purchases/", request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var purchase = await response.Content.ReadAsAsync<Purchase>();
            Assert.IsNotNull(purchase);

            Assert.AreEqual(request.Basket.Sum(bi => bi.Quantity * bi.UnitPrice), purchase.TotalAmount);
            Assert.AreEqual(0, purchase.PointsEarned);
            Assert.AreEqual(0, purchase.DiscountApplied);
            Assert.AreEqual(purchase.TotalAmount, purchase.GrandTotal);
        }
        [TestMethod]
        public async Task TestPurchaseWithPointsButNoDiscountPromotionsApplied()
        {
            var request = new PurchaseRequest
            {
                CustomerId = "8e4e8991-aaee-495b-9f24-52d5d0e509c5",
                LoyaltyCard = "CTX0000001",
                TransactionDate = DateTime.Parse("01 Mar 2020"),
                Basket = new[]
                    {
                        new BasketItem { ProductId = "PRD01", Quantity = 1, UnitPrice = 1.2m },
                        new BasketItem { ProductId = "PRD02", Quantity = 5, UnitPrice = 1.3m },
                        new BasketItem { ProductId = "PRD05", Quantity = 3, UnitPrice = 5.1m },
                        new BasketItem { ProductId = "PRD06", Quantity = 2, UnitPrice = 3.4m },
                    }.ToList()
            };

            var response = await Client.PostAsJsonAsync("/Purchases/", request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var purchase = await response.Content.ReadAsAsync<Purchase>();
            Assert.IsNotNull(purchase);

            //Shop Promo points promotion on the 1st of Mar 2020 with 4 points for every dollar spent
            var expectedPointsEarned = (int)request.Basket.GetRange(2, 2).Sum(bi => bi.Quantity * bi.UnitPrice) * 4;

            Assert.AreEqual(request.Basket.Sum(bi => bi.Quantity * bi.UnitPrice), purchase.TotalAmount);
            Assert.AreEqual(expectedPointsEarned, purchase.PointsEarned);
            Assert.AreEqual(0, purchase.DiscountApplied);
            Assert.AreEqual(purchase.TotalAmount, purchase.GrandTotal);
        }
        [TestMethod]
        public async Task TestPurchaseWithPointsAndDiscountPromotionsApplied()
        {
            var request = new PurchaseRequest
            {
                CustomerId = "8e4e8991-aaee-495b-9f24-52d5d0e509c5",
                LoyaltyCard = "CTX0000001",
                TransactionDate = DateTime.Parse("5 Jan 2020"),
                Basket = new[]
                    {
                        new BasketItem { ProductId = "PRD01", Quantity = 1, UnitPrice = 1.2m },
                        new BasketItem { ProductId = "PRD02", Quantity = 5, UnitPrice = 1.3m },
                        new BasketItem { ProductId = "PRD05", Quantity = 3, UnitPrice = 5.1m },
                        new BasketItem { ProductId = "PRD06", Quantity = 2, UnitPrice = 3.4m },
                    }.ToList()
            };

            var response = await Client.PostAsJsonAsync("/Purchases/", request);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var purchase = await response.Content.ReadAsAsync<Purchase>();
            Assert.IsNotNull(purchase);

            //New Year Promo points promotion with 2 points for every dollar spent on any product categories
            var expectedPointsEarned = (int)request.Basket.Sum(bi => bi.Quantity * bi.UnitPrice) * 2;

            // PRD01 and PRD02 will be discounted by 20% due to the Fuel Discount Promo discount between 1 Jan - 15 Feb
            var expectedDiscountApplied = request.Basket.GetRange(0, 2).Sum(bi => bi.Quantity * bi.UnitPrice) * 20/100m;
            Assert.AreEqual(request.Basket.Sum(bi => bi.Quantity * bi.UnitPrice), purchase.TotalAmount);
            Assert.AreEqual(expectedPointsEarned, purchase.PointsEarned);
            Assert.AreEqual(expectedDiscountApplied, purchase.DiscountApplied);
            Assert.AreEqual(purchase.TotalAmount - expectedDiscountApplied, purchase.GrandTotal);
        }

    }
}
