namespace TheCollector;

public class CustomAbilities
{
    public static bool LimitSpeed = true;
    public static float speed;

    public static void init()
    {
        On.Player.UpdateMSC += Glide;
        On.Player.UpdateMSC += Flap;
    }

    private static void Flap(On.Player.orig_UpdateMSC orig, Player self)
    {
        orig(self);

        if (!self.IsTheCollector(out var collector)) return;

        var room = self.room;
        var input = self.input;
        var jumps = 2;

        if (!self.dead && self != null && room != null)
        {
            if (collector.JumpCollectorLock > 0)
            {
                collector.JumpCollectorLock--;
            }

            bool inputXY = (self.input[0].y >= 0 || self.input[0].y < 0);
            bool jump = self.wantToJump > 0 && collector.Jumptimer <= 0 && !collector.CollectorJumped && self.canJump <= 0;
            bool maul = self.eatMeat >= 20 || self.maulTimer >= 15;
            bool WasJumped = false;
            bool animation = self.animation != Player.AnimationIndex.VineGrab && self.bodyMode != Player.BodyModeIndex.Crawl && self.bodyMode != Player.BodyModeIndex.CorridorClimb && self.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut && self.animation != Player.AnimationIndex.HangFromBeam && self.animation != Player.AnimationIndex.ClimbOnBeam && self.bodyMode != Player.BodyModeIndex.WallClimb && self.bodyMode != Player.BodyModeIndex.Swimming && self.animation != Player.AnimationIndex.AntlerClimb && self.animation != Player.AnimationIndex.VineGrab && self.animation != Player.AnimationIndex.ZeroGPoleGrab && self.animation != Player.AnimationIndex.HangFromBeam && self.bodyMode != Player.BodyModeIndex.ClimbingOnBeam && self.animation != Player.AnimationIndex.GetUpOnBeam && self.animation != Player.AnimationIndex.StandOnBeam;

            if (!WasJumped && jump && !maul && inputXY && self.Consious && animation && self.onBack == null)
            {
                collector.CollectorJumped = true;
                WasJumped = true;
                collector.JumpCollectorLock = 40;
                collector.NoGrabCollector = 5;
                Vector2 pos = self.firstChunk.pos;
                if (self.input[0].x != 0)
                {
                    self.animation = Player.AnimationIndex.RocketJump;
                }
                else
                {
                    for (int l = 0; l < 2; l++)
                    {
                        if (self.bodyChunks[l].ContactPoint.x != 0 || self.bodyChunks[l].ContactPoint.y != 0)
                        {
                            self.animation = Player.AnimationIndex.None;
                            self.bodyMode = Player.BodyModeIndex.Stand;
                            self.standing = self.bodyChunks[0].pos.y > self.bodyChunks[1].pos.y;
                            break;
                        }
                    }
                }

                if (self.bodyMode == Player.BodyModeIndex.ZeroG)
                {
                    self.standing = true;
                }

                room.PlaySound(TCEnums.Sound.flap, pos);


                if ((self.bodyMode == Player.BodyModeIndex.ZeroG || room.gravity == 0f || self.gravity == 0f) && collector.Jumptimer <= 0)
                {
                    float inputx = self.input[0].x;
                    float inputy = self.input[0].y;

                    while (inputx == 0f && inputy == 0f)
                    {
                        inputx = !((double)Random.value <= 0.33) ? (double)Random.value <= 0.5 ? 1 : -1 : 0;
                        inputy = !((double)Random.value <= 0.33) ? (double)Random.value <= 0.5 ? 1 : -1 : 0;
                    }
                    self.bodyChunks[0].vel.x = 9f * inputx;
                    self.bodyChunks[0].vel.y = 9f * inputy;
                    self.bodyChunks[1].vel.x = 8f * inputx;
                    self.bodyChunks[1].vel.y = 8f * inputy;
                    collector.JumpCollectorCount++;

                    float JumpDelay = 2f;
                    collector.Jumptimer = (int)(JumpDelay * 40f);
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
                        if (collector.JumpCollectorCount >= jumps)
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
                        self.bodyChunks[0].vel.x = 10f * input[0].x;
                        self.bodyChunks[1].vel.x = 8f * input[0].x;
                    }
                    else
                    {
                        self.bodyChunks[0].vel.x = 15f * input[0].x;
                        self.bodyChunks[1].vel.x = 13f * input[0].x;
                    }

                    collector.JumpCollectorCount++;
                }
            }

            if (!collector.isSliding && (self.canJump > 0 || !self.Consious || self.Stunned || self.animation == Player.AnimationIndex.HangFromBeam || self.animation == Player.AnimationIndex.ClimbOnBeam || self.bodyMode == Player.BodyModeIndex.WallClimb || self.animation == Player.AnimationIndex.AntlerClimb || self.animation == Player.AnimationIndex.VineGrab || self.animation == Player.AnimationIndex.ZeroGPoleGrab || self.bodyMode == Player.BodyModeIndex.Swimming || self.bodyMode == Player.BodyModeIndex.ZeroG))
            {
                collector.CollectorJumped = false;
            }

            if (self.bodyMode == Player.BodyModeIndex.WallClimb && collector.Jumptimer <= 0)
            {
                float JumpDelay = 0.25f;
                collector.Jumptimer = (int)(JumpDelay * 40f);
            }

            if (self.bodyMode == Player.BodyModeIndex.CorridorClimb && collector.Jumptimer <= 0)
            {
                float JumpDelay = 0.75f;
                collector.Jumptimer = (int)(JumpDelay * 40f);
            }

            if (collector.Jumptimer > 0 && collector.Jumptimer != 0)
            {
                collector.Jumptimer--;
            }

            if (collector.Jumptimer == 0)
            {
                WasJumped = false;
            }
        }
    }

    private static void Glide(On.Player.orig_UpdateMSC orig, Player self)
    {
        orig(self);

        if (!self.IsTheCollector(out var collector)) return;

        var room = self.room;

        const float normalGravity = 0.9f;
        const float normalAirFriction = 0.999f;
        const float flightGravity = 0.12f;
        const float flightAirFriction = 0.7f;
        const float flightKickinDuration = 6f;

        if (!self.dead && self is not null && room is not null)
        {
            if (collector.CanSlide)
            {
                if (self.animation == Player.AnimationIndex.HangFromBeam)
                {
                    collector.preventSlide = 15;
                }
                else if (collector.preventSlide > 0)
                {
                    collector.preventSlide--;
                }
                if (!collector.isSliding)
                {
                    speed = 2f;
                    LimitSpeed = true;
                }
                if (collector.isSliding)
                {

                    collector.windSound.Volume = Mathf.Lerp(0f, 0.4f, collector.slideDuration / flightKickinDuration);

                    collector.slideDuration++;

                    self.AerobicIncrease(0.0001f);

                    self.gravity = Mathf.Lerp(normalGravity, flightGravity, collector.slideDuration / flightKickinDuration);
                    self.airFriction = Mathf.Lerp(normalAirFriction, flightAirFriction, collector.slideDuration / flightKickinDuration);

                    if (LimitSpeed)
                    {
                        speed = Custom.LerpAndTick(speed, 10f, 0.001f, 0.3f);

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
                        else if (collector.UnlockedVerticalFlight)
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
                        speed = Custom.LerpAndTick(speed, 0f, 0.005f, 0.003f);

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
                        else if (collector.UnlockedVerticalFlight)
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
                            collector.StopSliding();
                        }
                    }
                    collector.slideStaminaRecoveryCooldown = 40;
                    collector.SlideStamina--;
                    if (!self.input[0].jmp || !collector.CanSustainFlight())
                    {
                        collector.StopSliding();
                    }

                }
                else
                {
                    collector.windSound.Volume = Mathf.Lerp(1f, 0f, collector.timeSinceLastSlide / flightKickinDuration);
                    collector.timeSinceLastSlide++;
                    collector.windSound.Volume = 0f;
                    if (collector.slideStaminaRecoveryCooldown > 0)
                    {
                        collector.slideStaminaRecoveryCooldown--;
                    }
                    else
                    {
                        collector.SlideStamina = Mathf.Min(collector.SlideStamina + collector.SlideRecovery, collector.SlideStaminaMax);
                    }
                    if (self.wantToJump > 0 && collector.SlideStamina > collector.MinimumSlideStamina && collector.CanSustainFlight() && collector.CollectorJumped)
                    {
                        collector.InitiateSlide();
                    }
                    self.airFriction = normalAirFriction;
                    self.gravity = normalGravity;
                }
            }
            if (collector.preventGrabs > 0)
            {
                collector.preventGrabs--;
            }
        }
        collector.windSound.Update();
    }
}