using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent;
using Terraria;
using Microsoft.Xna.Framework;

namespace BingoBoardCore.Util {
    internal static class DrawingHelper {
        // for whatever reason, TextureAssets.MagicPixel.Value is 1000px tall, or 62.5 tiles
        // normalise that so the texture draws at a reasonable scale
        static Vector2 normaliseVector = new(1, 0.001f);
        public static void drawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color colour) {
            var pos = rect.TopLeft();
            var size = rect.Size();
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, pos, null, colour * 0.6f, 0, Vector2.Zero, size * normaliseVector, SpriteEffects.None, 0);
            // Draw borders
            var vScale = new Vector2(2, size.Y) * normaliseVector;
            var hScale = new Vector2(size.X, 2) * normaliseVector;
            var borderColour = colour;
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, pos - Vector2.UnitX * 2, null, borderColour, 0, Vector2.Zero, vScale, SpriteEffects.None, 0);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, pos - Vector2.UnitY * 2, null, borderColour, 0, Vector2.Zero, hScale, SpriteEffects.None, 0);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, pos + Vector2.UnitX * size.X, null, borderColour, 0, Vector2.Zero, vScale, SpriteEffects.None, 0);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, pos + Vector2.UnitY * size.Y, null, borderColour, 0, Vector2.Zero, hScale, SpriteEffects.None, 0);
        }
    }
}
