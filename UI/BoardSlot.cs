using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;

namespace BingoMod.UI {
    internal class BoardSlot : UIElement {
        public readonly int index;
        public int context;
        Item displayItem;

        public BoardSlot(int index, Item innerItem, int context = ItemSlot.Context.InventoryItem) {
            this.index = index;
            this.context = context;

            displayItem = innerItem;

            Width.Set(TextureAssets.InventoryBack9.Value.Width * Main.UIScale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height * Main.UIScale, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            Vector2 origin = GetDimensions().Position();
            float tmp = Main.inventoryScale;
            Main.inventoryScale = Main.UIScale;
            ItemSlot.Draw(
                spriteBatch,
                ref displayItem,
                context, 
                origin + new Vector2(
                    (index % 5) * (TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) + 2,
                    (index / 5) * (TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) + 2
                )
            );
            Main.inventoryScale = tmp;
        }
    }
}
