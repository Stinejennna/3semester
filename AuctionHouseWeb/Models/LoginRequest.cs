using System.ComponentModel.DataAnnotations;

namespace AuctionHouseWeb.Models;

public class LoginRequest
{
    [Required]
    public string Identifier { get; set; } = string.Empty; 
    // kan være email eller username

    [Required]
    public string Password { get; set; } = string.Empty;
}
