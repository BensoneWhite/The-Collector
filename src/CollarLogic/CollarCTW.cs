using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RWCustom;
using UnityEngine;
using TheCollector;

namespace TheCollector
{
    public static class CollarCWT
    {
        public class Disdain
        {
            public PorlCollection porlztorage;

            public class AbstractPorls : AbstractPhysicalObject.AbstractObjectStick
            {
                public AbstractPhysicalObject Player
                {
                    get { return A; }
                    set { A = value; }
                }

                public AbstractPhysicalObject DataPearl
                {
                    get { return B; }
                    set { B = value; }
                }

                public AbstractPorls(AbstractPhysicalObject player, AbstractPhysicalObject pearl)
                    : base(player, pearl)
                {
                }
            }

            public class PorlCollection
            {
                public Player Owner;  // The owner! ALL HAIL THE OWNER
                public Stack<DataPearl> deezNuts;  // Storage (no dics here!)
                public Stack<AbstractPorls> abstractNutz;  // Abstract stick links storage stuff whatever magic
                public int capacity = 4;  // Number of porls scug can store
                public bool increment;  // Allow interaction
                private int counter;  // Animation frame counter
                private bool locked;  // For preventing the pearls from being interacted with while the take in/out process occurs or the player is at an invalid state to do so

                public PorlCollection(Player owner)
                {
                    deezNuts ??= new Stack<DataPearl>(capacity);
                    abstractNutz ??= new Stack<AbstractPorls>(capacity);
                    Owner = owner;
                }

                public void GraphicsModuleUpdated(bool actuallyViewed, bool eu)
                {
                    if (deezNuts.Count > 0)  // If there's a nut to draw/animate
                    {
                        PlayerGraphics playerGraphics = (PlayerGraphics)Owner.graphicsModule;
                        Vector2 vector1 = playerGraphics.drawPositions[0, 0];
                        Vector2 vector2 = playerGraphics.drawPositions[1, 0];
                        Vector2 pos = playerGraphics.head.pos;
                        Vector2 vector3 = new Vector2(12f, 0f);
                        Vector2 vector4 = new Vector2(-12f, -14f);
                        Vector2 vector5 = vector1 - vector2;
                        Vector2 normalized = vector5.normalized;
                        vector3 = Custom.RotateAroundOrigo(vector3, Custom.VecToDeg(normalized));
                        vector4 = Custom.RotateAroundOrigo(vector4, Custom.VecToDeg(normalized));
                        vector3 = vector1 + vector3;
                        vector4 = vector1 + vector4;
                        Vector2 vector6 = vector4 - vector3;
                        int index = 0;
                        foreach (DataPearl nut in deezNuts)
                        {
                            float porls = ((float)index + 1f) / ((float)deezNuts.Count + 1f);
                            Vector2 moveTo = vector3 + vector6 * porls;
                            nut.firstChunk.MoveFromOutsideMyUpdate(eu, moveTo);
                            nut.firstChunk.vel = Owner.mainBodyChunk.vel;
                            index++;
                        }
                    }
                }

                public void Update(bool evenUpdates, Player self)
                {
                    if (!SlideData.TryGetValue(self, out var player) || !player.isCollector)
                    {

                        return;
                    }

                    if (increment)
                    {
                        counter++;
                        // Move porl from paw to store
                        if (counter > 20 && deezNuts.Count < capacity)
                        {
                            // Move porl from any hand to store if store is empty
                            if (deezNuts.Count == 0)
                            {
                                for (int index = 0; index < Owner.grasps.Length; index++)
                                {
                                    if (Owner.grasps[index]?.grabbed is DataPearl nut)
                                    {
                                        CanYouUnderstandWithoutLookingThisUp(nut, self, index);
                                        counter = 0;
                                        break;
                                    }
                                }
                            }

                            // Move porl from main paw to store if there's something in the store
                            else if (Owner.grasps[0]?.grabbed is DataPearl nut)
                            {
                                CanYouUnderstandWithoutLookingThisUp(nut, self);
                                counter = 0;
                            }
                        }

                        // Move porl from store to paw
                        if (counter > 20 && deezNuts.Count > 0)
                        {
                            GoogleTranslateDidntWork(evenUpdates, self);
                            counter = 0;
                        }
                    }
                    else
                    {
                        counter = 0;
                    }

                    if (!Owner.input[0].pckp)
                    {
                        locked = false;
                    }
                    increment = false;
                }

