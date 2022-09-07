using BingoMod.UI.States;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoMod
{
    public class BingoMod : Mod {
        internal BoardUIState boardUI;
        internal UserInterface _boardUI;

        public static BingoMod instance;

		public BingoMod() { }

        public override void Load() {
            Logger.InfoFormat("{0} loading start", Name);
            instance = this;
            if (!Main.dedServ) {
                boardUI = new BoardUIState();
                boardUI.Activate();
                _boardUI = new UserInterface();
                _boardUI.SetState(boardUI);
            }
            Logger.InfoFormat("{0} loading finish", Name);
        }
    }
}