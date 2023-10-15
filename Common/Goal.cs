using System;
using Terraria;

namespace BingoBoardCore.Common {
    public sealed class Goal {
        public readonly Item icon;
        public readonly string description;
        public readonly string id;
        public Goal(Item icon, string description, string id) {
            this.icon = icon;
            this.description = description;
            this.id = id;
        }
    }
}
