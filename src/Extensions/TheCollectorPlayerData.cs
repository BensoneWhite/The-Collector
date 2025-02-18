using UnityEngine;

namespace TheCollector;

public class TheCollectorPlayerData
{
    public float SlideRecovery => UnlockedExtraStamina ? SlideStaminaRecoveryBase * 1.2f : SlideStaminaRecoveryBase;
    public float MinimumSlideStamina => SlideStaminaMax * 0.1f;

    public readonly float SlideStaminaRecoveryBase;
    public float SlideStamina;
    public float SlideSpeed;

    public int SlideStaminaMax => UnlockedExtraStamina ? (int)(SlideStaminaMaxBase * 1.6f) : SlideStaminaMaxBase;

    public readonly int SlideStaminaMaxBase;
    public int slideStaminaRecoveryCooldown;
    public int slideDuration;
    public int timeSinceLastSlide;
    public int preventSlide;
    public int preventGrabs;

    public bool CanSlide => SlideStaminaMax > 0 && SlideSpeed > 0;

    public readonly bool Collector;

    public readonly bool IsCollector;
    public readonly Player player;
    public readonly WeakReference<Player> PlayerRef;

    public bool isSliding;
    public bool UnlockedExtraStamina = false;
    public bool UnlockedVerticalFlight = false;

    public DynamicSoundLoop windSound;

    public int JumpCollectorLock;
    public int cooldownAlone;
    public int JumpCollectorCount;
    public int NoGrabCollector;
    public int Jumptimer;

    public bool CollectorJumped;
    public bool Jumping;

    public TheCollectorPlayerData(Player player)
    {
        IsCollector = player.slugcatStats.name == TCEnums.TheCollector;
        this.player = player;
        PlayerRef = new WeakReference<Player>(player);

        if(!IsCollector)
        {
            return;
        }

        SetupSounds(player);

        SlideStamina = SlideStaminaMax;
        timeSinceLastSlide = 200;
    }

    private void SetupSounds(Player player)
    {
        windSound = new ChunkDynamicSoundLoop(player.bodyChunks[0])
        {
            sound = TCEnums.Sound.wind,
            Pitch = 1f,
            Volume = 1f
        };
    }

    public void StopSliding()
    {
        slideDuration = 0;
        timeSinceLastSlide = 0;
        isSliding = false;
    }

    public void InitiateSlide()
    {
        if (!PlayerRef.TryGetTarget(out var player))
        {
            return;
        }
        player.bodyMode = Player.BodyModeIndex.Default;
        player.animation = Player.AnimationIndex.None;
        player.wantToJump = 0;
        slideDuration = 0;
        timeSinceLastSlide = 0;
        isSliding = true;
    }

    public bool CanSustainFlight()
    {
        if (!PlayerRef.TryGetTarget(out var player))
        {
            return false;
        }
        return SlideStamina > 0
            && preventSlide == 0
            && player.canJump <= 0
            && player.bodyMode != Player.BodyModeIndex.Crawl
            && player.bodyMode != Player.BodyModeIndex.CorridorClimb
            && player.bodyMode != Player.BodyModeIndex.ClimbIntoShortCut
            && player.animation != Player.AnimationIndex.HangFromBeam
            && player.animation != Player.AnimationIndex.ClimbOnBeam
            && player.bodyMode != Player.BodyModeIndex.WallClimb
            && player.bodyMode != Player.BodyModeIndex.Swimming
            && player.Consious
            && !player.Stunned
            && player.animation != Player.AnimationIndex.AntlerClimb
            && player.animation != Player.AnimationIndex.VineGrab
            && player.animation != Player.AnimationIndex.ZeroGPoleGrab;
    }
}