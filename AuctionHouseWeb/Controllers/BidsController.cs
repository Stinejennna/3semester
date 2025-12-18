using AuctionHouseWeb.Data;
using AuctionHouseWeb.Models;
using AuctionHouseWeb.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BidsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public BidsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Bids/{lotId}
    [HttpPost("{lotId}")]
    public async Task<IActionResult> PlaceBid(int lotId, [FromBody] BidRequest request)
    {
    
        var userIdClaim = User.FindFirst("id")?.Value;
        if (userIdClaim == null) return Unauthorized();
        var userId = int.Parse(userIdClaim);
        
        var lot = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Images)
            .Include(l => l.Auction) // Vigtigt: Mappen bruger lot.Auction.Title
            .FirstOrDefaultAsync(l => l.Id == lotId);

        if (lot == null) return NotFound("Lot not found");
        if (DateTime.UtcNow > lot.EndTime) return BadRequest("Auction for this lot has ended.");
        
        var minBid = lot.CurrentPrice * 1.10m;
        if (request.Amount < minBid)
            return BadRequest($"Bid must be at least {minBid}.");
        
        var bid = new Bid
        {
            Amount = request.Amount,
            TimeStamp = DateTime.UtcNow,
            UserId = userId,
            LotId = lotId,
            IsAutoBid = request.MaxBid > 0
        };
        
        _context.Bids.Add(bid);
        
        if (request.MaxBid > 0)
        {
            var competingBids = lot.Bids
                .Where(b => b.IsAutoBid && b.Amount < request.MaxBid)
                .OrderByDescending(b => b.Amount)
                .ToList();

            decimal nextBid = Math.Max(lot.CurrentPrice * 1.10m, lot.CurrentPrice + 1);
            
            foreach (var cb in competingBids)
            {
                if (nextBid <= request.MaxBid && nextBid > cb.Amount)
                {
                    lot.CurrentPrice = nextBid;
                    cb.Amount = nextBid;
                    _context.Bids.Update(cb);
                    nextBid = nextBid * 1.10m;
                }
            }
            lot.CurrentPrice = Math.Min(nextBid, request.MaxBid);
        }
        else
        {
            lot.CurrentPrice = request.Amount;
        }
        
        if ((lot.EndTime - DateTime.UtcNow).TotalMinutes <= 2)
        {
            lot.EndTime = lot.EndTime.AddMinutes(2);
        }
        
        await _context.SaveChangesAsync();
        
        return Ok(LotMapper.MapLotToDto(lot, DateTime.UtcNow, userId, true, true));
    }
    
    // GET: api/Bids/{lotId}/history
    [HttpGet("{lotId}/history")]
    public async Task<IActionResult> GetBidHistory(int lotId)
    {
        var bids = await _context.Bids
            .Where(b => b.LotId == lotId)
            .OrderByDescending(b => b.TimeStamp)
            .Select(b => new
            {
                b.Amount,
                b.TimeStamp,
                b.UserId,
                b.IsAutoBid
            })
            .ToListAsync();
        
        return Ok(bids);
    }
}