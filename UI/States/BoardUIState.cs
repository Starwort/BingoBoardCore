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
        public bool visible = false;
        internal BoardSlot[] innerPanels;
        internal bool pendingScaleChange;

        internal static readonly Goal dummyGoal = new(
            new(ItemID.FallenStar),
            "Mods.BingoBoardCore.Debug.Error",
            "BingoBoardCore.Placeholder",
            0,
            System.Array.Empty<string>(),
            (_, _) => false
        );

        public BoardUIState() : base() {
            boardPanel = new DraggableUIPanel();
            innerPanels = new BoardSlot[25];
            if (!Main.dedServ) {
                boardPanel.SetPadding(0);
                boardPanel.Width.Set((TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) * 5 + 4, 0f);
                boardPanel.Height.Set((TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) * 5 + 4, 0f);
                var parentSpace = GetDimensions().ToRectangle();
                boardPanel.Left.Set(0, 0f);
                boardPanel.Top.Set(parentSpace.Bottom - Height.Pixels, 0f);
                for (int i = 0; i < 25; i++) {
                    innerPanels[i] = new BoardSlot(i, new(dummyGoal));
                    boardPanel.Append(innerPanels[i]);
                }
                Append(boardPanel);
            }
        }

        public void setupUI(GoalState[] goalStates) {
            if (!Main.dedServ) {
                Debug.Assert(goalStates.Length == 25);

                for (int i = 0; i < 25; i++) {
                    innerPanels[i].goalState = goalStates[i];
                }
            }
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            if (pendingScaleChange && this.visible) {
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
