using Terraria.ModLoader;

namespace BingoBoardCore.Icons {
    internal class VanillaIconRegistry : ModSystem {
        public override void Load() {
            VanillaIcons.Achievement.registerItems();
            VanillaIcons.Bestiary.registerItems();
            var star = new VanillaIcons.Star(true);
            VanillaIcons.Star.On = star.Item;
            Mod.AddContent(star);
        }
    }
}
