using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;

namespace server.Controllers
{
    public class PaymentsController : Controller
    {
        public readonly IOptions<StripeOptions> options;
        private readonly IStripeClient client;

        public PaymentsController(IOptions<StripeOptions> options)
        {
            this.options = options;
            this.client = new StripeClient(this.options.Value.SecretKey);
        }

        [HttpGet("setup")]
        public SetupResponse Setup()
        {
            return new SetupResponse
            {
                ProPrice = this.options.Value.ProPrice,
                BasicPrice = this.options.Value.BasicPrice,
                PublishableKey = this.options.Value.PublishableKey,
            };
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CreateCheckoutSessionRequest req)
        {
            var options = new SessionCreateOptions
            {
                SuccessUrl = $"{this.options.Value.Domain}/success.html?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{this.options.Value.Domain}/cancel.html",
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                Mode = "subscription",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = req.PriceId,
                        Quantity = 1,
                    },
                },
            };
            var service = new SessionService(this.client);
            try
            {
                var session = await service.CreateAsync(options);
                return Ok(new CreateCheckoutSessionResponse
                {
                    SessionId = session.Id,
                });
            }
            catch (StripeException e)
            {
                Console.WriteLine(e.StripeError.Message);
                return BadRequest(new ErrorResponse
                {
                    ErrorMessage = new ErrorMessage
                    {
                        Message = e.StripeError.Message,
                    }
                });
            }
        }

        [HttpPost("update-subs-item")]
        public async Task<IActionResult> UpdateSubsItem([FromBody] UpdateSubscriptionItemRequest req)
        {
            Console.WriteLine(req);
            StripeConfiguration.ApiKey = "";

            var options = new SubscriptionItemUpdateOptions
            {
                Price = req.PriceId

            };
            var service = new SubscriptionItemService();
            var session = service.Update(req.Id, options);
            return Ok(new CreateCheckoutSessionResponse
            {
                SessionId = session.Id,
            });
        }

        [HttpDelete("delete-subs/{id}")]
        public async Task<IActionResult> DeleteSubs(string id)
        {
            StripeConfiguration.ApiKey = "";

            var service = new SubscriptionService();
            return Ok(service.Cancel(id));
        }

        [HttpGet("checkout-session")]
        public async Task<IActionResult> CheckoutSession(string sessionId)
        {
            var service = new SessionService(this.client);
            var session = await service.GetAsync(sessionId);
            return Ok(session);
        }
        [HttpGet("get-prices/")]
        public async Task<IActionResult> GetAllPrices()
        {
            StripeConfiguration.ApiKey = "";

            var options = new PriceListOptions { Limit = 3 };
            var service = new PriceService();
            StripeList<Price> prices = service.List(options);
            return Ok(prices);
        }

        [HttpGet("get-prods/{id}")]
        public async Task<IActionResult> GetProds(string id)
        {
            StripeConfiguration.ApiKey = "";

            var service = new ProductService();
            return Ok(service.Get(id));
        }
        [HttpGet("get-prods/")]
        public async Task<IActionResult> GetAllProds()
        {
            StripeConfiguration.ApiKey = "";

            var service = new ProductService();
            StripeList<Product> products = service.List();
            return Ok(products);
        }
        [HttpGet("get-subs/{id}")]
        public async Task<IActionResult> GetSubs(string id)
        {
            StripeConfiguration.ApiKey = "";

            var service = new SubscriptionService();
            return Ok(service.Get(id));
        }

        [HttpGet("get-subs-item/{id}")]
        public async Task<IActionResult> GetSubsItem(string id)
        {
            StripeConfiguration.ApiKey = "";

            var service = new SubscriptionItemService();
            return Ok(service.Get(id));
        }

        [HttpDelete("delete-subs-item/{id}")]
        public async Task<IActionResult> DeleteSubsItem(string id)
        {
            StripeConfiguration.ApiKey = "";

            var service = new SubscriptionService();
            return Ok(service.Cancel(id));
        }
        [HttpGet("get-subs")]
        public async Task<IActionResult> GetAllSubs()
        {
            StripeConfiguration.ApiKey = "";

            var service = new SubscriptionService();
            StripeList<Subscription> subscriptions = service.List();
            return Ok(subscriptions);

        }

        [HttpPost("customer-portal")]
        public async Task<IActionResult> CustomerPortal([FromBody] CustomerPortalRequest req)
        {
            // For demonstration purposes, we're using the Checkout session to retrieve the customer ID. 
            // Typically this is stored alongside the authenticated user in your database.
            var checkoutSessionId = req.SessionId;
            var checkoutService = new SessionService(this.client);
            var checkoutSession = await checkoutService.GetAsync(checkoutSessionId);

            // This is the URL to which your customer will return after
            // they are done managing billing in the Customer Portal.
            var returnUrl = this.options.Value.Domain;

            var options = new Stripe.BillingPortal.SessionCreateOptions
            {
                Customer = checkoutSession.CustomerId,
                ReturnUrl = returnUrl,
            };
            var service = new Stripe.BillingPortal.SessionService(this.client);
            var session = await service.CreateAsync(options);

            return Ok(session);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    this.options.Value.WebhookSecret
                );
                Console.WriteLine($"Webhook notification with type: {stripeEvent.Type} found for {stripeEvent.Id}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Something failed {e}");
                return BadRequest();
            }

            if (stripeEvent.Type == "checkout.session.completed")
            {
                var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                Console.WriteLine($"Session ID: {session.Id}");
                // Take some action based on session.
            }

            return Ok();
        }
    }
}
