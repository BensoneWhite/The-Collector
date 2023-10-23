using BepInEx;
using static SlugBase.Features.FeatureTypes;
using SlugBase.Features;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DressMySlugcat;
using System.Security.Permissions;
using static DressMySlugcat.SpriteDefinitions;

#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace TheCollector
{
    [BepInDependency("slime-cubed.slugbase")]
    [BepInDependency("dressmyslugcat", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("TheCollector", "The Collector", "0.1.0")]

    class Plugin : BaseUnityPlugin
    {
        static bool _Initialized;
        private TheCollectorOptionsMenu optionsMenuInstance;

        private void LogInfo(object ex) => Logger.LogInfo(ex);

        public void OnEnable()
        {
            LogInfo("The Collector is working? SO TRUE! IT IS!!");
            On.RainWorld.OnModsInit += RainWorld_OnModsInit;
        }

        public static readonly PlayerFeature<int> slideStamina = PlayerInt("collector/SlideStamina");
        public static readonly PlayerFeature<float> SlideRecovery = PlayerFloat("collector/SlideRecovery");
        public static readonly PlayerFeature<float> SlideSpeed = PlayerFloat("collector/SlideSpeed");
        public static readonly PlayerFeature<bool> Collector = PlayerBool("collector/Collector");

        private void RainWorld_OnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            try
            {
                if (_Initialized) { return; }
                _Initialized = true;

                TheCollectorEnums.RegisterValues();
                CollarCollector.Init();

                TheCollectorRelativeStats.Init();
                TheCollectorFlapLog.init();

                //On.Player.SpitUpCraftedObject += Player_SpitUpCraftedObject1;
                //On.Player.GraspsCanBeCrafted += Player_GraspsCanBeCrafted;
                //On.Player.GrabUpdate += Player_GrabUpdate;

                MachineConnector.SetRegisteredOI("TheCollector", optionsMenuInstance = new TheCollectorOptionsMenu());

                if (ModManager.ActiveMods.Any(mod => mod.id == "dressmyslugcat"))
                {
                    SetupDMSSprites();
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Remix Menu: Hook_OnModsInit options failed init error {optionsMenuInstance}{ex}");
                Logger.LogError(ex);
                Logger.LogMessage("WHOOPS something go wrong"); 
            }
            finally
            {
                orig.Invoke(self);
            }
        }

        private void Player_GrabUpdate(On.Player.orig_GrabUpdate orig, Player self, bool eu)
        {
            orig(self, eu);

            if (self.SlugCatClass.value == "TheCollector" && self.input[0].pckp && self.GraspsCanBeCrafted())
            {
                self.craftingObject = true;
                if (self.craftingObject)
                {
                    self.swallowAndRegurgitateCounter++;
                    if(self.swallowAndRegurgitateCounter > 105)
                    {
                        if (self.spearOnBack != null)
                        {
                            self.spearOnBack.increment = false;
                            self.spearOnBack.interactionLocked = true;

                            self.SpitUpCraftedObject();
                            self.swallowAndRegurgitateCounter = 0;
                        }
                    }
                }
            }
        }

        private bool Player_GraspsCanBeCrafted(On.Player.orig_GraspsCanBeCrafted orig, Player self)
        {

            if (self.SlugCatClass.value == "TheCollector")
            {
                return true;
            }
            return orig(self);
        }

        private void Player_SpitUpCraftedObject1(On.Player.orig_SpitUpCraftedObject orig, Player self)
        {
            var room = self.room;
            bool flag = false;
            float hue = 0f;

            if (self.SlugCatClass.value == "TheCollector" && self.FoodInStomach > 0)
            {
                hue = 0.5f;

                for (int i = 0; i < self.grasps.Length; i++)
                {
                    AbstractPhysicalObject abstractPhysicalObject = self.grasps[i].grabbed.abstractPhysicalObject;
                    if ((abstractPhysicalObject.type == AbstractPhysicalObject.AbstractObjectType.Spear) && self.grasps[i] is not null && self.grasps[i].grabbed is Spear spear && !spear.bugSpear)
                    {
                        self.ReleaseGrasp(i);
                        abstractPhysicalObject.realizedObject.RemoveFromRoom();
                        room.abstractRoom.RemoveEntity(abstractPhysicalObject);

                        self.SubtractFood(1);

                        AbstractSpear abstractSpear = new AbstractSpear(self.room.world, null, self.coord, self.room.game.GetNewID(), false, flag);
                        abstractSpear.hue = hue;
                        room.abstractRoom.AddEntity(abstractSpear);
                        abstractSpear.RealizeInRoom();

                        if (self.FreeHand() != -1)
                        {
                            self.SlugcatGrab(abstractSpear.realizedObject, self.FreeHand());
                        }
                    }
                }
            }
            return;
        }

        public void SetupDMSSprites()
        {
            //Glape Gills??????? Whiskers?!?!?!?!?!
            //SpriteDefinitions.AddSprite(new AvailableSprite
            //{
            //    Name = "WHISKERS",
            //    Description = "Whiskers",
            //    GallerySprite = "LizardScaleA0",
            //    RequiredSprites = new List<string> { "LizardScaleA0" },
            //    Slugcats = new List<string> { "TheCollector" }
            //});

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
}