using BingoMod.Common;
using BingoMod.Common.Systems;
using BingoMod.UI;
using BingoMod.UI.States;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoMod
{
    public class BingoMod : Mod {
        public static BingoMod? instance;

        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoMod";

        public static void addGoals(IEnumerable<Goal> goals) {
            BingoUI.goals ??= new List<Goal>();
            foreach (Goal goal in goals) {
                BingoUI.goals.Add(goal);
            }
        }

        public static void triggerGoal(int goalId, Team team) {
            if (BingoUI.boardUI is null) {
                throw new InvalidOperationException("boardUI is null");
            }
            foreach (BoardSlot slot in BingoUI.boardUI.innerPanels) {
                if (slot.goal.id == goalId) {
                    switch (team) {
                        case Team.None: throw new ArgumentOutOfRangeException(nameof(team), "team must not be None");
                        case Team.Red: slot.redCleared = true; break;
                        case Team.Green: slot.greenCleared = true; break;
                        case Team.Blue: slot.blueCleared = true; break;
                        case Team.Yellow: slot.yellowCleared = true; break;
                        case Team.Pink: slot.pinkCleared = true; break;
                    }
                    return;
                }
            }
        }

        public override void Load() {
            Logger.InfoFormat("{0} loading start", Name);
            instance = this;
            Logger.InfoFormat("{0} loading finish", Name);
        }
    }
}