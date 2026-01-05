namespace FiapCloudGames.Payments.Api.Controllers;

using FiapCloudGames.Payments.Api.DTOs;
using FiapCloudGames.Payments.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<ActionResult<PaymentResponse>> ProcessPayment([FromBody] CreatePaymentRequest request)
    {
        var response = await _paymentService.ProcessPaymentAsync(request);
        return CreatedAtAction(nameof(GetPaymentById), new { id = response.Id }, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PaymentResponse>> GetPaymentById(Guid id)
    {
        var payment = await _paymentService.GetPaymentByIdAsync(id);
        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetPaymentsByUser(Guid userId)
    {
        var payments = await _paymentService.GetPaymentsByUserAsync(userId);
        return Ok(payments);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<PaymentResponse>> UpdatePaymentStatus(Guid id, [FromBody] PaymentStatusRequest request)
    {
        try
        {
            var response = await _paymentService.UpdatePaymentStatusAsync(id, request.Status);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
