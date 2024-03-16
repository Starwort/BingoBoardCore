using Terraria.ModLoader;

namespace BingoBoardCore.Icons {
    internal class VanillaIconRegistry : ModSystem {
        public override void Load() {
            VanillaIcons.Achievement.registerItems();
            VanillaIcons.Bestiary.registerItems();
        }
    }
}
