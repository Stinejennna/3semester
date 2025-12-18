using AuctionHouseWeb.Data;
using AuctionHouseWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseWeb.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;
    
    public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    
    // POST: api/Admin/CreateAuction
    [HttpPost("CreateAuction")]
    public async Task<IActionResult> CreateAuction([FromForm] AuctionRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.LotTitle))
            return BadRequest("Manglende påkrævede felter.");

        var auction = new Auction
        {
            Title = request.Title,
            StartDate = request.StartDate ?? DateTime.UtcNow,
            EndDate = request.EndDate ?? DateTime.UtcNow.AddDays(1)
        };

        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync();

        var lot = new Lot
        {
            Title = request.LotTitle,
            Description = request.LotDescription,
            EstimatedValue = request.EstimatedValue ?? 0m,
            StartPrice = request.StartPrice ?? 0m,
            CurrentPrice = request.StartPrice ?? 0m,
            EndTime = auction.StartDate.AddHours(12),
            AuctionId = auction.Id
        };

        _context.Lots.Add(lot);
        await _context.SaveChangesAsync();

        // Upload billeder hvis der er nogen
        if (request.Images != null && request.Images.Any())
        {
            var wwwRootPath = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            
            var uploadDir = Path.Combine(wwwRootPath, "uploads", $"Lot_{lot.Id}");

            if (!Directory.Exists(uploadDir)) 
                Directory.CreateDirectory(uploadDir);

            foreach (var image in request.Images)
            {
                if (image.Length > 0)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(uploadDir, fileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await image.CopyToAsync(stream);

                    var imageUrl = $"/uploads/Lot_{lot.Id}/{fileName}";
                    _context.LotImages.Add(new LotImage { LotId = lot.Id, Url = imageUrl });
                }
            }
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            AuctionId = auction.Id,
            LotId = lot.Id,
            lot.Title,
            lot.CurrentPrice
        });
    }
    
    [HttpPut("EditAuction/{auctionId}")]
    public async Task<IActionResult> EditAuction(int auctionId, [FromForm] AuctionRequest request)
    { 
        var auction = await _context.Auctions.FindAsync(auctionId);
        if (auction == null)
            return NotFound("Auction not found");

        auction.Title = request.Title ?? auction.Title;
        auction.StartDate = request.StartDate ?? auction.StartDate;
        auction.EndDate = request.EndDate ?? auction.EndDate;

        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync();

        return Ok(auction);
    }
    
    [HttpPost("CloseAuction/{auctionId}")]
    public async Task<IActionResult> CloseAuction(int auctionId)
    {
        var auction = await _context.Auctions
            .Include(a => a.Lots)
            .ThenInclude(l => l.Bids)
            .FirstOrDefaultAsync(a => a.Id == auctionId);

        if (auction == null)
            return NotFound("Auction not found");

        // Sæt enddate til nu og markér lots som solgt hvor bud findes
        auction.EndDate = DateTime.UtcNow;

        foreach (var lot in auction.Lots)
        {
            if (lot.Bids.Any())
                lot.IsSold = true;

            _context.Lots.Update(lot);
        }

        _context.Auctions.Update(auction);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Auction closed successfully" });
    }
    
    // GET: api/Admin/GetAllAuctions
    [HttpGet("GetAllAuctions")]
    public async Task<IActionResult> GetAllAuctions()
    {
        var auctions = await _context.Auctions
            .Include(a => a.Lots)
            .ThenInclude(l => l.Images)
            .ToListAsync();
    
        return Ok(auctions);
    }
    
    [HttpGet("GetAuction/{id}")]
    public async Task<IActionResult> GetAuction(int id)
    {
        var auction = await _context.Auctions
            .Include(a => a.Lots)
            .ThenInclude(l => l.Images)
            .Include(a => a.Lots)
            .ThenInclude(l => l.Bids)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (auction == null) return NotFound();
        return Ok(auction);
    }
    
    // DELETE: api/Admin/DeleteAuction/{id}
    [HttpDelete("DeleteAuction/{id}")]
    public async Task<IActionResult> DeleteAuction(int id)
    {
        var auction = await _context.Auctions.FindAsync(id);
        if (auction == null) return NotFound();

        _context.Auctions.Remove(auction);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpPost("UploadLotImages/{lotId}")]
    public async Task<IActionResult> UploadLotImages(int lotId, [FromForm] List<IFormFile> images)
    {
        var lot = await _context.Lots
            .Include(l => l.Images) 
            .FirstOrDefaultAsync(l => l.Id == lotId);
        
        if (lot == null)
            return NotFound("Lot not found");

        if (images == null || !images.Any())
            return BadRequest("No images provided");

        // Per-lot mappe
        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", $"Lot_{lotId}");
        if (!Directory.Exists(uploadDir))
            Directory.CreateDirectory(uploadDir);

        var uploadedUrls = new List<string>();

        foreach (var image in images)
        {
            if (image.Length > 0)
            {
                var fileName = Path.GetFileName(image.FileName);
                var filePath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Gem billede URL i database
                var imageUrl = $"/uploads/Lot_{lotId}/{fileName}";
                _context.LotImages.Add(new LotImage
                {
                    LotId = lotId,
                    Url = imageUrl
                });

                uploadedUrls.Add(imageUrl);
            }
        }

        await _context.SaveChangesAsync();
        
        // Bestem MainImage efter saving 
        var mainImage = lot.Images.OrderBy(img => img.Id).FirstOrDefault()?.Url;
        if (mainImage == null && uploadedUrls.Any())
        {
            mainImage = uploadedUrls.OrderBy(url => url).First();
        }

        return Ok(new
        {
            LotId = lotId,
            UploadedImages = uploadedUrls,
            MainImage = mainImage
        });
    }
    
    // GET: api/Admin/GetAllUsers
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // DELETE: api/Admin/DeleteUser/{id}
    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok();
    }
    
    [HttpGet("DashboardStats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        var totalAuctions = await _context.Auctions.CountAsync();
        var activeAuctions = await _context.Auctions.CountAsync(a => a.EndDate > DateTime.UtcNow);
        var totalUsers = await _context.Users.CountAsync();

        return Ok(new
        {
            totalAuctions,
            activeAuctions,
            totalUsers
        });
    }
    
    [HttpGet("Payments")]
    public async Task<IActionResult> GetAllPayments()
    {
        var payments = await _context.Payments
            .Include(p => p.User)
            .Include(p => p.Lot)
            .OrderByDescending(p => p.Timestamp)
            .ToListAsync();

        return Ok(payments.Select(p => new
        {
            p.Id,
            User = new
            {
                p.User.Id,
                p.User.UserName,
                p.User.Email
            },
            Lot = new { p.Lot.Id, p.Lot.Title },
            p.Amount,
            p.Status,
            p.Timestamp
        }));
    }
}