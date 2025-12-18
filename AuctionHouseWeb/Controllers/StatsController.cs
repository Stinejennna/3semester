using AuctionHouseWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseWeb.Controllers;


[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public StatsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("leaderboard")]
    public async Task<IActionResult> GetLeaderboard()
    {
        var leaderboard = await _context.Lots
            .Where(l => l.IsSold)
            .SelectMany(l => l.Bids
                .OrderByDescending(b => b.Amount)
                .Take(1)
                .Select(b => new
                {
                    b.UserId,
                    l.FinalPrice
                }))
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                WonAuctions = g.Count(),
                TotalWonValue = g.Sum(x => x.FinalPrice ?? 0)
            })
            .OrderByDescending(x => x.WonAuctions)
            .ThenByDescending(x => x.TotalWonValue)
            .Take(10)
            .ToListAsync();

        return Ok(leaderboard);
    }
}