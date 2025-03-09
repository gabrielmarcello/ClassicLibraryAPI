using ClassicLibraryAPI.Models;
using Stripe.Checkout;
using Stripe;

namespace ClassicLibraryAPI.Services {
    public class StripeService {

        private readonly string _secretKey = "";

        public StripeService() {
            StripeConfiguration.ApiKey = _secretKey;
        }

        public Session CriarSessaoCheckout(Pedido pedido) {
            var options = new SessionCreateOptions {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = pedido.Itens.Select(i => new SessionLineItemOptions {
                    PriceData = new SessionLineItemPriceDataOptions {
                        Currency = "brl",
                        UnitAmount = (long)(i.Livro.Preco * 100),
                        ProductData = new SessionLineItemPriceDataProductDataOptions {
                            Name = i.Livro.Titulo,
                        },
                    },
                    Quantity = i.Quantidade,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = "https://localhost:5001/pedido/success",
                CancelUrl = "https://localhost:5001/pedido/cancel",
            };

            var service = new SessionService();
            var session = service.Create(options);
            return session;
        }

        public bool VerificarPagamento(string sessionId) {
            var service = new SessionService();
            var session = service.Get(sessionId);

            return session.PaymentStatus == "paid";
        }
    }
}
