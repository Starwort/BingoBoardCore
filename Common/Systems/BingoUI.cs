using Terraria.ModLoader;
using BingoMod.UI.States;
using Terraria;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BingoMod.Common.Systems {
    public class BingoUI : ModSystem {
        internal static BoardUIState boardUI;
        internal static UserInterface _boardUI;
        internal static string mouseText = "";

        public override void Load() {
            if (!Main.dedServ) {
                boardUI = new BoardUIState();
                boardUI.Activate();
                _boardUI = new UserInterface();
                _boardUI.SetState(boardUI);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
            int inventoryIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Inventory");
            if (inventoryIndex != -1) {
                layers.Insert(
                    inventoryIndex + 1,
                    new LegacyGameInterfaceLayer(
                        "BingoMod: Bingo Board UI",
                        () => {
                            _boardUI.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.UI
                    )
                );
            }
        }

        internal static bool pendingScaleChange = false;
        internal static float lastKnownUIScale = -1;

        public override void UpdateUI(GameTime gameTime) {
            if (lastKnownUIScale != Main.UIScale) {
                lastKnownUIScale = Main.UIScale;
                pendingScaleChange = true;
            }

            if (pendingScaleChange) {
                boardUI.pendingScaleChange = true;
            }

            _boardUI.Update(gameTime);
            if (mouseText.Length > 0) {
                Main.instance.MouseText(mouseText);
            }
        }
    }
}
