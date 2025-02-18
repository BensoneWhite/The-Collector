namespace TheCollector;

[BepInDependency("slime-cubed.slugbase")]
//This dependency can be deleted if manual spriting is done
[BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(MOD_ID, MOD_NAME, VERSION)]

public class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "TheCollector";
    public const string MOD_NAME = "The Collector";
    public const string VERSION = "0.1.0";

    //BepInEx Logger for easy console logs
    public new static ManualLogSource Logger;

    public static void DebugLog(object ex) => Logger.LogInfo(ex);
    public static void DebugWarning(object ex) => Logger.LogWarning(ex);
    public static void DebugError(object ex) => Logger.LogError(ex);
    public static void DebugFatal(object ex) => Logger.LogFatal(ex);

    static bool _Initialized;
    private TheCollectorOptionsMenu optionsMenuInstance;

    public void OnEnable()
    {
        try
        {
            Logger = base.Logger;
            //Enums should be initialized first
            TCEnums.Init();

            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
            On.RainWorld.OnModsDisabled += RainWorld_OnModsDisabled;
        }
        catch (Exception e)
        {
            DebugError(e);
        }
    }

    private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (_Initialized) { return; }
            _Initialized = true;

            PorlCollar.Init();

            StatsHooks.Init();
            CustomAbilities.init();

            MachineConnector.SetRegisteredOI(MOD_ID, optionsMenuInstance = new TheCollectorOptionsMenu());

            if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
            {
                SetupDMSSprites();
            }
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    private void RainWorld_OnModsDisabled(On.RainWorld.orig_OnModsDisabled orig, RainWorld self, ModManager.Mod[] newlyDisabledMods)
    {
        orig(self, newlyDisabledMods);
        try
        {
            foreach (var mod in newlyDisabledMods)
            {
                if (mod.id == MOD_ID || mod.id == "moreslugcats")
                {
                    TCEnums.Unregister();
                    DebugWarning($"Unregistering enums from: {MOD_NAME}");
                }
            }
        }
        catch (Exception ex)
        {
            DebugError(ex);
        }
    }

    public void SetupDMSSprites()
    {
        var sheetID = "TheCollector.Legacy";

        for (int index = 0; index < 4; index++)
        {
            SpriteDefinitions.AddSlugcatDefault(new Customization()
            {
                Slugcat = "TheCollector",
                PlayerNumber = index,
                CustomSprites = new List<CustomSprite>
                {
                    new CustomSprite() { Sprite = "HEAD", SpriteSheetID = sheetID, Color = Color.white },
                    new CustomSprite() { Sprite = "ARMS", SpriteSheetID = sheetID, Color = Color.white },
                    new CustomSprite() { Sprite = "BODY", SpriteSheetID = sheetID, Color = Color.white },
                    new CustomSprite() { Sprite = "HIPS", SpriteSheetID = sheetID, Color = Color.white },
                    new CustomSprite() { Sprite = "LEGS", SpriteSheetID = sheetID, Color = Color.white },
                    new CustomSprite() { Sprite = "TAIL", SpriteSheetID = sheetID, Color = Color.white },
                    new CustomSprite() { Sprite = "FACE", SpriteSheetID = sheetID, Color = Color.white },
                },

                CustomTail = new CustomTail()
                {
                    Length = 7,
                    Wideness = 4f,
                    Roundness = 0.7f
                }
            });
        }
    }
}    