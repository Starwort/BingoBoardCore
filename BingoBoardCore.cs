global using static BingoBoardCore.Util.DrawingHelper;
using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using BingoBoardCore.UI;
using BingoBoardCore.UI.States;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoBoardCore
{
    public class BingoBoardCore : Mod {
        public static BingoBoardCore? instance;

        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoBoardCore";

        public static void addGoals(IEnumerable<Goal> goals) {
            BingoBoardSystem.allGoals ??= new List<Goal>();
            foreach (Goal goal in goals) {
                BingoBoardSystem.allGoals.Add(goal);
            }
        }

        public static void triggerGoal(Goal goal, Team team) {
            if (BingoBoardSystem.boardUI is null) {
                throw new InvalidOperationException("boardUI is null");
            }
            foreach (BoardSlot slot in BingoBoardSystem.boardUI.innerPanels) {
                if (slot.goal.id == goal.id) {
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