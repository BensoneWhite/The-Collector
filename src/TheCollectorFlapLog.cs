﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheCollector
{
    internal class TheCollectorFlapLog
    {
        public static ConditionalWeakTable<Player, TheCollectorEX> ExtraJumpdata = new();
        public static bool LimitSpeed = true;
        public static float speed;

        public static void init()
        {
            On.Player.ctor += Player_ctor;
            On.Player.UpdateMSC += Player_UpdateMSC;
        }

        private static void Player_UpdateMSC(On.Player.orig_UpdateMSC orig, Player self)
        {
            orig(self);

            var room = self.room;
            var input = self.input;
            var jumps = Mathf.Max(1, 3);

            const float normalGravity = 0.9f;
            const float normalAirFriction = 0.999f;
            const float flightGravity = 0.12f;
            const float flightAirFriction = 0.7f;
            const float flightKickinDuration = 6f;


            if (!ExtraJumpdata.TryGetValue(self, out var player) || !player.isCollector)
            {
                return;
            }

            if (!self.dead && self.slugcatStats.name.value == "TheCollector" && self is not null && self.room is not null)
            {
                if (player.JumpCollectorLock > 0)
                {
                    player.JumpCollectorLock--;
                }

                bool flag2 = self.eatMeat >= 20 || self.maulTimer >= 15;
                bool jumper = self.wantToJump > 0;
                bool WasJumped = false;

                if (player.JumpCollectorCount > 0 && (self.Consious || self.dead))
                {
                    player.JumpCollectorCooldown -= 1;
                    if (player.JumpCollectorCooldown <= 0f)
                    {
                        if (player.JumpCollectorCount >= jumps)
                        {
                            player.JumpCollectorCooldown = 40f;
                        }
                        else
                        {
                            player.JumpCollectorCooldown = 60f;
                        }
                        player.JumpCollectorCount--;
                    }
                }
                if (jumper && player.Jumptimer <= 0 && !player.CollectorJumped && self.canJump <= 0 && !flag2 && (self.input[0].y >= 0 || (self.input[0].y < 0 && (self.bodyMode != Player.BodyModeIndex.ZeroG || self.gravity <= 0.1f))) && self.Consious && self.bodyMode != Player.BodyModeIndex.Crawl && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.animation != Player.AnimationIndex.HangFromBeam && self.animation != Player.AnimationIndex.ClimbOnBeam && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab && self.onBack == null)
                {
                    player.CollectorJumped = true;
                    WasJumped = true;
                    player.JumpCollectorLock = 40;
                    player.NoGrabCollector = 5;
                    Vector2 pos = self.firstChunk.pos;
                    self.animation = Player.AnimationIndex.RocketJump;

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
                }

                if (!player.isSliding && (self.canJump > 0 || !self.Consious || self.Stunned || self.animation == Player.AnimationIndex.HangFromBeam || self.animation == Player.AnimationIndex.ClimbOnBeam || self.bodyMode == Player.BodyModeIndex.WallClimb || self.animation == Player.AnimationIndex.AntlerClimb || self.animation == Player.AnimationIndex.VineGrab || self.animation == Player.AnimationIndex.ZeroGPoleGrab || self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG))
                {
                    player.CollectorJumped = false;
                }

                if ((self.wantToJump > 0 || WasJumped) && !(player.Jumptimer > 0))
                {
                    float JumpDelay = 0.35f;
                    player.Jumptimer = (int)(JumpDelay * 40f);
                }

                if (player.Jumptimer > 0 && player.Jumptimer != 0)
                {
                    player.Jumptimer--;
                }

                if(player.Jumptimer == 0)
                {
                    WasJumped = false;
                }

                //----------------------------------------------------------------------------------------------------------------------------
                //Glide
                //----------------------------------------------------------------------------------------------------------------------------

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
                    if (player.isSliding)
                    {

                        player.windSound.Volume = Mathf.Lerp(0f, 0.4f, player.slideDuration / flightKickinDuration);

                        player.slideDuration++;

                        self.AerobicIncrease(0.0001f);

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
                        if (self.wantToJump > 0 && player.SlideStamina > player.MinimumSlideStamina && player.CanSustainFlight() && player.CollectorJumped)
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

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            ExtraJumpdata.Add(self, new TheCollectorEX(self));
        }
    }
}
