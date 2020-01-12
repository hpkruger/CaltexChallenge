using CaltexChallenge.Data;
using CaltexChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaltexChallenge.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class PurchasesController : ControllerBase
    {
        private IPurchaseService PurchaseService { get; }

        public PurchasesController(IPurchaseService purchaseService)
        {
            PurchaseService = purchaseService;
        }

        [HttpPost]
        public Task<Purchase> Purchase(PurchaseRequest request)
        {
            return PurchaseService.PurchaseAsync(request.CustomerId, request.LoyaltyCard, request.TransactionDate, request.Basket);
        }

    }
}
