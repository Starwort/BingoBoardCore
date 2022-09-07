using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace BingoMod.UI.States
{
    internal class BoardUIState : UIState
    {
        public DraggableUIPanel boardPanel;
        public static bool visible = true;
        public Item[] displayItems;
        internal BoardSlot[] innerPanels;
        internal bool pendingScaleChange;

        public override void OnInitialize()        {
            displayItems = new Item[25];
            innerPanels = new BoardSlot[25];

            boardPanel = new DraggableUIPanel();
            boardPanel.SetPadding(0);
            boardPanel.Left.Set(400f, 0f);
            boardPanel.Top.Set(100f, 0f);
            boardPanel.Width.Set((TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) * 5 + 4, 0f);
            boardPanel.Height.Set((TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) * 5 + 4, 0f);
            boardPanel.BackgroundColor = new Color(73, 94, 171);

            for (int i = 0; i < 25; i++)            {
                displayItems[i] = new Item(ItemID.Zenith);
                innerPanels[i] = new BoardSlot(i, displayItems[i]);
                boardPanel.Append(innerPanels[i]);
            }

            Append(boardPanel);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (pendingScaleChange) {
                boardPanel.Width.Set((TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) * 5 + 4, 0f);
                boardPanel.Height.Set((TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) * 5 + 4, 0f);
                pendingScaleChange = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            base.Draw(spriteBatch);
        }
    }
}
