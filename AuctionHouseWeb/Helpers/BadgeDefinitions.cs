namespace AuctionHouseWeb.Helpers;

public class BadgeDefinitions
{
    public static readonly Dictionary<string, int> BidBadges = new()
    {
        { "Første Bud", 1 },
        { "Regulær Byder", 10 },
        { "Power Byder", 50 },
        { "Byder Maniac", 100 }
    };

    public static readonly Dictionary<string, int> WinBadges = new()
    {
        { "Første Vind", 1 },
        { "Double Vinder", 2 },
        { "Auktions Pro", 5 },
        { "Auktions Konge", 10 }
    };

    public static readonly Dictionary<string, int> AutoBidBadges = new()
    {
        { "Automation Rookie", 1 },
        { "Strategisk Byder", 10 },
        { "Sniper Bot", 25 }
    };

    public static readonly Dictionary<string, int> ValueBadges = new()
    {
        { "High Roller", 50_000 },
        { "Big Spender", 10_000 },
        { "Elite Køber", 100_000 }
    };

    public static readonly Dictionary<string, int> TimingBadges = new()
    {
        { "Sidste Sekunds Sniper", 1 }
    };

    public static readonly Dictionary<string, int> ActivityBadges = new()
    {
        { "Ugentlig Aktiv", 7 },   // dage
        { "Fast Deltager", 30 }    // dage
    };

    public static readonly Dictionary<string, int> SocialBadges = new()
    {
        { "Top 10 Vinder", 1 }
    };
}