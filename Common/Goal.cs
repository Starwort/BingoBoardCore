using Terraria;

namespace BingoMod.Common {
    public class Goal {
        public readonly Item icon;
        public readonly string description;
        public readonly int id;
        private static int nextID = 0;
        public Goal(Item icon, string description) {
            this.icon = icon;
            this.description = description;
            id = nextID++;
        }
    }
}
