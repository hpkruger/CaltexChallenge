using System;
using System.Collections.Generic;
using System.Text;

namespace CaltexChallenge.Data
{
    public class PurchaseRequest
    {
        public string CustomerId { get; set; }
        public string LoyaltyCard { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<BasketItem> Basket { get; set; } = new List<BasketItem>();
    }
}