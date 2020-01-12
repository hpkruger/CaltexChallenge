using System;
using System.Collections.Generic;
using System.Text;

namespace CaltexChallenge.Data
{
    public class BasketItem
    {
        public string ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}
