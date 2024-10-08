﻿using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using System;
using System.Linq;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BingoBoardCore.Commands {
    internal class TriggerGoalCommand : ModCommand {
        public override string Command => "givegoal";
        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (args.Length < 2) {
                caller.Reply("need goal ID and team colour");
                return;
            }
            var goalId = args[0];
            var team = args[1] switch {
                "white" => Team.None,
                "red" => Team.Red,
                "green" => Team.Green,
                "blue" => Team.Blue,
                "yellow" => Team.Yellow,
                "pink" => Team.Pink,
                _ => throw new Exception("Team colour must be one of white,red,green,blue,yellow,pink"),
            };
            ModContent.GetInstance<BingoBoardSystem>().triggerGoal(goalId, team);
        }
    }
    internal class UnTriggerGoalCommand : ModCommand {
        public override string Command => "takegoal";
        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (args.Length < 2) {
                caller.Reply("need goal ID and team colour");
                return;
            }
            var goalId = args[0];
            var team = args[1] switch {
                "white" => Team.None,
                "red" => Team.Red,
                "green" => Team.Green,
                "blue" => Team.Blue,
                "yellow" => Team.Yellow,
                "pink" => Team.Pink,
                _ => throw new Exception("Team colour must be one of white,red,green,blue,yellow,pink"),
            };
            ModContent.GetInstance<BingoBoardSystem>().untriggerGoal(goalId, team);
        }
    }
    internal class DebugBoardCommand : ModCommand {
        public override string Command => "debugboard";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (args.Length < 2) {
                caller.Reply("need goal position ID and team colour");
                return;
            }
            if (!int.TryParse(args[0], out int id)) {
                return;
            }
            if (id < 0 || id > 24) {
                caller.Reply("goal position ID must be in range [0,25)");
                return;
            }
            var system = ModContent.GetInstance<BingoBoardSystem>();
            var activeGoals = system.activeGoals;
            if (activeGoals is null) { return; }
            switch (args[1]) {
                case "red":
                    activeGoals[id].redCleared = !activeGoals[id].redCleared;
                    break;
                case "green":
                    activeGoals[id].greenCleared = !activeGoals[id].greenCleared;
                    break;
                case "blue":
                    activeGoals[id].blueCleared = !activeGoals[id].blueCleared;
                    break;
                case "yellow":
                    activeGoals[id].yellowCleared = !activeGoals[id].yellowCleared;
                    break;
                case "pink":
                    activeGoals[id].pinkCleared = !activeGoals[id].pinkCleared;
                    break;
                case "white":
                    activeGoals[id].whiteCleared = !activeGoals[id].whiteCleared;
                    break;
                default:
                    caller.Reply("team colour must be one of red,green,blue,yellow,pink,white");
                    break;
            }
            system.sync();
        }
    }
    internal class ForceBoardDisplay : ModCommand {
        public override string Command => "showboard";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is null) {
                system.activeGoals = new GoalState[25];
                Goal goal = BingoBoardSystem.allGoals.Where(g => (g is not DynamicGoal) || (g is DynamicGoal d && d.origin != null)).First();
                for (int i = 0; i < 25; i++) {
                    system.activeGoals[i] = new(goal);
                }
            }
            system.isGameOver = false;
            system.sync();
        }
    }
    internal class GoalDebugCommand : ModCommand {
        public override string Command => "setgoal";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (args.Length < 2) {
                caller.Reply("need goal position ID and goal ID");
                return;
            }
            if (!int.TryParse(args[0], out int id)) {
                return;
            }
            if (id < 0 || id > 24) {
                caller.Reply("goal position ID must be in range [0,25)");
                return;
            }
            var goal = BingoBoardSystem.allGoals.Where(goal => goal.id == args[1]).FirstOrDefault();
            if (goal is null) {
                caller.Reply($"unknown goal {args[1]}");
                return;
            }
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is null) {
                system.activeGoals = new GoalState[25];
                for (var i = 0; i < 25; i++) {
                    system.activeGoals[i] = new(goal);
                }
            }
            system.activeGoals[id] = new(goal);
            system.sync();
        }
    }
}
