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
using System;
using Terraria.Enums;
using Terraria.DataStructures;

namespace BingoBoardCore.Common.Systems {
    internal class GoalState {
        public GoalState(Goal goal) {
            this.goal = goal;
        }

        public Goal goal;
        public byte packedClear = 0;
        public bool whiteCleared {
            get => (packedClear & (1 << 0)) != 0;
            set {
                if (value) {
                    packedClear |= 1 << 0;
                } else if (whiteCleared) {
                    packedClear ^= 1 << 0;
                }
            }
        }
        public bool redCleared {
            get => (packedClear & (1 << 1)) != 0;
            set {
                if (value) {
                    packedClear |= 1 << 1;
                } else if (redCleared) {
                    packedClear ^= 1 << 1;
                }
            }
        }
        public bool greenCleared {
            get => (packedClear & (1 << 2)) != 0;
            set {
                if (value) {
                    packedClear |= 1 << 2;
                } else if (greenCleared) {
                    packedClear ^= 1 << 2;
                }
            }
        }
        public bool blueCleared {
            get => (packedClear & (1 << 3)) != 0;
            set {
                if (value) {
                    packedClear |= 1 << 3;
                } else if (blueCleared) {
                    packedClear ^= 1 << 3;
                }
            }
        }
        public bool yellowCleared {
            get => (packedClear & (1 << 4)) != 0;
            set {
                if (value) {
                    packedClear |= 1 << 4;
                } else if (yellowCleared) {
                    packedClear ^= 1 << 4;
                }
            }
        }
        public bool pinkCleared {
            get => (packedClear & (1 << 5)) != 0;
            set {
                if (value) {
                    packedClear |= 1 << 5;
                } else if (pinkCleared) {
                    packedClear ^= 1 << 5;
                }
            }
        }
    }

    public enum BingoMode {
        Bingo,
        TripleBingo,
        Blackout,
        Lockout,
    }

    public class BingoBoardSystem : ModSystem {
        internal BoardUIState boardUI = new();
        internal UserInterface _boardUI = new();
        internal string mouseText = "";
        internal Dictionary<string, Goal> allGoals = new();
        internal GoalState[]? activeGoals;
        internal bool isGameOver = false;
        internal BingoMode mode = BingoMode.Bingo;

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

        internal static void announce(Color colour, string text, params string[] substitutions) {
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

        public void addGoals(IEnumerable<Goal> goals) {
            foreach (var goal in goals) {
                addGoal(goal);
            }
        }

        public void addGoal(Goal goal) {
            Debug.Assert(!allGoals.ContainsKey(goal.id));
            allGoals[goal.id] = goal;
        }

        public void triggerGoal(string goalId, Team team) {
            if (activeGoals is null || isGameOver) {
                return;
            }
            foreach (GoalState slot in activeGoals) {
                if (slot.goal.id == goalId) {
                    if (mode == BingoMode.Lockout && (
                        slot.whiteCleared || slot.redCleared
                        || slot.greenCleared || slot.blueCleared
                        || slot.yellowCleared || slot.pinkCleared
                    )) {
                        return;
                    }
                    switch (team) {
                        case Team.None:
                            slot.whiteCleared = true;
                            break;
                        case Team.Red:
                            slot.redCleared = true;
                            break;
                        case Team.Green:
                            slot.greenCleared = true;
                            break;
                        case Team.Blue:
                            slot.blueCleared = true;
                            break;
                        case Team.Yellow:
                            slot.yellowCleared = true;
                            break;
                        case Team.Pink:
                            slot.pinkCleared = true;
                            break;
                    }
                    announce(
                        Main.teamColor[(int) team],
                        "Mods.BingoBoardCore.Achieve",
                        teams[(int) team],
                        slot.goal.description
                    );
                }
            }
            checkGameEnd();
            sync();
        }

        // Good for 'never do X' goals
        public void untriggerGoal(string goalId, Team team) {
            if (activeGoals is null || isGameOver) {
                return;
            }
            foreach (GoalState slot in activeGoals) {
                if (slot.goal.id == goalId) {
                    switch (team) {
                        case Team.None:
                            slot.whiteCleared = false;
                            break;
                        case Team.Red:
                            slot.redCleared = false;
                            break;
                        case Team.Green:
                            slot.greenCleared = false;
                            break;
                        case Team.Blue:
                            slot.blueCleared = false;
                            break;
                        case Team.Yellow:
                            slot.yellowCleared = false;
                            break;
                        case Team.Pink:
                            slot.pinkCleared = false;
                            break;
                    }
                    announce(
                        Main.teamColor[(int) team],
                        "Mods.BingoBoardCore.Fail",
                        teams[(int) team],
                        slot.goal.description
                    );
                }
            }
            sync();
        }


        public void generateBingoBoard(BingoMode mode) {
            activeGoals = new GoalState[25];
            this.mode = mode;

            for (int i = 0; i < 25; i++) {
                activeGoals[i] = new(this.allGoals[$"BingoBoardCore.TestGoal{i}"]);
            }

            announce(Color.White, "Mods.BingoBoardCore.MatchStarted", $"Mods.BingoBoardCore.MatchType.{(int) mode}");
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
                    boardUI.setupUI(activeGoals!);
                }
            }
        }

