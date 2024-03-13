using BingoBoardCore.Common.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace BingoBoardCore.Common {
    internal class DynamicGoal : Goal {
        private Mod origin;
        private string originId;

        public override string localId => $"{origin.Name}.{originId}";

        public override bool IsLoadingEnabled(Mod mod) {
            return origin is not null;
        }

        public DynamicGoal() {
            this.origin = null!;
        }

        public DynamicGoal(
            Item icon, Mod mod, string id, int difficultyTier,
            string[] synergyTypes, Func<BingoMode, int, bool, bool>? enable = null,
            string iconText = "", Item? modifierIcon = null
        ) {
            this.origin = mod;
            this.originId = id;
            this.icon = icon;
            if (difficultyTier < 0) {
                throw new ArgumentOutOfRangeException(nameof(difficultyTier), "Difficulty tier must not be negative");
            } else if (difficultyTier > 24) {
                throw new ArgumentOutOfRangeException(nameof(difficultyTier), "Difficulty tier too high (must be at most 24)");
            }
            this.difficultyTier = difficultyTier;
            this.synergyTypes = Array.AsReadOnly((string[]) synergyTypes.Clone());
            this._enable = enable ?? base.enable;
            this.modifierText = iconText;
            this.modifierIcon = modifierIcon;
        }

        private readonly Func<BingoMode, int, bool, bool> _enable = (_, _, _) => false;
        public override bool enable(
            BingoMode mode, int numPlayers, bool isSharedWorld
        ) => this._enable(mode, numPlayers, isSharedWorld);

        public override Item icon { get; }
        public override int difficultyTier {
            get;
        }
        public override IList<string> synergyTypes {
            get;
        }
        public override string modifierText {
            get;
        }
        public override Item? modifierIcon {
            get;
        }
    }
}
