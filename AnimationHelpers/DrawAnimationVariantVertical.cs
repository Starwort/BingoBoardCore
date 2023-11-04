using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;

namespace BingoBoardCore.AnimationHelpers {
    public class DrawAnimationVariantVertical : DrawAnimationVertical {
        public int sheetFrameCount;
        public int variantStart;

        public DrawAnimationVariantVertical(int sheetFrames, int variantStart, int ticksperframe, int frameCount, bool pingPong = false) : base(ticksperframe, frameCount, pingPong) {
            this.sheetFrameCount = sheetFrames;
            this.variantStart = variantStart;
        }

        public override Rectangle GetFrame(Texture2D texture, int frameCounterOverride = -1) {
            if (frameCounterOverride != -1) {
                int num = frameCounterOverride / TicksPerFrame;
                int num2 = FrameCount;
                if (PingPong) {
                    num2 = num2 * 2 - 1;
                }

                int num3 = num % num2;
                if (PingPong && num3 >= FrameCount) {
                    num3 = FrameCount * 2 - 2 - num3;
                }

                Rectangle result = texture.Frame(1, sheetFrameCount, 0, num3 + variantStart);
                result.Height -= 2;
                return result;
            }

            int frameY = Frame;
            if (PingPong && Frame >= FrameCount) {
                frameY = FrameCount * 2 - 2 - Frame;
            }

            Rectangle result2 = texture.Frame(1, sheetFrameCount, 0, frameY + variantStart);
            result2.Height -= 2;
            return result2;
        }
    }
}
