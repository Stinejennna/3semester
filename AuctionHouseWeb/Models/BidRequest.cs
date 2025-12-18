namespace AuctionHouseWeb.Models;

public class BidRequest
{
    public decimal Amount { get; set; }       // Start bid
    public decimal MaxBid { get; set; } = 0;  // Optional: auto-bid maks
}