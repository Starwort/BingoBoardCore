using BingoMod.UI.States;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoMod
{
    public class BingoMod : Mod {
        public static BingoMod instance;

        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoMod";

        public BingoMod() { }

        public override void Load() {
            Logger.InfoFormat("{0} loading start", Name);
            instance = this;
            Logger.InfoFormat("{0} loading finish", Name);
        }
    }
}