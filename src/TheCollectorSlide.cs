using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheCollector
{
    internal class TheCollectorSlideLog
    {
        public static ConditionalWeakTable<Player, TheCollectorEX> ExtraJumpdata = new();


        public static void Init()
        {
            On.Player.ctor += Player_ctor;
            
        }

        private static void Player_ctor(On.Player.orig_ctor orig, Player self, AbstractCreature abstractCreature, World world)
        {
            orig(self, abstractCreature, world);

            ExtraJumpdata.Add(self, new TheCollectorEX(self));
        }
    }
}
