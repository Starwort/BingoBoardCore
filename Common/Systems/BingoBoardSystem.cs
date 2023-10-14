using Terraria.ModLoader;
using BingoBoardCore.UI.States;
using Terraria;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Collections;
using Terraria.ID;

namespace BingoBoardCore.Common.Systems {
    internal class GoalState {
        public GoalState(Goal goal) {
            this.goal = goal;
            redCleared = greenCleared = blueCleared = yellowCleared = pinkCleared = false;
        }

        public Goal goal;
        public bool redCleared;
        public bool greenCleared;
        public bool blueCleared;
        public bool yellowCleared;
        public bool pinkCleared;
    }
    public class BingoBoardSystem : ModSystem {
        internal BoardUIState boardUI;
        internal UserInterface _boardUI;
        internal string mouseText = "";
        internal Dictionary<string, Goal> allGoals = new();
        internal GoalState[]? activeGoals;

        public BingoBoardSystem() : base() {
            boardUI = new BoardUIState();
            _boardUI = new UserInterface();
            activeGoals = null;
            allGoals.Clear();
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

        internal bool pendingScaleChange = false;
        internal float lastKnownUIScale = -1;

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

        public void createBingoBoard() {
            activeGoals = new GoalState[25];

            for (int i = 0; i < 25; i++) {
                activeGoals[i] = new(new Goal(
                    new Item(ItemID.Zenith),
                    $"Test {i}"
                ));
            }

            boardUI.setupUI(activeGoals);
        }

        public void destroyBingoBoard() {
            activeGoals = null;
            boardUI.visible = false;
        }
    }
}
