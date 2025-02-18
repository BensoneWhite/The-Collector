namespace TheCollector;

public static class TCEnums
{
    public static SlugcatStats.Name TheCollector = new("TheCollector");

    public static void Init()
    {
        RuntimeHelpers.RunClassConstructor(typeof(Sound).TypeHandle);
    }

    public static void Unregister()
    {
        Utils.UnregisterEnums(typeof(Sound));
    }

    public static class Sound
    {
        public static SoundID porl = new(nameof(porl), true);
        public static SoundID flap = new(nameof(flap), true);
        public static SoundID wind = new(nameof(wind), true);
    }
}