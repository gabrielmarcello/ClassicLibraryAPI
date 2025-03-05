using ClassicLibraryAPI.Models;
using ClassicLibraryAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClassicLibraryAPI.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class PedidoController : ControllerBase{

        private readonly StripeService _stripeService;

        public PedidoController(StripeService stripeService) {
            _stripeService = stripeService;
        }

        [HttpPost("criar-pedido")]
        public IActionResult CriarPedido([FromBody] Pedido pedido) {
            var session = _stripeService.CriarSessaoCheckout(pedido);
            return Ok(new { sessionId = session.Id, url = session.Url });
        }

        [HttpGet("success")]
        public IActionResult Success() {
            return Ok("Pagamento confirmado com sucesso!");
        }

        [HttpGet("cancel")]
        public IActionResult Cancel() {
            return Ok("Pagamento cancelado.");
        }
    }
}
