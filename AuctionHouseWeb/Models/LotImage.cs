namespace AuctionHouseWeb.Models;

public class LotImage
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty; // URL eller path til image
    public int LotId { get; set; }
    public Lot Lot { get; set; } = null!;
}