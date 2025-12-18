using System.ComponentModel.DataAnnotations;

namespace AuctionHouseWeb.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    public string FullName { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    public string UserName { get; set; } = string.Empty;
    
    public string Role { get; set; } = "User";
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? RefreshToken { get; set; } 
    public DateTime? RefreshTokenExpiry { get; set; }
}