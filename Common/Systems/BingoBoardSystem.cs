using Terraria.ModLoader;
using BingoBoardCore.UI.States;
using Terraria;
using Terraria.UI;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ID;
using System.IO;
using System.Diagnostics;
using Terraria.Localization;
using System.Linq;
using Terraria.Chat;
using System;
using Terraria.Enums;

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

        internal List<Action> gameStartCallbacks = new();

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

        public override void UpdateUI(GameTime gameTime) {
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

        // from https://github.com/kbuzsaki/bingosync/blob/main/bingosync-app/generators/generator_bases/srl_generator_v5.js
        static readonly int[][] lineChecklist = new[] {
            new[] {1, 2, 3, 4, 5, 10, 15, 20, 6, 12, 18, 24},
            new[] {0, 2, 3, 4, 6, 11, 16, 21},
            new[] {0, 1, 3, 4, 7, 12, 17, 22},
            new[] {0, 1, 2, 4, 8, 13, 18, 23},
            new[] {0, 1, 2, 3, 8, 12, 16, 20, 9, 14, 19, 24},
            new[] {0, 10, 15, 20, 6, 7, 8, 9},
            new[] {0, 12, 18, 24, 5, 7, 8, 9, 1, 11, 16, 21},
            new[] {5, 6, 8, 9, 2, 12, 17, 22},
            new[] {4, 12, 16, 20, 9, 7, 6, 5, 3, 13, 18, 23},
            new[] {4, 14, 19, 24, 8, 7, 6, 5},
            new[] {0, 5, 15, 20, 11, 12, 13, 14},
            new[] {1, 6, 16, 21, 10, 12, 13, 14},
            new[] {0, 6, 12, 18, 24, 20, 16, 8, 4, 2, 7, 17, 22, 10, 11, 13, 14},
            new[] {3, 8, 18, 23, 10, 11, 12, 14},
            new[] {4, 9, 19, 24, 10, 11, 12, 13},
            new[] {0, 5, 10, 20, 16, 17, 18, 19},
            new[] {15, 17, 18, 19, 1, 6, 11, 21, 20, 12, 8, 4},
            new[] {15, 16, 18, 19, 2, 7, 12, 22},
            new[] {15, 16, 17, 19, 23, 13, 8, 3, 24, 12, 6, 0},
            new[] {4, 9, 14, 24, 15, 16, 17, 18},
            new[] {0, 5, 10, 15, 16, 12, 8, 4, 21, 22, 23, 24},
            new[] {20, 22, 23, 24, 1, 6, 11, 16},
            new[] {2, 7, 12, 17, 20, 21, 23, 24},
            new[] {20, 21, 22, 24, 3, 8, 13, 18},
            new[] {0, 6, 12, 18, 20, 21, 22, 23, 19, 14, 9, 4},
        };

        static int difficulty(int i, int seed) {
            int num3 = seed % 1000;
            int rem8 = num3 % 8;
            int rem4 = rem8 / 2;
            int rem2 = rem8 % 2;
            int rem5 = num3 % 5;
            int rem3 = num3 % 3;
            int remT = num3 / 120;
            var table5 = new List<int> { 0 };
            table5.Insert(rem2, 1);
            table5.Insert(rem3, 2);
            table5.Insert(rem4, 3);
            table5.Insert(rem5, 4);

            num3 = seed / 1000;
            num3 %= 1000;
            rem8 = num3 % 8;
            rem4 = rem8 / 2;
            rem2 = rem8 % 2;
            rem5 = num3 % 5;
            rem3 = num3 % 3;
            remT = remT * 8 + num3 / 120;
            var table1 = new List<int> { 0 };
            table1.Insert(rem2, 1);
            table1.Insert(rem3, 2);
            table1.Insert(rem4, 3);
            table1.Insert(rem5, 4);

            remT %= 5;
            int x = (i + remT) % 5;
            int y = i / 5;
            int e5 = table5[(x + 3 * y) % 5];
            int e1 = table1[(3 * x + y) % 5];
            int value = 5 * e5 + e1;

            //if (MODE == "short") {
            //    value = value / 2;
            //} else if (MODE == "long") {
            //    value = (value + 25) / 2;
            //}

            return value;
        }

        static int checkLine(int i, IList<string> typesA, IList<string>[] boardTypes) {
            var synergy = 0;
            for (int j = 0; j < lineChecklist[i].Length; j++) {
                var typesB = boardTypes[lineChecklist[i][j]];
                if (typesB is null) {
                    continue;
                }
                for (int k = 0; k < typesA.Count; k++) {
                    for (int l = 0; l < typesB.Count; l++) {
                        if (typesA[k] == typesB[l]) {
                            synergy++;
                            // bonus synergy for the primary types of each goal
                            if (k == 0) {
                                synergy++;
                            }
                            if (l == 0) {
                                synergy++;
                            }
                        }
                    }
                }
            }
            return synergy;
        }

        public void generateBingoBoard(BingoMode mode) {
            activeGoals = new GoalState[25];
            this.mode = mode;

            var teams = new HashSet<int>();
            foreach (var player in Main.player) {
                if (!player.active) {
                    continue;
                }
                teams.Add(player.team);
            }

            var eligibleGoalGroups = allGoals.Values
                .Where(goal => goal.shouldInclude(mode, teams.Count))
                .OrderBy(goal => goal.difficultyTier)
                .GroupBy(goal => goal.difficultyTier)
                .Select(group => group.ToArray())
                .ToArray();

            var eligibleGoals = new Goal[25][];
            for (int i = 0; i < 25; i++) {
                eligibleGoals[i] = eligibleGoalGroups.SingleOrDefault(group => group[0].difficultyTier == i)?.ToArray() ?? Array.Empty<Goal>();
            }

            var seed = (new Random()).Next();
            Main.NewText($"generating board with seed {seed}");

            var rand = new Random(seed);

            var boardDifficulties = new int[25];
            for (int i = 0; i < 25; i++) {
                boardDifficulties[i] = difficulty(i, seed);
            }

            var boardTypes = new IList<string>[25];

            for (int i = 0; i < 25; i++) {
                var difficulty = boardDifficulties[i];
                var candidateGoals = eligibleGoals[difficulty];
                var numCandidates = candidateGoals.Length;
                if (numCandidates == 0) {
                    announce(Color.Red, "Mods.BingoBoardCore.Error.NotEnoughGoals", difficulty.ToString());
                    activeGoals = null;
                    return;
                }
                var rng = rand.Next(numCandidates);

                var j = 0;
                var synergy = 0;
                var curGoal = candidateGoals[rng];
                var minSyn = new {
                    synergy = int.MaxValue,
                    goal = curGoal,
                };
                do {
                    curGoal = candidateGoals[(j + rng) % numCandidates];
                    synergy = checkLine(i, curGoal.synergyTypes, boardTypes);
                    if (synergy < minSyn.synergy) {
                        minSyn = new {
                            synergy,
                            goal = curGoal,
                        };
                    }
                    j++;
                } while (synergy != 0 && j < numCandidates);
                boardTypes[i] = minSyn.goal.synergyTypes;
                activeGoals[i] = new(minSyn.goal);
            }

            announce(Color.White, "Mods.BingoBoardCore.MatchStarted", $"Mods.BingoBoardCore.MatchType.{(int) mode}");
            sync();
            foreach (var gameStartCallback in gameStartCallbacks) {
                gameStartCallback();
            }
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