                ///<summary>
                /// This takes the dataporl object as input, then attempts to store it into the (presumably) initialized porl storage.
                ///</summary>
                private void CanYouUnderstandWithoutLookingThisUp(DataPearl DataUnfold, Player self, int Foot = 0)
                {
                    if (!SlideData.TryGetValue(self, out var player) || !player.isCollector)
                    {

                        return;
                    }

                    if (deezNuts.Count >= capacity)
                    {
                        return;
                    }

                    // Liberate the porl from one of the paws that's grabbing onto a datapearl
                    if (!player.isSliding)
                    {
                        Owner.ReleaseGrasp(Foot);
                        DataUnfold.CollideWithObjects = false;
                        deezNuts.Push(DataUnfold);
                        locked = true;
                        Owner.noPickUpOnRelease = 20;
                        Owner.room?.PlaySound(TheCollectorEnums.porl, Owner.mainBodyChunk);
                        abstractNutz.Push(new AbstractPorls(Owner.abstractPhysicalObject, DataUnfold.abstractPhysicalObject));
                        Debug.LogWarning("Moved porl from paw to strage! Storage index is now: " + deezNuts.Count);
                    }
                }

                ///<summary>
                /// This attempts to grab the latest porl stored in the storage and places it in an empty hand. If the hands are all full, do nothing.
                ///</summary>
                private void GoogleTranslateDidntWork(bool Cause, Player self)
                {

                    if (!SlideData.TryGetValue(self, out var player) || !player.isCollector)
                    {
                        return;
                    }

                    // Check for paw availability (if the player is already holding something)
                    for (int index = 0; index < Owner.grasps.Length; index++)
                    {
                        if (Owner.grasps[index] is not null && Owner.Grabability(Owner.grasps[index].grabbed) >= Player.ObjectGrabability.TwoHands)
                        {
                            return;  // If there is no free spot
                        }
                    }

                    int toPaw = Owner.FreeHand(); // Find free paw, value is -1 if free paw is not found.

                    if (toPaw == -1)
                    {
                        return;
                    }

                    if (abstractNutz is not null && deezNuts is not null && !player.isSliding)
                    {
                        if (toPaw != -1)
                        {  // If free paw is detected
                            DataPearl nut = deezNuts.Pop();
                            AbstractPorls aNut = abstractNutz.Pop();
                            if (Owner.graphicsModule is not null && aNut is not null && nut is not null)
                            {
                                nut.firstChunk.MoveFromOutsideMyUpdate(Cause, (Owner.graphicsModule as PlayerGraphics).hands[toPaw].pos);
                            }
                            aNut?.Deactivate();  // Deactivate abstract porl object if it's live.
                            nut.CollideWithObjects = true;
                            Owner.SlugcatGrab(nut, toPaw);
                            locked = true;
                            Owner.ReleaseGrasp(0);
                            Owner.noPickUpOnRelease = 20;
                            Owner.room?.PlaySound(TheCollectorEnums.porl, Owner.mainBodyChunk);
                            Debug.LogWarning("Successfully moved porl from storage to paw! Storage index is now: " + deezNuts.Count);
                        }
                        else
                        {  // Paws are both full, shit the fuck.
                            Debug.LogWarning("Could not move porl from storage to paw! Storage index is now: " + deezNuts.Count);
                        }
                    }
                }
            }

