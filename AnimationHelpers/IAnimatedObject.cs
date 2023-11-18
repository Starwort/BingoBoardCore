using Terraria.ModLoader;

namespace BingoBoardCore.AnimationHelpers {
    public abstract class IAnimatedObject : ModItem {
        public IAnimatedObject() {
            IconAnimationSystem.animatedObjects.Add(Type, this);
        }

        // Update the animation. The frame parameter corresponds
        // to the total number of frames displayed so far. Will only be called
        // when we want a new frame
        public abstract void animate(uint frame);
    }
}
