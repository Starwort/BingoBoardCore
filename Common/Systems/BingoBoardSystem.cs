using Terraria.ModLoader;
using BingoBoardCore.UI.States;
using Terraria;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Collections;
using Terraria.ID;
using System.IO;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria.Localization;
using System.Linq;
using Terraria.Chat;

namespace BingoBoardCore.Common.Systems {
    internal class GoalState {
        public GoalState(Goal goal) {
            this.goal = goal;
        }

        public Goal goal;
        public bool redCleared = false;
        public bool greenCleared = false;
        public bool blueCleared = false;
        public bool yellowCleared = false;
        public bool pinkCleared = false;
        public bool whiteCleared = false;
    }

    public enum BingoMode {
        Bingo,
        TripleBingo,
        Blackout,
        Lockout,
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
            mode = BingoMode.Bingo;
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

        internal void announce(Color colour, string text, params string[] substitutions) {
            if (Main.netMode == NetmodeID.Server) {
                ChatHelper.BroadcastChatMessage(
                    NetworkText.FromKey(text, substitutions.Select(text =>
                        NetworkText.FromKey(text)
                    ).ToArray()), colour);
            } else {
                Main.NewText(
                    Language.GetTextValue(text, substitutions.Select(text =>
                        Language.GetTextValue(text)
                    ).ToArray()), colour);
            }
        }

        internal BingoMode mode;

        public void generateBingoBoard(BingoMode mode) {
            activeGoals = new GoalState[25];
            this.mode = mode;

            for (int i = 0; i < 25; i++) {
                activeGoals[i] = new(this.allGoals[$"BingoBoardCore.TestGoal{i}"]);
            }

            sync();
        }

        public void destroyBingoBoard() {
            activeGoals = null;
            sync();
        }

        internal void syncUI() {
            if (!Main.dedServ) {
                boardUI.visible = activeGoals is not null;
                if (boardUI.visible) {
                    Debug.Assert(activeGoals is not null); // fix null warning
                    boardUI.setupUI(activeGoals);
                }
            }
        }

        internal void sync() {
            NetMessage.SendData(MessageID.WorldData);
            syncUI();
        }

        public override void NetReceive(BinaryReader reader) {
            var goals = reader.ReadBoolean();
            boardUI.visible = goals;
            if (!goals) {
                this.activeGoals = null;
                return;
            }
            this.activeGoals = new GoalState[25];
            this.mode = (BingoMode) reader.ReadByte();
            for (int i = 0; i < 25; i++) {
                var goalId = reader.ReadString();
                var state = new GoalState(this.allGoals[goalId]);
                var packedClear = reader.ReadByte();
                state.redCleared = (packedClear & (1 << 0)) != 0;
                state.greenCleared = (packedClear & (1 << 1)) != 0;
                state.blueCleared = (packedClear & (1 << 2)) != 0;
                state.yellowCleared = (packedClear & (1 << 3)) != 0;
                state.pinkCleared = (packedClear & (1 << 4)) != 0;
                state.whiteCleared = (packedClear & (1 << 5)) != 0;
                this.activeGoals[i] = state;
            }
            syncUI();
        }

        public override void NetSend(BinaryWriter writer) {
            writer.Write(activeGoals is not null);
            if (activeGoals is null) {
                return;
            }
            Debug.Assert(activeGoals.Length == 25);
            writer.Write((byte) this.mode);
            foreach (var goal in activeGoals) {
                writer.Write(goal.goal.id);
                byte packedClear = 0;
                packedClear |= (byte) (goal.redCleared ? 1 << 0 : 0);
                packedClear |= (byte) (goal.greenCleared ? 1 << 1 : 0);
                packedClear |= (byte) (goal.blueCleared ? 1 << 2 : 0);
                packedClear |= (byte) (goal.yellowCleared ? 1 << 3 : 0);
                packedClear |= (byte) (goal.pinkCleared ? 1 << 4 : 0);
                packedClear |= (byte) (goal.whiteCleared ? 1 << 5 : 0);
                writer.Write(packedClear);
            }
        }
    }
}
