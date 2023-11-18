using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace BingoBoardCore.AnimationHelpers {
    public abstract class AssetCycleAnimation : IAnimatedObject {
        // Placeholder, used to give tML something to load
        public sealed override string Texture => "Terraria/Images/CoolDown";

        // Get the textureasset corresponding with what to show this frame
        // only called when the texture should update, so safe to make stateful decisions
        // (like calling an rng)
        public abstract Asset<Texture2D> getFrame(uint frame);

        public sealed override void animate(uint frame) {
            TextureAssets.Item[Type] = getFrame(frame);
        }
    }
}
