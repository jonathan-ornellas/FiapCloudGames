using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Payments.Api.DTOs;
using FiapCloudGames.Payments.Business;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;

namespace FiapCloudGames.Payments.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Process(ProcessPaymentDto processPaymentDto)
    {
        var payment = new Payment(processPaymentDto.UserId, processPaymentDto.GameId, new Money(processPaymentDto.Amount), processPaymentDto.PaymentMethod);
        await _paymentService.ProcessPaymentAsync(payment);
        return Ok();
    }
}
