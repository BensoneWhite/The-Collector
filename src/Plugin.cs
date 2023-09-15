using BepInEx;
using MonoMod;
using Newtonsoft.Json.Linq;
using RWCustom;
using static SlugBase.Features.FeatureTypes;
using SlugBase.Features;
using SlugBase;
using System;
using UnityEngine;
using static MonoMod.InlineRT.MonoModRule;
using static Player;
using System.Collections.Generic;
using System.Linq;
using DressMySlugcat;
using System.Security.Permissions;
using UnityEngine.UI;
using On.HUD;
using static Conversation;

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
                On.Player.UpdateBodyMode += BellySlide;
                On.Player.Jump += Movement;
                On.Player.CanBeSwallowed += Player_CanBeSwallowed;
                On.Player.UpdateBodyMode += Player_UpdateBodyMode;
                On.Player.ThrownSpear += Player_ThrownSpear;
                On.Player.CanEatMeat += Player_CanEatMeat;
                On.Player.WallJump += Player_WallJump;
                CollarCollector.Init();
                CollarCWT.OnInit();

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

        private void Player_WallJump(On.Player.orig_WallJump orig, Player self, int direction)
        {
            orig(self, direction);
            float num = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

            if (self.slugcatStats.name.value == "TheCollector")
            {
                self.bodyChunks[0].vel.y = 10f * num;
                self.bodyChunks[1].vel.y = 9f * num;
                self.bodyChunks[0].vel.x = 8f * num * (float)direction;
                self.bodyChunks[1].vel.x = 6f * num * (float)direction;
            }
        }

        private bool Player_CanEatMeat(On.Player.orig_CanEatMeat orig, Player self, Creature crit)
        {
            orig(self, crit);
            if (self.slugcatStats.name.value == "TheCollector" && crit.dead && crit is not Player && (!self.isNPC || !self.isSlugpup) && (self.room.game.IsStorySession || self.room.game.IsArenaSession))
            {
                return true;
            }
            else
            {
                return orig(self, crit);
            }
        }

        private void Player_ThrownSpear(On.Player.orig_ThrownSpear orig, Player self, Spear spear)
        {
            orig(self, spear);
            if (self.slugcatStats.name.value == "TheCollector")
            {
                switch (TheCollectorOptionsMenu.Damage.Value)
                {
                    case "1. Monk":
                        {
                            break;
                        }
                    case "2. Survivor":
                        {
                            spear.spearDamageBonus = UnityEngine.Random.Range(1f, 1.1f);
                            break;
                        }
                    case "3. Hunter":
                        {
                            spear.spearDamageBonus = UnityEngine.Random.Range(1.1f, 1.2f);
                            break;
                        }
                    default:
                        break;
                }
            }

            if (self.slugcatStats.name.value == "TheCollector" && self.room.game.IsArenaSession)
            {
                spear.spearDamageBonus = UnityEngine.Random.Range(0.6f, 1.2f);
            }
        }

        private void Player_UpdateBodyMode(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            orig(self);
            var power = 1.2f;
            if (self.slugcatStats.name.value == "TheCollector" && !self.standing && (self.bodyMode == Player.BodyModeIndex.Default || self.bodyMode == Player.BodyModeIndex.Crawl))
            {
                self.dynamicRunSpeed[0] += power;
                self.dynamicRunSpeed[1] += power;
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
                        new CustomSprite() { Sprite = "FACE", SpriteSheetID = sheetID, Color = Color.white }
                    },

                    CustomTail = new CustomTail()
                    {
                        Length = 8,
                        Wideness = 3.4f,
                        Roundness = 1.5f
                    }
                });
            }
        }

        private bool Player_CanBeSwallowed(On.Player.orig_CanBeSwallowed orig, Player self, PhysicalObject testObj)
        {
            if (self.slugcatStats.name.value == "TheCollector" && testObj is DataPearl)
            {
                return false;
            }
            return orig(self, testObj);
        }

        private void Movement(On.Player.orig_Jump orig, Player self)
        {
            orig(self);

            float jumpboost = Mathf.Lerp(1f, 1.15f, self.Adrenaline);

            if (self.slugcatStats.name.value == "TheCollector")
            {
                self.jumpBoost += 0.70f + jumpboost;
            }

            if (self.animation == Player.AnimationIndex.Flip && self.slugcatStats.name.value == "TheCollector")
            {

                self.bodyChunks[0].vel.y = 10f * jumpboost;
                self.bodyChunks[1].vel.y = 9f * jumpboost;


                self.bodyChunks[0].vel.x = 5f * (float)self.flipDirection * jumpboost;
                self.bodyChunks[1].vel.x = 4f * (float)self.flipDirection * jumpboost;
            }

            if (self.animation == Player.AnimationIndex.RocketJump && self.slugcatStats.name.value == "TheCollector")
            {

                self.bodyChunks[0].vel.y += 4f + jumpboost;
                self.bodyChunks[1].vel.y += 3f + jumpboost;

                self.bodyChunks[0].vel.x += 3f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
                self.bodyChunks[1].vel.x += 2f * self.bodyChunks[0].vel.x < 1 ? 0 : Mathf.Sign(self.bodyChunks[0].vel.x);
            }
        }

        private void BellySlide(On.Player.orig_UpdateBodyMode orig, Player self)
        {
            orig(self);

            if (self.animation == AnimationIndex.BellySlide && self.slugcatStats.name.value == "TheCollector")
            {

                float vector1 = 10f;
                float vector2 = 15f;

                self.bodyChunks[0].vel.x += (self.longBellySlide ? vector1 : vector2) * (float)self.rollDirection * Mathf.Sin((float)self.rollCounter / (self.longBellySlide ? 29f : 15f) * (float)Math.PI);
                if (self.IsTileSolid(0, 0, -1) || self.IsTileSolid(0, 0, -2))
                {
                    self.bodyChunks[0].vel.y -= 2.3f;
                }
            }
        }
    }
    internal static class Extras
    {
        private static bool _initialized;
        public static On.RainWorld.hook_OnModsInit WrapInit(Action<RainWorld> loadResources)
        {
            return (orig, self) =>
            {
                orig(self);

                try
                {
                    if (!_initialized)
                    {
                        _initialized = true;
                        loadResources(self);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            };
        }
    }
}