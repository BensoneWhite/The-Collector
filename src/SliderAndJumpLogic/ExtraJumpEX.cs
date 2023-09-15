using SlugBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheCollector
{
    public class ExtraJumpEX
    {
        public int JumpCollectorLock;
        public int cooldownAlone;
        public int JumpCollectorCount;
        public int NoGrabCollector;
        public int Jumptimer;

        public float JumpCollectorCooldown;

        public bool CollectorJumped;
        public bool Jumping;
        
        public readonly bool Collector;
        public readonly bool isCollector;
        public readonly SlugcatStats.Name Name;

        public WeakReference<Player> collectorRef;

        public ExtraJumpEX(Player player)
        {
            isCollector =
                Plugin.Collector.TryGet(player, out Collector);

            collectorRef = new WeakReference<Player>(player);

            if (ExtEnumBase.TryParse(typeof(SlugcatStats.Name), "TheCollector", true, out var extEnum))
            {
                Name = extEnum as SlugcatStats.Name;
            }
        }
    }
}
