using Terraria.ModLoader;

namespace BingoBoardCore.Commands {
    internal class LockoutCommand : ModCommand {
        public override string Command => "lockout";

        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args) {
            throw new System.NotImplementedException();
        }
    }
}
