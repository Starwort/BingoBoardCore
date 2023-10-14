using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.UI;

namespace BingoBoardCore.UI.States {
    internal class BoardUIState : UIState {
        public DraggableUIPanel boardPanel;
        public bool visible = true;
        internal BoardSlot[] innerPanels;
        internal bool pendingScaleChange;

        public BoardUIState() : base() {
            boardPanel = new DraggableUIPanel();
            innerPanels = new BoardSlot[25];
        }

        public void setupUI(GoalState[] goals) {
            Debug.Assert(goals.Length == 25);
            innerPanels = new BoardSlot[25];

            boardPanel = new DraggableUIPanel();
            boardPanel.SetPadding(0);
            boardPanel.Left.Set(400f, 0f);
            boardPanel.Top.Set(100f, 0f);
            boardPanel.Width.Set((TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) * 5 + 4, 0f);
            boardPanel.Height.Set((TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) * 5 + 4, 0f);

            for (int i = 0; i < 25; i++) {
                innerPanels[i] = new BoardSlot(i, goals[i]);
                boardPanel.Append(innerPanels[i]);
            }
            Append(boardPanel);
            this.visible = true;
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (pendingScaleChange) {
                boardPanel.Width.Set((TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) * 5 + 4, 0f);
                boardPanel.Height.Set((TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) * 5 + 4, 0f);
                for (int i = 0; i < 25; i++) {
                    innerPanels[i].pendingScaleChange = true;
                }
                pendingScaleChange = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch) {
            if (this.visible) {
                base.Draw(spriteBatch);
            }
        }
    }
}
