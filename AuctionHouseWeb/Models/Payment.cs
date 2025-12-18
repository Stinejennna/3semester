namespace AuctionHouseWeb.Models;

public class Payment
{
    public int Id { get; set; }
    public int UserId { get; set; }
    
    public int LotId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
    
    public string Status { get; set; } = "Pending";

    public User User { get; set; } = null!;
    public Lot Lot { get; set; } = null!;
}