using Terraria;
using Terraria.ModLoader;

namespace BingoBoardCore.Icons {
    public partial class VanillaIcons {
        public class Star : ModItem {
            private bool on;
            public override string Texture => on
                ? "Terraria/Images/UI/Bestiary/Icon_Rank_Light"
                : "Terraria/Images/UI/Bestiary/Icon_Rank_Dark";
            public static Item Off { get; internal set; } = null!;
            public static Item On { get; internal set; } = null!;
            public Star() {
                on = false;
                Off = this.Item;
            }
            public Star(bool on) {
                this.on = on;
            }
        }
    }
}
