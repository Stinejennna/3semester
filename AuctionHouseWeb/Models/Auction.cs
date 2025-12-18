namespace AuctionHouseWeb.Models;

public class Auction
{
    public int Id { get; set; }
    
    public string Title { get; set; } = String.Empty;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }

    public ICollection<Lot> Lots { get; set; } = new List<Lot>();
}