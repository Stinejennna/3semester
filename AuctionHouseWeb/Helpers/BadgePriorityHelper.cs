namespace AuctionHouseWeb.Helpers;

public class BadgePriorityHelper
{
    private static readonly List<string> Priority = new()
    {
        "Auktions Konge",
        "Auktions Pro",
        "Double Vinder",
        "Første Vind",
        "Byder Maniac",
        "Power Byder",
        "Regulær Byder",
        "Første Bud"
    };

    public static string? GetFeaturedBadge(List<string> badges)
    {
        return Priority.FirstOrDefault(b => badges.Contains(b));
    }
}