using BingoBoardCore.Common.Systems;
using System;
using Terraria;

namespace BingoBoardCore.Common {
    public sealed class Goal {
        public readonly Item icon;
        public readonly string description;
        public readonly string id;
        public readonly Func<BingoMode, int, bool> shouldInclude;
        public static bool alwaysInclude(BingoMode mode, int teamCount) => true;
        public Goal(Item icon, string description, string id, Func<BingoMode, int, bool> shouldInclude) {
            this.icon = icon;
            this.description = description;
            this.id = id;
            this.shouldInclude = shouldInclude;
        }
    }
}
