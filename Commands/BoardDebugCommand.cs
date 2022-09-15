using BingoMod.Common.Systems;
using Terraria.ModLoader;

namespace BingoMod.Commands {
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
            if (BingoUI.boardUI is null) { return; }
            switch (args[1]) {
                case "red":
                    BingoUI.boardUI.innerPanels[id].redCleared = !BingoUI.boardUI.innerPanels[id].redCleared;
                    break;
                case "green":
                    BingoUI.boardUI.innerPanels[id].greenCleared = !BingoUI.boardUI.innerPanels[id].greenCleared;
                    break;
                case "blue":
                    BingoUI.boardUI.innerPanels[id].blueCleared = !BingoUI.boardUI.innerPanels[id].blueCleared;
                    break;
                case "yellow":
                    BingoUI.boardUI.innerPanels[id].yellowCleared = !BingoUI.boardUI.innerPanels[id].yellowCleared;
                    break;
                case "pink":
                    BingoUI.boardUI.innerPanels[id].pinkCleared = !BingoUI.boardUI.innerPanels[id].pinkCleared;
                    break;
                default:
                    caller.Reply("team colour must be one of red,green,blue,yellow,pink");
                    break;
            }
        }
    }
}
