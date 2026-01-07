using FiapCloudGames.Payments.Business;
using FiapCloudGames.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FiapCloudGames.Payments.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Payment payment)
        {
            await _paymentService.CreateAsync(payment);
            return Ok();
        }
    }
}
