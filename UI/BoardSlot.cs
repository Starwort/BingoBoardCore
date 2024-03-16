using BingoBoardCore.Common.Systems;
using BingoBoardCore.Icons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoBoardCore.UI {
    internal class BoardSlot : UIElement {
        public readonly int index;
        internal GoalState goalState;
        internal bool isMarked;
        internal UIText iconText;

        public BoardSlot(int index, GoalState goalState) {
            this.index = index;

            this.goalState = goalState;
            this.isMarked = false;

            Top.Set((index / 5) * (TextureAssets.InventoryBack9.Value.Height + 4) + 4, 0f);
            Left.Set((index % 5) * (TextureAssets.InventoryBack9.Value.Width + 4) + 4, 0f);
            Width.Set(TextureAssets.InventoryBack9.Value.Width, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height, 0f);
            iconText = new(goalState.goal.modifierText);
            iconText.Left.Set(4, 0);
            iconText.Top.Set(TextureAssets.InventoryBack9.Value.Height - 20, 0);
            iconText.DynamicallyScaleDownToWidth = true;
            this.Append(iconText);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
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
            Main.DrawItemIcon(spriteBatch, goalState.goal.cachedIcon, origin, Color.White, this.GetDimensions().Width - 8);
            if (possibleColours.Count == 0 || system.mode != BingoMode.Lockout) {
                Main.DrawItemIcon(
                    spriteBatch,
                    this.isMarked
                        ? VanillaIcons.Star.On
                        : VanillaIcons.Star.Off,
                    origin + markOffset,
                    Color.White,
                    16
                );
            }
            if (goalState.goal.modifierIcon is Item icon) {
                Main.DrawItemIcon(spriteBatch, icon, origin + modifierOffset, Color.White, 16);
            }
        }
        internal static readonly Vector2 modifierOffset = new(16, -16);
        internal static readonly Vector2 markOffset = new(16, 16);
        internal static readonly Item markIcon = new(ItemID.FallenStar);

        public override void Update(GameTime gameTime) {
            iconText.SetText(goalState.goal.modifierText);
            base.Update(gameTime);
        }

        public override void RightMouseDown(UIMouseEvent evt) {
            this.isMarked = !this.isMarked;
        }

        public override void MouseOver(UIMouseEvent evt) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.boardUI.visible) {
                var extraInfo = goalState.goal.progressText();
                system.mouseText = Language.GetTextValue(goalState.goal.description)
                    + (extraInfo is null ? "" : $"\n{extraInfo}");
            }
        }

        public override void MouseOut(UIMouseEvent evt) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.boardUI.visible) {
                system.mouseText = "";
            }
        }
    }
}
