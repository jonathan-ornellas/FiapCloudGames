using Microsoft.AspNetCore.Mvc;
using FiapCloudGames.Payments.Api.DTOs;
using FiapCloudGames.Payments.Business;
using FiapCloudGames.Domain.Entities;
using FiapCloudGames.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using FiapCloudGames.Payments.Api.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FiapCloudGames.Payments.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly PaymentsContext _context;

    public PaymentsController(IPaymentService paymentService, PaymentsContext context)
    {
        _paymentService = paymentService;
        _context = context;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Process(ProcessPaymentDto processPaymentDto)
    {
        var payment = new Payment(
            processPaymentDto.UserId, 
            processPaymentDto.GameId, 
            new Money(processPaymentDto.Amount), 
            processPaymentDto.PaymentMethod
        );
        
        await _paymentService.CreateAsync(payment);
        await _paymentService.ProcessPaymentAsync(payment);
        
        return Ok(new { 
            Message = "Payment processed successfully", 
            PaymentId = payment.Id,
            Status = payment.Status 
        });
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

        var payment = await _context.Payments
            .Where(p => p.Id == Guid.Parse(id.ToString()))
            .Select(p => new
            {
                p.Id,
                p.UserId,
                p.GameId,
                p.Amount,
                p.PaymentMethod,
                p.Status,
                p.CreatedAt,
                p.ProcessedAt
            })
            .FirstOrDefaultAsync();
        
        if (payment == null)
            return NotFound();

        return Ok(payment);
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<IActionResult> GetUserPayments()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            return Unauthorized();

        var payments = await _context.Payments
            .Where(p => p.UserId.ToString() == userIdClaim || User.IsInRole("Admin"))
            .Select(p => new
            {
                p.Id,
                p.UserId,
                p.GameId,
                p.Amount,
                p.PaymentMethod,
                p.Status,
                p.CreatedAt,
                p.ProcessedAt
            })
            .ToListAsync();

        return Ok(payments);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        var validStatuses = new[] { "Pending", "Completed", "Failed", "Refunded" };
        if (!validStatuses.Contains(status))
            return BadRequest("Invalid status");

        var payment = await _context.Payments.FindAsync(id);
        if (payment == null)
            return NotFound();

        payment.Status = status;
        payment.ProcessedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
