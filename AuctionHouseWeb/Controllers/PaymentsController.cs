using AuctionHouseWeb.Data;
using AuctionHouseWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public PaymentsController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // Opretter en "simuleret" betaling for en vundet auktion
    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] PaymentRequest request)
    {
        var lot = await _context.Lots
            .Include(l => l.Bids)
            .FirstOrDefaultAsync(l => l.Id == request.LotId);

        if (lot == null) return NotFound("Lot not found");
        if (lot.IsSold) return BadRequest("Lot is already sold");

        var winningBid = lot.Bids.OrderByDescending(b => b.Amount).FirstOrDefault();
        if (winningBid == null) return BadRequest("No bids found for this lot");

        // Beregn total pris
        decimal hammerPrice = winningBid.Amount;
        decimal commission = hammerPrice * 0.30m;
        decimal hammerFee = 250m;
        decimal totalAmount = hammerPrice + commission + hammerFee;

        // Simuler betalingsreference
        string paymentReference = Guid.NewGuid().ToString();

        // Simuler "betalingslink" afhængig af metode
        string? paymentUrl = request.PaymentMethod.ToLower() switch
        {
            "paypal" => $"https://www.paypal.com/checkoutnow?token={paymentReference}",
            "mobilepay" => $"https://mobilepay.dk/pay?ref={paymentReference}",
            _ => null
        };

        if (paymentUrl == null) return BadRequest("Unsupported payment method");
        
        var payment = new Payment
        {
            UserId = winningBid.UserId,
            LotId = lot.Id,
            Amount = totalAmount,
            Status = "Pending",
            Timestamp = DateTime.UtcNow
        };
        
        _context.Payments.Add(payment);

        // Gem reference + totalpris (ikke solgt endnu)
        lot.PaymentReference = paymentReference;
        lot.FinalPrice = totalAmount;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            LotId = lot.Id,
            TotalAmount = totalAmount,
            HammerPrice = hammerPrice,
            Commission = commission,
            HammerFee = hammerFee,
            PaymentReference = paymentReference,
            PaymentUrl = paymentUrl
        });
    }
    
    // "Webhook" til at simulere gateway callback
    [HttpPost("webhook")]
    [AllowAnonymous] // Kommer fra gateway, ikke bruger
    public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookRequest request)
    {
        var lot = await _context.Lots
            .FirstOrDefaultAsync(l => l.PaymentReference == request.PaymentReference);

        if (lot == null) return NotFound("Lot not found");
        
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.LotId == lot.Id && p.Status == "Pending");

        if (payment == null) return NotFound("Payment not found");


        if (request.Status.ToLower() == "completed")
        {
            lot.IsSold = true;
            lot.SoldAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Payment confirmed, lot marked as sold." });
        }

        return BadRequest(new { Message = $"Payment status: {request.Status}" });
    }
}
