using BingoBoardCore.AnimationHelpers;
using Terraria;
using Terraria.ModLoader;

namespace BingoBoardCore.Icons {
    public partial class VanillaIcons {
        public class Bestiary : ModItem {
            public override string Texture => "Terraria/Images/UI/Bestiary/Icon_Tags_Shadow";
            public static Item Forest { get; private set; } = null!;
            public static Item Underground { get; private set; } = null!;
            public static Item Caverns { get; private set; } = null!;
            public static Item Desert { get; private set; } = null!;
            public static Item UndergroundDesert { get; private set; } = null!;
            public static Item Snow { get; private set; } = null!;
            public static Item UndergroundSnow { get; private set; } = null!;
            public static Item Corruption { get; private set; } = null!;
            public static Item UndergroundCorruption { get; private set; } = null!;
            public static Item CorruptDesert { get; private set; } = null!;
            public static Item UndergroundCorruptDesert { get; private set; } = null!;
            public static Item CorruptIce { get; private set; } = null!;
            public static Item Crimson { get; private set; } = null!;
            public static Item UndergroundCrimson { get; private set; } = null!;
            public static Item CrimsonDesert { get; private set; } = null!;
            public static Item UndergroundCrimsonDesert { get; private set; } = null!;
            public static Item CrimsonIce { get; private set; } = null!;
            public static Item Hallow { get; private set; } = null!;
            public static Item UndergroundHallow { get; private set; } = null!;
            public static Item HallowedDesert { get; private set; } = null!;
            public static Item UndergroundHallowedDesert { get; private set; } = null!;
            public static Item HallowedIce { get; private set; } = null!;
            public static Item Jungle { get; private set; } = null!;
            public static Item UndergroundJungle { get; private set; } = null!;
            public static Item SurfaceMushroom { get; private set; } = null!;
            public static Item UndergroundMushroom { get; private set; } = null!;
            public static Item SkyIsland { get; private set; } = null!;
            public static Item Oasis { get; private set; } = null!;
            public static Item Ocean { get; private set; } = null!;
            public static Item Marble { get; private set; } = null!;
            public static Item Granite { get; private set; } = null!;
            public static Item Temple { get; private set; } = null!;
            public static Item Dungeon { get; private set; } = null!;
            public static Item Underworld { get; private set; } = null!;
            public static Item Hell => Underworld;
            public static Item SpiderCave { get; private set; } = null!;
            public static Item Graveyard { get; private set; } = null!;
            public static Item Day { get; private set; } = null!;
            public static Item Night { get; private set; } = null!;
            public static Item BloodMoon { get; private set; } = null!;
            public static Item SolarEclipse { get; private set; } = null!;
            public static Item Rain { get; private set; } = null!;
            public static Item WindyDay { get; private set; } = null!;
            public static Item Blizzard { get; private set; } = null!;
            public static Item Snowstorm => Blizzard;
            public static Item Sandstorm { get; private set; } = null!;
            public static Item Meteor { get; private set; } = null!;
            public static Item Halloween { get; private set; } = null!;
            public static Item Pumpkin => Halloween;
            public static Item Christmas { get; private set; } = null!;
            public static Item Present => Christmas;
            public static Item SlimeRain { get; private set; } = null!;
            public static Item Party { get; private set; } = null!;
            public static Item GoblinArmy { get; private set; } = null!;
            public static Item PirateInvasion { get; private set; } = null!;
            public static Item PumpkinMoon { get; private set; } = null!;
            public static Item FrostMoon { get; private set; } = null!;
            public static Item MartianMadness { get; private set; } = null!;
            public static Item FrostLegion { get; private set; } = null!;
            public static Item OldOnesArmy { get; private set; } = null!;
            public static Item SolarPillar { get; private set; } = null!;
            public static Item VortexPillar { get; private set; } = null!;
            public static Item NebulaPillar { get; private set; } = null!;
            public static Item StardustPillar { get; private set; } = null!;
            public static Item Hardmode { get; private set; } = null!;
            public static Item ItemSpawn { get; private set; } = null!;
            public static Item Bag => ItemSpawn;
            public static Item IfUnlocked { get; private set; } = null!;
            public static Item Unlocked => IfUnlocked;
            public static Item Boss { get; private set; } = null!;
            public static Item Skull => Boss;
            public static Item WhiteQuestionMark { get; private set; } = null!;
            public static Item DarkQuestionMark { get; private set; } = null!;

