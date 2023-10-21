using BingoBoardCore.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoBoardCore.UI {
    internal class DraggableUIPanel : UIElement {
        // Stores the offset from the top left of the UIPanel while dragging.
        private Vector2 offset;
        public bool dragging;

        public override void LeftMouseDown(UIMouseEvent evt) {
            if (ModContent.GetInstance<BingoBoardSystem>().boardUI.visible) {
                base.LeftMouseDown(evt);
                DragStart(evt);
            }
        }

        public override void LeftMouseUp(UIMouseEvent evt) {
            if (ModContent.GetInstance<BingoBoardSystem>().boardUI.visible) {
                base.LeftMouseUp(evt);
                DragEnd(evt);
            }
        }

        private void DragStart(UIMouseEvent evt) {
            offset = new Vector2(evt.MousePosition.X - Left.Pixels, evt.MousePosition.Y - Top.Pixels);
            dragging = true;
        }

        private void DragEnd(UIMouseEvent evt) {
            Vector2 end = evt.MousePosition;
            dragging = false;

            Left.Set(end.X - offset.X, 0f);
            Top.Set(end.Y - offset.Y, 0f);

            Recalculate();
        }

        public override void Update(GameTime gameTime) {
            if (this.ContainsPoint(Main.MouseScreen)) {
                Main.LocalPlayer.mouseInterface = true;
            }
            if (dragging) {
                Left.Set(Main.mouseX - offset.X, 0f);
                Top.Set(Main.mouseY - offset.Y, 0f);
                Recalculate();
            }

            // If off the screen, snap back onto the screen
            var parentSpace = Parent.GetDimensions().ToRectangle();
            if (!parentSpace.Contains(GetDimensions().ToRectangle())) {
                Left.Pixels = Utils.Clamp(Left.Pixels, 0, parentSpace.Right - Width.Pixels);
                Top.Pixels = Utils.Clamp(Top.Pixels, 0, parentSpace.Bottom - Height.Pixels);
                Recalculate();
            }
            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            drawRectangle(spriteBatch, this.GetDimensions().ToRectangle(), new Color(73, 94, 171));
        }
    }
}
