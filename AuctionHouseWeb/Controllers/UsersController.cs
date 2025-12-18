using AuctionHouseWeb.Data;
using AuctionHouseWeb.Helpers;
using AuctionHouseWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseWeb.Controllers;

[ApiController]
[Route("api/users/me")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    private int GetUserId()
        => int.Parse(User.FindFirst("id")!.Value);

    [HttpGet("bids")]
    public async Task<IActionResult> GetMyBids()
    {
        var userId = GetUserId();
        
        var bids = await _context.Bids
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.TimeStamp)
            .Select(b => new
            {
                b.Amount,
                b.TimeStamp,
                b.IsAutoBid,
                LotId = b.LotId,
                LotTitle = b.Lot.Title,
                AuctionTitle = b.Lot.Auction.Title,
                IsWinningBid = b.Amount == b.Lot.CurrentPrice && b.Lot.EndTime <= DateTime.UtcNow
            })
            .ToListAsync();

        return Ok(bids);
    }
    
    [HttpGet("active-auctions")]
    public async Task<IActionResult> GetMyActiveAuctions()
    {
        var userId = GetUserId();
        var now = DateTime.UtcNow;

        var lots = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Images)
            .Include(l => l.Auction) // Inkluderet så LotMapper kan læse auktions titlen
            .Where(l =>
                l.EndTime > now &&
                l.Bids.Any(b => b.UserId == userId))
            .ToListAsync();

        return Ok(lots.Select(l => LotMapper.MapLotToDto(l, now, userId)));
    }
    
    [HttpGet("won-auctions")]
    public async Task<IActionResult> GetWonAuctions()
    {
        var userId = GetUserId();
        var now = DateTime.UtcNow;

        var lots = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Images)
            .Include(l => l.Auction)
            .Where(l =>
                l.IsSold &&
                l.Bids.OrderByDescending(b => b.Amount)
                    .First().UserId == userId)
            .ToListAsync();

        return Ok(lots.Select(l => LotMapper.MapLotToDto(l, now, userId, includeWinningBid: true)));
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = GetUserId();
        var user = await _context.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.FullName,
                u.UserName,
                u.Email,
                u.Role,
                u.CreatedAt
            })
            .FirstOrDefaultAsync();

        if (user == null)
            return NotFound();

        return Ok(user);
    }
    
    [HttpGet("stats")]
    public async Task<IActionResult> GetMyStats()
    {
        var userId = GetUserId();
        
        var bids = await _context.Bids
            .Include(b => b.Lot)
            .Where(b => b.UserId == userId)
            .ToListAsync();
        
        var wonLots = await _context.Lots
            .Include(l => l.Bids)
            .Where(l => l.IsSold && l.Bids.OrderByDescending(b => b.Amount).First().UserId == userId)
            .ToListAsync();

        int totalBids = bids.Count;
        int wonAuctions = wonLots.Count;
        decimal totalWonValue = wonLots.Sum(l => l.FinalPrice ?? 0);
        int autoBidsCount = bids.Count(b => b.IsAutoBid);
        
        var badges = CalculateUserBadges(userId, totalBids, wonAuctions, totalWonValue, autoBidsCount, bids);
        
        var top10UserIds = await _context.Lots
            .Where(l => l.IsSold)
            .SelectMany(l => l.Bids
                .OrderByDescending(b => b.Amount)
                .Take(1)
                .Select(b => b.UserId))
            .GroupBy(id => id)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .Select(g => g.Key)
            .ToListAsync();

        if (top10UserIds.Contains(userId)) badges.Add("Top 10 Vinder");

        return Ok(new
        {
            TotalBids = totalBids, 
            WonAuctions = wonAuctions,
            TotalWonValue = totalWonValue,
            Badges = badges.Distinct().ToList()
        });
    }

    private List<string> CalculateUserBadges(int userId, int totalBids, int wonAuctions, decimal totalSpent, int autoBids, List<Bid> allBids)
    {
        var badges = new List<string>();

        // --- Bud Statistik ---
        if (totalBids >= 1) badges.Add("Første Bud");
        if (totalBids >= 10) badges.Add("Regulær Byder");
        if (totalBids >= 50) badges.Add("Power Byder");
        if (totalBids >= 100) badges.Add("Byder Maniac");

        // --- Vinder Statistik ---
        if (wonAuctions >= 1) badges.Add("Første Vind");
        if (wonAuctions >= 2) badges.Add("Double Vinder");
        if (wonAuctions >= 10) badges.Add("Auktions Pro");
        if (wonAuctions >= 25) badges.Add("Auktions Konge");

        // --- Økonomi Statistik ---
        if (totalSpent >= 1000) badges.Add("High Roller");
        if (totalSpent >= 10000) badges.Add("Big Spender");
        if (totalSpent >= 50000) badges.Add("Elite Køber");

        // --- Specielle Badges ---
        if (autoBids > 0) badges.Add("Automation Rookie");
        if (autoBids >= 20) badges.Add("Strategisk Byder");
        
        // Sidste Sekunds Sniper (Bud indenfor 2 min af slut)
        if (allBids.Any(b => b.Lot != null && (b.Lot.EndTime - b.TimeStamp).TotalMinutes <= 2))
        {
            badges.Add("Sidste Sekunds Sniper");
        }
    
        return badges;
    }
}