            internal (int, int) iconPosition;

            public override string Name => $"BestiaryIcon/{iconPosition.Item1}-{iconPosition.Item2}";

            public Bestiary() {
                iconPosition = (0, 0);
                Forest = this.Item;
            }

            public Bestiary((int, int) position) {
                iconPosition = position;
            }

            public override void SetStaticDefaults() {
                (var x, var y) = iconPosition;
                Main.RegisterItemAnimation(Type, new DrawAnimationSheetSlice(
                    new(x * 30, y * 30, 30, 30)
                ));
            }

            public static void registerItems() {
                var mod = ModContent.GetInstance<BingoBoardCore>();
                Item add(int x, int y) {
                    Bestiary icon = new((x, y));
                    mod.AddContent(
                        icon
                    );
                    return icon.Item;
                }
                Underground = add(1, 0);
                Caverns = add(2, 0);
                Desert = add(3, 0);
                UndergroundDesert = add(4, 0);
                Snow = add(5, 0);
                UndergroundSnow = add(6, 0);
                Corruption = add(7, 0);
                UndergroundCorruption = add(8, 0);
                CorruptDesert = add(9, 0);
                UndergroundCorruptDesert = add(10, 0);
                CorruptIce = add(11, 0);
                Crimson = add(12, 0);
                UndergroundCrimson = add(13, 0);
                CrimsonDesert = add(14, 0);
                UndergroundCrimsonDesert = add(15, 0);
                CrimsonIce = add(0, 1);
                Hallow = add(1, 1);
                UndergroundHallow = add(2, 1);
                HallowedDesert = add(3, 1);
                UndergroundHallowedDesert = add(4, 1);
                HallowedIce = add(5, 1);
                Jungle = add(6, 1);
                UndergroundJungle = add(7, 1);
                SurfaceMushroom = add(8, 1);
                UndergroundMushroom = add(9, 1);
                SkyIsland = add(10, 1);
                Oasis = add(11, 1);
                Ocean = add(12, 1);
                Marble = add(13, 1);
                Granite = add(14, 1);
                Temple = add(15, 1);
                Dungeon = add(0, 2);
                Underworld = add(1, 2);
                SpiderCave = add(2, 2);
                Graveyard = add(3, 2);
                Day = add(4, 2);
                Night = add(5, 2);
                BloodMoon = add(6, 2);
                SolarEclipse = add(7, 2);
                Rain = add(8, 2);
                WindyDay = add(9, 2);
                Blizzard = add(10, 2);
                Sandstorm = add(11, 2);
                Meteor = add(12, 2);
                Halloween = add(13, 2);
                Christmas = add(14, 2);
                SlimeRain = add(15, 2);
                Party = add(0, 3);
                GoblinArmy = add(1, 3);
                PirateInvasion = add(2, 3);
                PumpkinMoon = add(3, 3);
                FrostMoon = add(4, 3);
                MartianMadness = add(5, 3);
                FrostLegion = add(6, 3);
                OldOnesArmy = add(7, 3);
                SolarPillar = add(8, 3);
                VortexPillar = add(9, 3);
                NebulaPillar = add(10, 3);
                StardustPillar = add(11, 3);
                Hardmode = add(12, 3);
                ItemSpawn = add(13, 3);
                IfUnlocked = add(14, 3);
                Boss = add(15, 3);
                WhiteQuestionMark = add(0, 4);
                DarkQuestionMark = add(1, 4);
            }
        }
    }
}
