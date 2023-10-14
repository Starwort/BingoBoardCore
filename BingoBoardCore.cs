global using static BingoBoardCore.Util.DrawingHelper;
using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using BingoBoardCore.UI;
using BingoBoardCore.UI.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoBoardCore
{
    public class BingoBoardCore : Mod {
        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoBoardCore";

        public void addGoals(IEnumerable<(string, Goal)> goals) {
            foreach ((string id, Goal goal) data in goals) {
                this.addGoal(data.id, data.goal);
            }
        }

        public void addGoal(string goalId, Goal goal) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            Debug.Assert(!system.allGoals.ContainsKey(goalId));
            system.allGoals[goalId] = goal;
        }

        public void triggerGoal(Goal goal, Team team) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is null) {
                throw new InvalidOperationException("activeGoals is null");
            }
            foreach (GoalState slot in system.activeGoals) {
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
    }
}