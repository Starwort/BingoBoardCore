using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace BingoBoardCore.AnimationHelpers {
    public abstract class AssetCycleAnimation : ModItem {
        // Placeholder, used to give tML something to load
        public override string Texture => $"Terraria/Images/CoolDown";

        public AssetCycleAnimation() {
            IconAnimationSystem.assetAnimations.Add(this);
        }

        // Get the textureasset corresponding with what to show this seq
        // only called when the texture should update, so safe to make stateful decisions
        // (like calling an rng)
        public abstract Asset<Texture2D> getFrame(uint frame);

        // Set the next seq of the animation. The seq parameter corresponds
        // to the total number of frames displayed so far. Will only be called
        // when we want a new seq
        internal void animate(uint frame) {
            TextureAssets.Item[Type] = getFrame(frame);
        }
    }
}