        internal void sync() {
            if (Main.netMode == NetmodeID.Server) {
                NetMessage.SendData(MessageID.WorldData);
            }
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
                var state = new GoalState(this.allGoals[goalId]) {
                    packedClear = reader.ReadByte()
                };
                this.activeGoals[i] = state;
            }
            isGameOver = reader.ReadBoolean();
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
                writer.Write(goal.packedClear);
            }
            writer.Write(isGameOver);
        }

        internal void checkGameEnd() {
            switch (this.mode) {
                case BingoMode.Bingo:
                    checkBingoEnd();
                    break;
                case BingoMode.TripleBingo:
                    checkTripleBingoEnd();
                    break;
                case BingoMode.Blackout:
                    checkBlackoutEnd();
                    break;
                case BingoMode.Lockout:
                    checkLockoutEnd();
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        internal static Team maskToTeam(byte teamMask) {
            for (int i = 0; i < 6; i++) {
                if ((teamMask & (1 << i)) != 0) {
                    return (Team) i;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(teamMask), "Must be set");
        }

        static readonly string[] teams = new[] {
            "Mods.BingoBoardCore.Team.White",
            "Mods.BingoBoardCore.Team.Red",
            "Mods.BingoBoardCore.Team.Green",
            "Mods.BingoBoardCore.Team.Blue",
            "Mods.BingoBoardCore.Team.Yellow",
            "Mods.BingoBoardCore.Team.Pink",
        };

        // Get the bitmask for row Y of goals
        internal static byte checkRow(GoalState[] activeGoals, int y) {
            byte rv = 0b111111;
            for (int i = 0; i < 5; i++) {
                rv &= activeGoals[y * 5 + i].packedClear;
            }
            return rv;
        }

        // Get the bitmask for col X of goals
        internal static byte checkCol(GoalState[] activeGoals, int x) {
            byte rv = 0b111111;
            for (int i = 0; i < 5; i++) {
                rv &= activeGoals[i * 5 + x].packedClear;
            }
            return rv;
        }

        internal static byte checkDiagTLDR(GoalState[] activeGoals) {
            byte rv = 0b111111;
            for (int i = 0; i < 5; i++) {
                rv &= activeGoals[i * 6].packedClear;
            }
            return rv;
        }

        internal static byte checkDiagDLTR(GoalState[] activeGoals) {
            byte rv = 0b111111;
            for (int i = 0; i < 5; i++) {
                rv &= activeGoals[20 - i * 4].packedClear;
            }
            return rv;
        }

        internal void endGame(Team team) {
            announce(Main.teamColor[(int) team], "Mods.BingoBoardCore.GameEnd", teams[(int) team]);
            isGameOver = true;
        }

        internal void checkBingoEnd() {
            if (activeGoals is null) {
                return;
            }
            var winners = checkDiagTLDR(activeGoals);
            winners |= checkDiagDLTR(activeGoals);
            if (winners != 0) {
                endGame(maskToTeam(winners));
                return;
            }
            for (int i = 0; i < 5; i++) {
                winners = checkRow(activeGoals, i);
                winners |= checkCol(activeGoals, i);

                if (winners != 0) {
                    endGame(maskToTeam(winners));
                    return;
                }
            }
        }

        internal void checkTripleBingoEnd() {
            if (activeGoals is null) {
                return;
            }
            var lineCount = new int[6];
            for (int i = 0; i < 5; i++) {
                var rowData = checkRow(activeGoals, i);
                var colData = checkCol(activeGoals, i);
                for (int bit = 0; bit < 6; bit++) {
                    if ((rowData & (1 << bit)) != 0) {
                        lineCount[bit]++;
                    }
                    if ((colData & (1 << bit)) != 0) {
                        lineCount[bit]++;
                    }
                }
            }
            var tldrDiag = checkDiagTLDR(activeGoals);
            var dltrDiag = checkDiagDLTR(activeGoals);
            for (int bit = 0; bit < 6; bit++) {
                if ((tldrDiag & (1 << bit)) != 0) {
                    lineCount[bit]++;
                }
                if ((dltrDiag & (1 << bit)) != 0) {
                    lineCount[bit]++;
                }

                if (lineCount[bit] >= 3) {
                    endGame((Team) bit);
                    return;
                }
            }
        }

        internal void checkBlackoutEnd() {
            if (activeGoals is null) {
                return;
            }
            byte winners = 0b111111;
            for (int i = 0; i < 25; i++) {
                winners &= activeGoals[i].packedClear;
            }
            if (winners != 0) {
                endGame(maskToTeam(winners));
            }
        }

        internal void checkLockoutEnd() {
            if (activeGoals is null) {
                return;
            }
            var currentScores = new int[6];
            var freeSquares = 0;
            for (int i = 0; i < 25; i++) {
                var squareInfo = activeGoals[i].packedClear;
                if (squareInfo == 0) {
                    freeSquares++;
                } else {
                    currentScores[(int) maskToTeam(squareInfo)]++;
                }
            }
            for (int team = 0; team < 6; team++) {
                var otherTeamsBestPossibleScore = 0;
                for (int other = 0; other < 6; other++) {
                    if (team == other) {
                        continue;
                    }
                    var bestPossibleScore = currentScores[other] + freeSquares;
                    if (bestPossibleScore > otherTeamsBestPossibleScore) {
                        otherTeamsBestPossibleScore = bestPossibleScore;
                    }
                }
                if (currentScores[team] > otherTeamsBestPossibleScore) {
                    endGame((Team) team);
                    return;
                }
            }
        }
    }
}
