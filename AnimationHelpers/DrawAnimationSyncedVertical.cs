using Terraria;
using Terraria.DataStructures;

namespace BingoBoardCore.AnimationHelpers {
    internal class DrawAnimationSyncedVertical : DrawAnimationVertical {
        public DrawAnimationSyncedVertical(int frameCount) : base((int)BingoBoardCore.animationPeriod, frameCount) { }

        public override void Update() {
            Frame = (int)(Main.GameUpdateCount / BingoBoardCore.animationPeriod);
            Frame %= FrameCount;
        }
    }
}
