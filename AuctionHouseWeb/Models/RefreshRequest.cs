using System.ComponentModel.DataAnnotations;

namespace AuctionHouseWeb.Models;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
