using System;
using Terraria;

namespace BingoBoardCore.Common {
    public sealed class Goal {
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
