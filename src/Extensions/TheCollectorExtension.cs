namespace TheCollector;

public static class TheCollectorExtension
{
    private static readonly ConditionalWeakTable<Player, TheCollectorPlayerData> _cwttc = new();

    public static TheCollectorPlayerData TheCollector(this Player player) => _cwttc.GetValue(player, _ => new(player));

    public static bool IsTheCollector(this Player player) => player.TheCollector().IsCollector;

    public static bool IsTheCollector(this Player player, out TheCollectorPlayerData data)
    {
        data = player.TheCollector();
        return data.IsCollector;
    }
}