namespace AuctionHouseWeb.Models;

// DTO for webhook requests fra betalingsgateway.

public class PaymentWebhookRequest
{
    // Den unikke reference til betalingen, som gemmes i lot.PaymentReference.
    public string PaymentReference { get; set; } = "";
    
    // Status for betalingen. Fx "completed", "failed".
    public string Status { get; set; } = "";
}