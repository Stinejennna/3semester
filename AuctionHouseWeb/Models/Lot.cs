namespace AuctionHouseWeb.Models;

public class Lot
{
    public int Id { get; set; }
    
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    
    public decimal EstimatedValue { get; set; }
    public decimal StartPrice { get; set; }
    public decimal CurrentPrice { get; set; }
    
    public DateTime EndTime { get; set; }
    
    public int AuctionId { get; set; }
    public Auction Auction { get; set; }
    
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();

    public ICollection<LotImage> Images { get; set; } = new List<LotImage>();
    
    public IEnumerable<string> ImageUrls => Images.Select(i => i.Url);
    
    public bool IsSold { get; set; } = false;
    
    public decimal? FinalPrice { get; set; }
    
    public DateTime? SoldAt { get; set; }
    
    public string? PaymentReference { get; set; }
}