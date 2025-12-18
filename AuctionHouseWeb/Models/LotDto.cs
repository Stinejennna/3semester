namespace AuctionHouseWeb.Models;

public class LotDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    
    public decimal EstimatedValue { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    
    public DateTime EndTime { get; set; }
    public string AuctionTitle { get; set; } = "";
    
    public decimal NextBid { get; set; }
    
    public object TimeLeft { get; set; } = null!;
    public object? TimeSinceEnd { get; set; }
    
    public List<object> BidHistory { get; set; } = new();
    public List<string> ImageUrls { get; set; } = new();
    public string? MainImage { get; set; }
    
    public int BidCount { get; set; }
    public decimal HighestBid { get; set; }
    public object? WinningBid { get; set; }
    public bool HasUserBid { get; set; }
    
    public int UniqueBidders { get; set; }
    public int RecentBids { get; set; } 
    public bool IsPopular { get; set; }
    
    public List<string> LotBadges { get; set; } = new();
}