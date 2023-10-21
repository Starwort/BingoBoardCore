using BingoBoardCore.Common.Systems;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BingoBoardCore.Commands {
    internal class BingoCommand : ModCommand {
        public override string Command => "bingo";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is not null && !system.isGameOver) {
                caller.Reply(Language.GetTextValue("Mods.BingoBoardCore.Error.GameInProgress"), Color.Red);
                return;
            }
            system.generateBingoBoard(BingoMode.Bingo);
        }
    }
    internal class TripleBingoCommand : ModCommand {
        public override string Command => "triplebingo";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is not null && !system.isGameOver) {
                caller.Reply(Language.GetTextValue("Mods.BingoBoardCore.Error.GameInProgress"), Color.Red);
                return;
            }
            system.generateBingoBoard(BingoMode.TripleBingo);
        }
    }
    internal class BlackoutCommand : ModCommand {
        public override string Command => "blackout";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is not null && !system.isGameOver) {
                caller.Reply(Language.GetTextValue("Mods.BingoBoardCore.Error.GameInProgress"), Color.Red);
                return;
            }
            system.generateBingoBoard(BingoMode.Blackout);
        }
    }
    internal class LockoutCommand : ModCommand {
        public override string Command => "lockout";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is not null && !system.isGameOver) {
                caller.Reply(Language.GetTextValue("Mods.BingoBoardCore.Error.GameInProgress"), Color.Red);
                return;
            }
            system.generateBingoBoard(BingoMode.Lockout);
        }
    }
    internal class StopGameCommand : ModCommand {
        public override string Command => "endgame";

        public override CommandType Type => CommandType.World;

        public override void Action(CommandCaller caller, string input, string[] args) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            if (system.activeGoals is not null) {
                system.activeGoals = null;
                if (system.isGameOver) {
                    caller.Reply(Language.GetTextValue("Mods.BingoBoardCore.ClosedBoard"));
                } else {
                    caller.Reply(Language.GetTextValue("Mods.BingoBoardCore.CancelledActiveGame"));
                }
            } else {
                caller.Reply(Language.GetTextValue("Mods.BingoBoardCore.Error.NoBoard"), Color.Red);
            }
        }
    }
}
