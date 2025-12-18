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
public class LotsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public LotsController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    
    // POST: api/Lots/{lotId}/upload-image
    [HttpPost("{lotId}/upload-image")]
    public async Task<IActionResult> UploadLotImage(int lotId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var lot = await _context.Lots.Include(l => l.Images).FirstOrDefaultAsync(l => l.Id == lotId);
        if (lot == null)
            return NotFound("Lot not found.");

        // Sikre upload mappe eksistere
        var uploadsDir = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        if (!Directory.Exists(uploadsDir))
            Directory.CreateDirectory(uploadsDir);

        // Generere unik filnavn
        var fileExt = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{fileExt}";
        var filePath = Path.Combine(uploadsDir, fileName);

        // Gem filen
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Build relative URL
        var url = $"/uploads/{fileName}";

        // Gem i database
        var lotImage = new LotImage()
        {
            LotId = lotId,
            Url = url
        };

        _context.LotImages.Add(lotImage);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            lotId,
            Url = url
        });
    }
    
    // GET: api/Lots/search
    [HttpGet("search")]
    public async Task<IActionResult> SearchLots(
        [FromQuery] string? query,
        [FromQuery] bool onlyActive = true,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] decimal? minEstimatedValue = null,
        [FromQuery] decimal? maxEstimatedValue = null,
        [FromQuery] int? categoryId = null,        
        [FromQuery] string? sortBy = "EndTime",   
        [FromQuery] bool descending = false,     
        [FromQuery] int page = 1,               
        [FromQuery] int pageSize = 20)            
    {
        var now = DateTime.UtcNow;
        
        var userIdClaim = User.FindFirst("id")?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

        var lotsQuery = _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Auction)
            .Include(l => l.Images)
            .AsQueryable();

        if (onlyActive)
            lotsQuery = lotsQuery.Where(l => l.EndTime > now);

        if (!string.IsNullOrEmpty(query))
            lotsQuery = lotsQuery.Where(l => l.Title.Contains(query) || l.Description.Contains(query));

        if (minPrice.HasValue)
            lotsQuery = lotsQuery.Where(l => l.CurrentPrice >= minPrice.Value);
        if (maxPrice.HasValue)
            lotsQuery = lotsQuery.Where(l => l.CurrentPrice <= maxPrice.Value);

        if (minEstimatedValue.HasValue)
            lotsQuery = lotsQuery.Where(l => l.EstimatedValue >= minEstimatedValue.Value);
        if (maxEstimatedValue.HasValue)
            lotsQuery = lotsQuery.Where(l => l.EstimatedValue <= maxEstimatedValue.Value);

        if (categoryId.HasValue)
            lotsQuery = lotsQuery.Where(l => l.CategoryId == categoryId.Value);

        lotsQuery = sortBy?.ToLower() switch
        {
            "currentprice" => descending ? lotsQuery.OrderByDescending(l => l.CurrentPrice) : lotsQuery.OrderBy(l => l.CurrentPrice),
            "estimatedvalue" => descending ? lotsQuery.OrderByDescending(l => l.EstimatedValue) : lotsQuery.OrderBy(l => l.EstimatedValue),
            "title" => descending ? lotsQuery.OrderByDescending(l => l.Title) : lotsQuery.OrderBy(l => l.Title),
            _ => descending ? lotsQuery.OrderByDescending(l => l.EndTime) : lotsQuery.OrderBy(l => l.EndTime)
        };

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var totalCount = await lotsQuery.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var lots = await lotsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = lots.Select(l => LotMapper.MapLotToDto(l, now, userId)).ToList();

        return Ok(new
        {
            Page = page,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalCount = totalCount,
            Lots = result
        });
    }
    
    // GET: api/Lots/upcoming
    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcomingLots()
    {
        var now = DateTime.UtcNow;
        var userIdClaim = User.FindFirst("id")?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

        var lots = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Auction)
            .Include(l => l.Images)
            .Where(l => l.EndTime > now)
            .OrderBy(l => l.EndTime)
            .ToListAsync();

        var result = lots.Select(l => LotMapper.MapLotToDto(l, now, userId)).ToList();

        return Ok(result);
    }
    
    // GET: api/Lots/active
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveBids()
    {
        var now = DateTime.UtcNow;
        var userIdClaim = User.FindFirst("id")?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

        var lots = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Auction)
            .Include(l => l.Images)
            .Where(l => l.EndTime > now)
            .ToListAsync();

        var result = lots.Select(l => LotMapper.MapLotToDto(l, now, userId)).ToList();

        return Ok(result);
    }
    
    // GET: api/Lots/recent
    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentlyClosedBids()
    {
        var now = DateTime.UtcNow;
        var since = now.AddHours(-24);
        
        var userIdClaim = User.FindFirst("id")?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

        var lots = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Auction)
            .Include(l => l.Images)
            .Where(l => l.EndTime <= now && l.EndTime >= since)
            .ToListAsync();

        var result = lots.Select(l => LotMapper.MapLotToDto(l, now, userId, includeWinningBid: true)).ToList();

        return Ok(result);
    }
    
    // GET: api/Lots/{lotId}/historyWithLot
    [HttpGet("{lotId}/historyWithLot")]
    public async Task<IActionResult> GetBidHistoryWithLot(int lotId)
    {
        var lot = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Auction)
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == lotId);

        if (lot == null)
            return NotFound("Lot not found");

        var now = DateTime.UtcNow;
        var userIdClaim = User.FindFirst("id")?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;
        
        var lotDto = LotMapper.MapLotToDto(lot, now, userId);

        var bidHistory = lot.Bids
            .OrderByDescending(b => b.TimeStamp)
            .Select(b => new
            {
                b.Amount,
                b.TimeStamp,
                b.UserId,
                b.IsAutoBid
            }).ToList();

        return Ok(new { Lot = lotDto, BidHistory = bidHistory });
    }
    
    // GET: api/Lots/{lotId}
    [HttpGet("{lotId}")]
    public async Task<IActionResult> GetLotDetail(int lotId)
    {
        var lot = await _context.Lots
            .Include(l => l.Bids)
            .Include(l => l.Auction)
            .Include(l => l.Images)
            .FirstOrDefaultAsync(l => l.Id == lotId);

        if (lot == null)
            return NotFound("Lot not found");

        var now = DateTime.UtcNow;
        var userIdClaim = User.FindFirst("id")?.Value;
        int? userId = userIdClaim != null ? int.Parse(userIdClaim) : null;

        var lotDto = LotMapper.MapLotToDto(lot, now, userId);

        return Ok(lotDto);
    }

}