            public Disdain(Player player)
            {
                porlztorage = new PorlCollection(player);
            }
        }
        private static readonly ConditionalWeakTable<Player, Disdain> CWT = new();
        public static Disdain Yippee(this Player player) => CWT.GetValue(player, _ => new(player));
        public static ConditionalWeakTable<Player, TheCollectorEX> SlideData = new();
        public static ConditionalWeakTable<Player, ExtraJumpEX> ExtraJumpdata = new();
        public static bool LimitSpeed = true;
        public static bool JumpCheck = true;
        public static bool JumperDumper = false;
        public static float speed;

        public static void OnInit()
        {
            On.Player.ctor += Player_ctor;
            On.Player.Grabability += Player_Grabability;
            On.Player.UpdateMSC += Player_UpdateMSC;
            On.Player.Update += Player_Update;
        }

        private static void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            var room = self.room;
            var input = self.input;
            var jumps = Mathf.Max(1, 200);

            if (!ExtraJumpdata.TryGetValue(self, out var player) || !player.isCollector)
            {
                return;
            }
            if (!SlideData.TryGetValue(self, out var slide) || !player.isCollector)
            {
                return;
            }

            if (!self.dead && self.slugcatStats.name.value == "TheCollector")
            {
                if (player.JumpCollectorLock > 0)
                {
                    player.JumpCollectorLock--;
                }
                bool jumper = self.wantToJump > 0 && (self.input[0].pckp || self.input[0].jmp);
                if (player.JumpCollectorCount > 0 && (self.Consious || self.dead))
                {
                    //-1 jump :-1:
                    player.JumpCollectorCooldown -= 1;

                    //Jump cooldown
                    if (player.JumpCollectorCooldown <= 0f)
                    {
                        //Big cooldown
                        if (player.JumpCollectorCount >= jumps)
                        {
                            player.JumpCollectorCooldown = 40f;
                        }
                        //Biger cooldown
                        else
                        {
                            player.JumpCollectorCooldown = 60f;
                        }
                        //Regenerate jumps
                        player.JumpCollectorCount--;
                    }
                }
                player.JumpCollectorCooldown -= 1f;
                if (jumper && !player.CollectorJumped && self.canJump <= 0 && (self.input[0].y >= 0 || (self.input[0].y < 0 && (self.bodyMode != Player.BodyModeIndex.ZeroG || self.gravity <= 0.1f))) && self.Consious && self.bodyMode != Player.BodyModeIndex.Crawl && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.animation != Player.AnimationIndex.HangFromBeam && self.animation != Player.AnimationIndex.ClimbOnBeam && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab && self.onBack == null)
                {
                    player.CollectorJumped = true;
                    JumperDumper = true;
                    player.JumpCollectorLock = 40;
                    player.NoGrabCollector = 5;
                    Vector2 pos = self.firstChunk.pos;

                    room.PlaySound(TheCollectorEnums.flap, pos);
                    if (self.bodyMode == Player.BodyModeIndex.ZeroG || room.gravity == 0f || self.gravity == 0f)
                    {
                        float inputx = self.input[0].x;
                        float inputy = self.input[0].y;

                        while (inputx == 0f && inputy == 0f)
                        {
                            inputx = ((!((double)UnityEngine.Random.value <= 0.33)) ? (((double)UnityEngine.Random.value <= 0.5) ? 1 : (-1)) : 0);
                            inputy = ((!((double)UnityEngine.Random.value <= 0.33)) ? (((double)UnityEngine.Random.value <= 0.5) ? 1 : (-1)) : 0);
                        }
                        self.bodyChunks[0].vel.x = 9f * inputx;
                        self.bodyChunks[0].vel.y = 9f * inputy;
                        self.bodyChunks[1].vel.x = 8f * inputx;
                        self.bodyChunks[1].vel.y = 8f * inputy;
                        player.JumpCollectorCooldown = 150f;
                        player.JumpCollectorCount++;
                        self.animation = Player.AnimationIndex.RocketJump;
                    }
                    else
                    {
                        if (input[0].x != 0)
                        {
                            self.bodyChunks[0].vel.y = Mathf.Min(self.bodyChunks[0].vel.y, 0f) + 8f;
                            self.bodyChunks[1].vel.y = Mathf.Min(self.bodyChunks[1].vel.y, 0f) + 7f;
                            self.jumpBoost = 6f;
                        }

                        if (input[0].x == 0 || input[0].y == 1)
                        {
                            if (player.JumpCollectorCount >= jumps)
                            {
                                self.bodyChunks[0].vel.y = 16f;
                                self.bodyChunks[1].vel.y = 15f;
                                self.jumpBoost = 10f;
                            }
                            else
                            {
                                self.bodyChunks[0].vel.y = 11f;
                                self.bodyChunks[1].vel.y = 10f;
                                self.jumpBoost = 8f;
                            }
                        }

                        if (input[0].y == 1)
                        {
                            self.bodyChunks[0].vel.x = 10f * (float)input[0].x;
                            self.bodyChunks[1].vel.x = 8f * (float)input[0].x;
                        }
                        else
                        {
                            self.bodyChunks[0].vel.x = 15f * (float)input[0].x;
                            self.bodyChunks[1].vel.x = 13f * (float)input[0].x;
                        }

                        player.JumpCollectorCount++;
                        player.JumpCollectorCooldown = 150f;
                    }
                    if (player.JumpCollectorCount >= jumps)
                    {   
                        self.Stun(60 * (player.JumpCollectorCount - (jumps - 1)));
                    }
                }
                if (player.CollectorJumped && player.Jumping)
                {
                    slide.StopSliding();
                    player.Jumping = false;
                    JumpCheck = false;
                    float JumpDelay = 2;
                    player.Jumptimer = (int)(JumpDelay * 40f);
                }
                if (player.Jumptimer <= 0 && !player.Jumping)
                {
                    player.Jumping = true;
                    player.CollectorJumped = false;
                }
                if (player.Jumptimer > 0 && player.Jumptimer != 0)
                {
                    player.Jumptimer--;
                }
                if (!self.input[0].jmp || !slide.CanSustainFlight())
                {
                    slide.StopSliding();
                    slide.isSliding = false;
                }
            }
        }
        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);
            SlideData.Add(self, new TheCollectorEX(self));

            ExtraJumpdata.Add(self, new ExtraJumpEX(self));
        }

        private static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
        {
            orig(self);
            if (!SlideData.TryGetValue(self, out var player) || !player.isCollector)
            {
                return;
            }
            if (!ExtraJumpdata.TryGetValue(self, out var jump) || !player.isCollector)
            {
                return;
            }
            const float normalGravity = 0.9f;
            const float normalAirFriction = 0.999f;
            const float flightGravity = 0.12f;
            const float flightAirFriction = 0.7f;
            const float flightKickinDuration = 6f;

            if (!self.dead && self.slugcatStats.name.value == "TheCollector")
            {
                if (player.CanSlide)
                {
                    if (self.animation == Player.AnimationIndex.HangFromBeam)
                    {
                        player.preventSlide = 15;
                    }
                    else if (player.preventSlide > 0)
                    {
                        player.preventSlide--;
                    }
                    if (!player.isSliding)
                    {
                        speed = 2f;
                        LimitSpeed = true;
                    }
                    if (player.isSliding && JumperDumper)
                    {

                        player.windSound.Volume = Mathf.Lerp(0f, 0.4f, player.slideDuration / flightKickinDuration);

                        player.slideDuration++;

                        self.AerobicIncrease(0.08f);

                        self.gravity = Mathf.Lerp(normalGravity, flightGravity, player.slideDuration / flightKickinDuration);
                        self.airFriction = Mathf.Lerp(normalAirFriction, flightAirFriction, player.slideDuration / flightKickinDuration);

                        if (LimitSpeed)
                        {
                            speed = RWCustom.Custom.LerpAndTick(speed, 10f, 0.001f, 0.3f);

                            if (speed >= 10f)
                            {
                                LimitSpeed = false;
                            }
                            if (self.input[0].x > 0)
                            {
                                self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x + speed;
                                self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x - 1f;
                            }
                            else if (self.input[0].x < 0)
                            {
                                self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x - speed;
                                self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x + 1f;
                            }
                            if (self.room.gravity <= 0.5)
                            {
                                if (self.input[0].y > 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 1f;
                                }
                                else if (self.input[0].y < 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 1f;
                                }
                            }
                            else if (player.UnlockedVerticalFlight)
                            {
                                if (self.input[0].y > 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed * 0.5f;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 0.3f;
                                }
                                else if (self.input[0].y < 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 0.3f;
                                }
                            }
                        }
                        if (!LimitSpeed)
                        {
                            speed = RWCustom.Custom.LerpAndTick(speed, 0f, 0.005f, 0.003f);

                            if (speed == 0f)
                            {
                                LimitSpeed = true;
                            }
                            if (self.input[0].x > 0)
                            {
                                self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x + speed;
                                self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x - 1f;
                            }
                            else if (self.input[0].x < 0)
                            {
                                self.bodyChunks[0].vel.x = self.bodyChunks[0].vel.x - speed;
                                self.bodyChunks[1].vel.x = self.bodyChunks[1].vel.x + 1f;
                            }
                            if (self.room.gravity <= 0.5)
                            {
                                if (self.input[0].y > 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 1f;
                                }
                                else if (self.input[0].y < 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 1f;
                                }
                            }
                            else if (player.UnlockedVerticalFlight)
                            {
                                if (self.input[0].y > 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y + speed * 0.5f;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y - 0.3f;
                                }
                                else if (self.input[0].y < 0)
                                {
                                    self.bodyChunks[0].vel.y = self.bodyChunks[0].vel.y - speed;
                                    self.bodyChunks[1].vel.y = self.bodyChunks[1].vel.y + 0.3f;
                                }
                            }
                            if (speed <= 1.2f)
                            {
                                player.StopSliding();
                                JumperDumper = false;
                            }
                        }
                        player.slideStaminaRecoveryCooldown = 40;
                        player.SlideStamina--;
                        if (!self.input[0].jmp || !player.CanSustainFlight())
                        {
                            player.StopSliding();
                        }

                    }
                    else
                    {
                        player.windSound.Volume = Mathf.Lerp(1f, 0f, player.timeSinceLastSlide / flightKickinDuration);
                        player.timeSinceLastSlide++;
                        player.windSound.Volume = 0f;
                        if (player.slideStaminaRecoveryCooldown > 0)
                        {
                            player.slideStaminaRecoveryCooldown--;
                        }
                        else
                        {
                            player.SlideStamina = Mathf.Min(player.SlideStamina + player.SlideRecovery, player.SlideStaminaMax);
                        }
                        if (self.wantToJump > 0 && player.SlideStamina > player.MinimumSlideStamina && player.CanSustainFlight())
                        {
                            player.InitiateSlide();
                        }
                        self.airFriction = normalAirFriction;
                        self.gravity = normalGravity;
                    }
                }
                if (player.preventGrabs > 0)
                {
                    player.preventGrabs--;
                }
            }
            player.windSound.Update();
        }

        private static Player.ObjectGrabability Player_Grabability(On.Player.orig_Grabability orig, Player self, PhysicalObject obj)
        {
            if (SlideData.TryGetValue(self, out var player) && player.preventGrabs > 0)
            {
                return Player.ObjectGrabability.CantGrab;
            }

            return orig(self, obj);
        }
    }
}