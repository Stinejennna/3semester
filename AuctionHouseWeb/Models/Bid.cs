namespace AuctionHouseWeb.Models;

public class Bid
{
    public int Id { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    
    public int UserId { get; set; }
    public User User { get; set; }
    
    public int LotId { get; set; }
    public Lot Lot { get; set; }
    
    public bool IsAutoBid { get; set; }
}