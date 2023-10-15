using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using BingoBoardCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoBoardCore.UI {
    internal class BoardSlot : UIElement {
        public readonly int index;
        private readonly Item displayItem;
        internal GoalState goalState;
        internal bool pendingScaleChange = false;
        internal bool isMarked;

        public BoardSlot(int index, GoalState goalState) {
            this.index = index;

            displayItem = goalState.goal.icon;

            this.goalState = goalState;
            this.isMarked = false;

            Top.Set((index / 5) * (TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) + 2, 0f);
            Left.Set((index % 5) * (TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) + 2, 0f);
            Width.Set(TextureAssets.InventoryBack9.Value.Width * Main.UIScale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height * Main.UIScale, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            // skip drawing if activegoals has been disposed
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is null) {
                return;
            }
            var dims = this.GetDimensions();
            Vector2 origin = dims.Center();
            var possibleColours = new List<Color>();
            if (goalState.redCleared) {
                possibleColours.Add(Main.teamColor[1]);
            }
            if (goalState.greenCleared) {
                possibleColours.Add(Main.teamColor[2]);
            }
            if (goalState.blueCleared) {
                possibleColours.Add(Main.teamColor[3]);
            }
            if (goalState.yellowCleared) {
                possibleColours.Add(Main.teamColor[4]);
            }
            if (goalState.pinkCleared) {
                possibleColours.Add(Main.teamColor[5]);
            }
            if (goalState.whiteCleared) {
                possibleColours.Add(Main.teamColor[0]);
            }
            var chosenColour = possibleColours.Count == 0 ? new Color(73, 94, 171) : possibleColours[(int) ((Main.GameUpdateCount / 60) % possibleColours.Count)];
            drawRectangle(spriteBatch, this.GetDimensions().ToRectangle(), chosenColour);
            Main.DrawItemIcon(spriteBatch, displayItem, origin, Color.White, this.GetDimensions().Width - 8);
            if (this.isMarked && (possibleColours.Count == 0 || system.mode != BingoMode.Lockout)) {
                var starTexture = TextureAssets.Item[ItemID.FallenStar].Value;
                var starAnim = Main.itemAnimations[ItemID.FallenStar];
                spriteBatch.Draw(starTexture, new Rectangle((int) origin.X + 8, (int) origin.Y + 8, 16, 16),
                    starAnim.GetFrame(starTexture),
                    Color.White);
            }
        }

        public override void Update(GameTime gameTime) {
            if (pendingScaleChange) {
                Top.Set((index / 5) * (TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) + 4, 0f);
                Left.Set((index % 5) * (TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) + 4, 0f);
                Width.Set(TextureAssets.InventoryBack9.Value.Width * Main.UIScale, 0f);
                Height.Set(TextureAssets.InventoryBack9.Value.Height * Main.UIScale, 0f);
                pendingScaleChange = false;
            }
        }

        public override void RightMouseDown(UIMouseEvent evt) {
            this.isMarked = !this.isMarked;
        }

        public override void MouseOver(UIMouseEvent evt) {
            ModContent.GetInstance<BingoBoardSystem>().mouseText = Language.GetTextValue(goalState.goal.description);
        }

        public override void MouseOut(UIMouseEvent evt) {
            ModContent.GetInstance<BingoBoardSystem>().mouseText = "";
        }
    }
}
