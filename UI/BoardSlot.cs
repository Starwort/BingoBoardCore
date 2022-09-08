using BingoMod.Common;
using BingoMod.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;

namespace BingoMod.UI {
    internal class BoardSlot : UIElement {
        public readonly int index;
        public int context;
        internal Item[] displayItem;
        internal Goal goal;

        public BoardSlot(int index, Goal goal, int context = ItemSlot.Context.InventoryItem) {
            this.index = index;
            this.context = context;

            displayItem = new Item[11];
            // hack to remove the number 1 and selection highlight
            // you can probably trick the highlight into coming back via auto select though
            displayItem[10] = goal.icon;
            this.goal = goal;

            Width.Set(TextureAssets.InventoryBack9.Value.Width * Main.UIScale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height * Main.UIScale, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            Vector2 origin = GetDimensions().Position();
            float tmp = Main.inventoryScale;
            Main.inventoryScale = Main.UIScale;
            ItemSlot.Draw(
                spriteBatch,
                displayItem,
                context,
                10,
                origin + new Vector2(
                    (index % 5) * (TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) + 4,
                    (index / 5) * (TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) + 4
                )
            );
            Main.inventoryScale = tmp;
        }

        public override void MouseOver(UIMouseEvent evt) {
            BingoUI.mouseText = Language.GetTextValue(goal.description);
        }

        public override void MouseOut(UIMouseEvent evt) {
            BingoUI.mouseText = "";
        }
    }
}
