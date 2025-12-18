namespace AuctionHouseWeb.Models;

// DTO sendt fra frontend for at starte en betaling.

public class PaymentRequest
{
    public int LotId { get; set; }

    /// <summary>
    /// Fx "PayPal" eller "MobilePay"
    /// </summary>
    public string PaymentMethod { get; set; } = "PayPal";
    public string Currency { get; set; } = "DKK";
}