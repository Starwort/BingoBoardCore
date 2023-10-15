global using static BingoBoardCore.Util.DrawingHelper;
using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using BingoBoardCore.UI;
using BingoBoardCore.UI.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Chat;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoBoardCore {
    public class BingoBoardCore : Mod {
        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoBoardCore";

        public static void addGoals(IEnumerable<Goal> goals) {
            foreach (var goal in goals) {
                addGoal(goal);
            }
        }

        public static void addGoal(Goal goal) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            Debug.Assert(!system.allGoals.ContainsKey(goal.id));
            system.allGoals[goal.id] = goal;
        }

        public static void triggerGoal(string goalId, Team team) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is null) {
                throw new InvalidOperationException("activeGoals is null");
            }
            foreach (GoalState slot in system.activeGoals) {
                if (slot.goal.id == goalId) {
                    if (system.mode == BingoMode.Lockout && (
                        slot.whiteCleared || slot.redCleared
                        || slot.greenCleared || slot.blueCleared
                        || slot.yellowCleared || slot.pinkCleared
                    )) {
                        return;
                    }
                    string teamKey = "Mods.BingoBoardCore.Debug.Error";
                    switch (team) {
                        case Team.None:
                            slot.whiteCleared = true;
                            teamKey = "Mods.BingoBoardCore.Team.White";
                            break;
                        case Team.Red:
                            slot.redCleared = true;
                            teamKey = "Mods.BingoBoardCore.Team.Red";
                            break;
                        case Team.Green:
                            slot.greenCleared = true;
                            teamKey = "Mods.BingoBoardCore.Team.Green";
                            break;
                        case Team.Blue:
                            slot.blueCleared = true;
                            teamKey = "Mods.BingoBoardCore.Team.Blue";
                            break;
                        case Team.Yellow:
                            slot.yellowCleared = true;
                            teamKey = "Mods.BingoBoardCore.Team.Yellow";
                            break;
                        case Team.Pink:
                            slot.pinkCleared = true;
                            teamKey = "Mods.BingoBoardCore.Team.Pink";
                            break;
                    }
                    if (Main.netMode == NetmodeID.Server) {
                        var text = NetworkText.FromKey("Mods.BingoBoardCore.Achieve", NetworkText.FromKey(teamKey), NetworkText.FromKey(slot.goal.description));
                        ChatHelper.BroadcastChatMessage(text, Main.teamColor[(int) team]);
                    } else {
                        var text = Language.GetTextValue("Mods.BingoBoardCore.Achieve", Language.GetTextValue(teamKey), Language.GetTextValue(slot.goal.description));
                        Main.NewText(text, Main.teamColor[(int) team]);
                    }
                }
            }
            system.sync();
        }

        // Good for 'never do X' goals
        public static void untriggerGoal(string goalId, Team team) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is null) {
                throw new InvalidOperationException("activeGoals is null");
            }
            foreach (GoalState slot in system.activeGoals) {
                if (slot.goal.id == goalId) {
                    string teamKey = "Mods.BingoBoardCore.Debug.Error";
                    switch (team) {
                        case Team.None:
                            slot.whiteCleared = false;
                            teamKey = "Mods.BingoBoardCore.Team.White";
                            break;
                        case Team.Red:
                            slot.redCleared = false;
                            teamKey = "Mods.BingoBoardCore.Team.Red";
                            break;
                        case Team.Green:
                            slot.greenCleared = false;
                            teamKey = "Mods.BingoBoardCore.Team.Green";
                            break;
                        case Team.Blue:
                            slot.blueCleared = false;
                            teamKey = "Mods.BingoBoardCore.Team.Blue";
                            break;
                        case Team.Yellow:
                            slot.yellowCleared = false;
                            teamKey = "Mods.BingoBoardCore.Team.Yellow";
                            break;
                        case Team.Pink:
                            slot.pinkCleared = false;
                            teamKey = "Mods.BingoBoardCore.Team.Pink";
                            break;
                    }
                    if (Main.netMode == NetmodeID.Server) {
                        var text = NetworkText.FromKey("Mods.BingoBoardCore.Fail", NetworkText.FromKey(teamKey), NetworkText.FromKey(slot.goal.description));
                        ChatHelper.BroadcastChatMessage(text, Main.teamColor[(int) team]);
                    } else {
                        var text = Language.GetTextValue("Mods.BingoBoardCore.Fail", Language.GetTextValue(teamKey), Language.GetTextValue(slot.goal.description));
                        Main.NewText(text, Main.teamColor[(int) team]);
                    }
                }
            }
            system.sync();
        }

        public override void Load() {
            for (int i = 0; i < 25; i++) {
                addGoal(new Goal(
                    new Item(ItemID.Zenith),
                    $"Test {i}",
                    $"BingoBoardCore.TestGoal{i}"
                ));
            }
        }
    }
}