using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheCollector
{
    public static class TheCollectorEnums
    {
        public static SoundID porl { get; set; }
        public static SoundID flap { get; set; }
        public static SoundID wind { get; set; }
        public static void RegisterValues()
        {
            flap = new SoundID("flap", true);
            wind = new SoundID("wind", true);
            porl = new SoundID("porl", true);

        }

        public static void UnregisterValues()
        {
            Unregister(flap);
            Unregister(wind);
            Unregister(porl);

        }

        private static void Unregister<T>(ExtEnum<T> extEnum) where T : ExtEnum<T>
        {
            extEnum?.Unregister();
        }
    }
}

