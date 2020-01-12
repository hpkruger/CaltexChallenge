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
            // In the real-world, structured logging should be used throughout the code base, e.g. inject ILogger<PurchasesController> here
            PurchaseService = purchaseService;
        }

        [HttpPost]
        public Task<Purchase> Purchase(PurchaseRequest request)
        {
            // The asp.net web api controller layer should be kept very simple and besides doing some validation and returning meaningful errors to the caller, it should simply delegate to the service layer (which represents the business logic)
            return PurchaseService.PurchaseAsync(request.CustomerId, request.LoyaltyCard, request.TransactionDate, request.Basket);
        }

    }
}
