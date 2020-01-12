using CaltexChallenge.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CaltexChallenge.Services
{
    public interface IPurchaseService
    {
        Task<Purchase> PurchaseAsync(string customerId, string loyaltyCard, DateTime transactionDate, IEnumerable<BasketItem> basket);
    }
}
