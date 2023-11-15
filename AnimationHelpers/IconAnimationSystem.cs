using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace BingoBoardCore.AnimationHelpers {
    public class IconAnimationSystem : ModSystem {
        internal static uint animationPeriod => BingoBoardCore.animationPeriod;

        internal static List<(Item, int[])> seqAnimations = new();
        internal static List<(Item, int[])> randAnimations = new();
        internal static List<AssetCycleAnimation> assetAnimations = new();

        private Random rng = new();

        public override void PreUpdateItems() {
            if (Main.GameUpdateCount % animationPeriod == 0) {
                var frame = Main.GameUpdateCount / animationPeriod;
                foreach ((Item icon, int[] frames) in seqAnimations) {
                    icon.type = frames[frame % frames.Length];
                }
                foreach ((Item icon, int[] frames) in randAnimations) {
                    icon.type = frames[rng.Next(frames.Length)];
                }
                foreach (var animation in assetAnimations) {
                    animation.animate(frame);
                }
            }
        }

        public static Item registerCycleAnimation(params int[] itemIds) {
            Item rv = new(itemIds[0]);
            seqAnimations.Add((rv, itemIds));
            return rv;
        }

        public static Item registerCycleAnimation(IEnumerable<int> itemIds) {
            return registerCycleAnimation(itemIds.ToArray());
        }

        public static Item registerRandAnimation(params int[] itemIds) {
            Item rv = new(itemIds[0]);
            randAnimations.Add((rv, itemIds));
            return rv;
        }

        public static Item registerRandAnimation(IEnumerable<int> itemIds) {
            return registerRandAnimation(itemIds.ToArray());
        }
    }
}
