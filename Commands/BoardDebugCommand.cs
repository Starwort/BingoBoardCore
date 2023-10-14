using BingoBoardCore.Common.Systems;
using Terraria.ModLoader;

namespace BingoBoardCore.Commands {
    internal class BoardDebugCommand : ModCommand {
        public override string Command => "debugboard";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            if (args.Length < 2) {
                caller.Reply("need goal ID and team colour");
                return;
            }
            if (!int.TryParse(args[0], out int id)) {
                return;
            }
            if (id < 0 || id > 24) {
                caller.Reply("goal ID must be in range [0,25)");
                return;
            }
            if (BingoBoardSystem.boardUI is null) { return; }
            switch (args[1]) {
                case "red":
                    BingoBoardSystem.boardUI.innerPanels[id].redCleared = !BingoBoardSystem.boardUI.innerPanels[id].redCleared;
                    break;
                case "green":
                    BingoBoardSystem.boardUI.innerPanels[id].greenCleared = !BingoBoardSystem.boardUI.innerPanels[id].greenCleared;
                    break;
                case "blue":
                    BingoBoardSystem.boardUI.innerPanels[id].blueCleared = !BingoBoardSystem.boardUI.innerPanels[id].blueCleared;
                    break;
                case "yellow":
                    BingoBoardSystem.boardUI.innerPanels[id].yellowCleared = !BingoBoardSystem.boardUI.innerPanels[id].yellowCleared;
                    break;
                case "pink":
                    BingoBoardSystem.boardUI.innerPanels[id].pinkCleared = !BingoBoardSystem.boardUI.innerPanels[id].pinkCleared;
                    break;
                default:
                    caller.Reply("team colour must be one of red,green,blue,yellow,pink");
                    break;
            }
        }
    }
}
