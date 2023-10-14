using BingoBoardCore.Common.Systems;
using Terraria.ModLoader;

namespace BingoBoardCore.Commands {
    internal class BoardDebugCommand : ModCommand {
        public override string Command => "debugboard";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (args.Length < 2) {
                caller.Reply("need goalState ID and team colour");
                return;
            }
            if (!int.TryParse(args[0], out int id)) {
                return;
            }
            if (id < 0 || id > 24) {
                caller.Reply("goalState ID must be in range [0,25)");
                return;
            }
            var activeGoals = ModContent.GetInstance<BingoBoardSystem>().activeGoals;
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
                default:
                    caller.Reply("team colour must be one of red,green,blue,yellow,pink");
                    break;
            }
        }
    }
}
