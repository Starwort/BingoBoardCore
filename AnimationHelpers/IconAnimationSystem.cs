using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BingoBoardCore.AnimationHelpers {
    internal class IconAnimationSystem : ModSystem {
        internal uint animationPeriod => BingoBoardCore.animationPeriod;

        internal List<(Item, int[])> seqAnimations = new();
        internal List<(Item, int[])> randAnimations = new();
        internal List<AssetCycleAnimation> assetAnimations = new();

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
    }
}
