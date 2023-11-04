using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace BingoBoardCore.AnimationHelpers {
    public class DrawAnimationSheetSlice : DrawAnimation {
        public Rectangle frame;

        public DrawAnimationSheetSlice(Rectangle frame) {
            this.frame = frame;
        }

        public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1) {
            return frame;
        }
    }
}
