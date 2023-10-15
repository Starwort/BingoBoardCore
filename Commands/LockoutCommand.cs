using BingoBoardCore.Common.Systems;
using Terraria.ModLoader;

namespace BingoBoardCore.Commands {
    internal class LockoutCommand : ModCommand {
        public override string Command => "lockout";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            ModContent.GetInstance<BingoBoardSystem>().generateBingoBoard(BingoMode.Lockout);
            caller.Reply("Created new lockout board");
        }
    }
}
