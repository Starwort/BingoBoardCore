using Terraria.ModLoader;
using BingoBoardCore.UI.States;
using Terraria;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BingoBoardCore.Common.Systems {
    public class BingoBoardSystem : ModSystem {
        internal static BoardUIState? boardUI;
        internal static UserInterface? _boardUI;
        internal static string mouseText = "";
        internal static List<Goal>? allGoals;
        private Goal[] activeGoals;

        public BingoBoardSystem() : base() {
            boardUI = new BoardUIState();
            _boardUI = new UserInterface();
            allGoals ??= new List<Goal>();
            activeGoals = new Goal[0];
        }

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
                        "BingoBoardCore: Bingo Board UI",
                        () => {
                            _boardUI?.Draw(Main.spriteBatch, new GameTime());
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

            if (pendingScaleChange && boardUI is not null) {
                boardUI.pendingScaleChange = true;
                pendingScaleChange = false;
            }

            _boardUI?.Update(gameTime);
            if (mouseText.Length > 0) {
                Main.instance.MouseText(mouseText);
            }
        }
    }
}
