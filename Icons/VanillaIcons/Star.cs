using Terraria;
using Terraria.ModLoader;

namespace BingoBoardCore.Icons {
    public partial class VanillaIcons {
        public class Star : ModItem {
            private int mode;
            public override string Texture => mode == 2
                ? "Terraria/Images/UI/Bestiary/Icon_Rank_Light"
                : "Terraria/Images/UI/Bestiary/Icon_Rank_Dim";
            public static Item Off { get; internal set; } = null!;
            public static Item On { get; internal set; } = null!;
            static readonly string[] names = ["unloaded", "Off", "On"];
            public override string Name => base.Name + names[mode];
            public Star() {
                mode = 0;
            }
            public override bool IsLoadingEnabled(Mod mod) => mode != 0;
            public Star(bool on) {
                this.mode = on ? 2 : 1;
            }

            public static void registerItems() {
                var mod = ModContent.GetInstance<BingoBoardCore>();
                Star off = new(false);
                mod.AddContent(off);
                Off = off.Item;
                Star on = new(true);
                mod.AddContent(on);
                On = on.Item;
            }
        }
    }
}
