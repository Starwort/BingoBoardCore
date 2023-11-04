using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria.ModLoader.Config;

namespace BingoBoardCore.Common.Configs {
    internal class BingoBoardUIConfig : ModConfig {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Increment(5)]
        [Range(5, 300)]
        [DefaultValue(30)]
        [Slider]
        public uint animationPeriod;

        [OnDeserialized]
        internal void fixAnimationPeriod(StreamingContext context) {
            if (animationPeriod == 0) {
                animationPeriod = 30;
            }
        }
    }
}
