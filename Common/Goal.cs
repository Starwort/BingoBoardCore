using BingoBoardCore.Common.Systems;
using System;
using System.Collections.Generic;
using Terraria;

namespace BingoBoardCore.Common {
    public class Goal {
        public readonly Item icon;
        public readonly string description;
        public readonly string id;
        // Difficulty tier, from 0 to 24
        public readonly int difficultyTier;
        public readonly IList<string> synergyTypes;
        public readonly Func<BingoMode, int, bool> shouldInclude;
        public static bool alwaysInclude(BingoMode mode, int teamCount) => true;
        public Goal(Item icon, string description, string id, int difficultyTier, string[] synergyTypes, Func<BingoMode, int, bool> shouldInclude) {
            this.icon = icon;
            this.description = description;
            this.id = id;
            if (difficultyTier < 0) {
                throw new ArgumentOutOfRangeException(nameof(difficultyTier), "Difficulty tier must not be negative");
            } else if (difficultyTier > 24) {
                throw new ArgumentOutOfRangeException(nameof(difficultyTier), "Difficulty tier too high (must be at most 24)");
            }
            this.difficultyTier = difficultyTier;
            this.synergyTypes = Array.AsReadOnly((string[]) synergyTypes.Clone());
            this.shouldInclude = shouldInclude;
        }
    }
}
