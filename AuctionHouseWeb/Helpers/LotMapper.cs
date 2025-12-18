using AuctionHouseWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace AuctionHouseWeb.Helpers;

public class LotMapper
{
    public static LotDto MapLotToDto(Lot lot, DateTime now, int? userId = null, bool includeWinningBid = false, bool includeFullHistory = false)
    {
        if (lot == null) return null;
        
        var bids = lot.Bids ?? new List<Bid>();
        var images = lot.Images ?? new List<LotImage>();

        var highestBid = bids.OrderByDescending(b => b.Amount).FirstOrDefault()?.Amount ?? lot.CurrentPrice;
        var bidCount = bids.Count;
        var uniqueBidders = bids.Select(b => b.UserId).Distinct().Count();
        var recentBids = bids.Count(b => b.TimeStamp >= now.AddMinutes(-5));

        bool isPopular = bidCount >= 5 || uniqueBidders >= 3;

        var lotBadges = new List<string>();
        if (isPopular) lotBadges.Add("Populær");
        
        var topBidder = bids.OrderByDescending(b => b.Amount).FirstOrDefault();
        if (topBidder != null && topBidder.UserId == userId) lotBadges.Add("Højest Budt");
        
        if (userId.HasValue && bids.Any(b => (lot.EndTime - b.TimeStamp).TotalMinutes <= 2 && b.UserId == userId)) 
            lotBadges.Add("Sidste Sekund Sniper");

        return new LotDto
        {
            Id = lot.Id,
            Title = lot.Title,
            Description = lot.Description,
            EstimatedValue = lot.EstimatedValue,
            StartPrice = lot.StartPrice,
            CurrentPrice = lot.CurrentPrice,
            EndTime = lot.EndTime,
            AuctionTitle = lot.Auction?.Title ?? "Ukendt Auktion",
            NextBid = lot.CurrentPrice * 1.10m,
            BidHistory = (includeFullHistory 
                ? bids.OrderByDescending(b => b.TimeStamp).Select(b => new { b.Amount, b.TimeStamp, b.UserId, b.IsAutoBid })
                : bids.OrderByDescending(b => b.TimeStamp).Take(5).Select(b => new { b.Amount, b.TimeStamp, b.UserId, b.IsAutoBid }))
                .Cast<object>().ToList(),
            ImageUrls = images.Select(img => img.Url).ToList(),
            MainImage = images.OrderBy(img => img.Id).FirstOrDefault()?.Url,
            BidCount = bidCount,
            UniqueBidders = uniqueBidders,
            RecentBids = recentBids,
            IsPopular = isPopular,
            HighestBid = highestBid,
            HasUserBid = userId.HasValue && bids.Any(b => b.UserId == userId.Value),
            LotBadges = lotBadges
        };
    }
}