using BingoBoardCore.Common.Systems;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BingoBoardCore.Common {
    public abstract class Goal : ModType {
        // For cycle animations, contributing mods should do one of the following:
        // - Register an item within that mod to perform that animation (using
        //   BingoBoardCore.AnimationHelpers to ensure that the user's cycle speed config
        //   is respected)
        // - Use BingoBoardCore[.IconAnimationSystem].register{Rand,Cycle}Animation()
        //   to register an animation
        public abstract Item icon { get; }
        public string description => "Mods." + id;
        protected readonly string localId;
        public string id => Mod is BingoBoardCore ? localId : Mod.Name + '.' + localId;
        // Difficulty tier, from 0 to 24
        public abstract int difficultyTier { get; }
        public virtual IList<string> synergyTypes => Array.Empty<string>();
        public virtual bool enable(BingoMode mode, int numPlayers, bool isSharedWorld) {
            return true;
        }
        public virtual string modifierText => "";
        public virtual Item? modifierIcon => null;
        public Goal(string localId) {
            this.localId = localId;
            if (difficultyTier < 0) {
                throw new ArgumentOutOfRangeException(nameof(difficultyTier), "Difficulty tier must not be negative");
            } else if (difficultyTier > 24) {
                throw new ArgumentOutOfRangeException(nameof(difficultyTier), "Difficulty tier too high (must be at most 24)");
            }
        }

        internal static BingoBoardSystem? _system;
        internal static BingoBoardSystem system = _system ??= ModContent.GetInstance<BingoBoardSystem>();

        public void trigger(Team team) {
            system.triggerGoal(this.id, team);
        }
        public void untrigger(Team team) {
            system.untriggerGoal(this.id, team);
        }
        public virtual string? progressText() => null;
        public virtual void onGameStart() {}

        // Report progress towards this goal, if this goal is present.
        // Localisation keys are expected to be of the format Mods.YourMod.Progress.GoalName
        public void reportProgress(params string[] substitutions) {
            BingoBoardCore.reportProgress(this.id, "Mods." + this.Mod.Name + ".Progress." + this.localId, substitutions);
        }
        // Report progress towards this goal, if this goal is present.
        // Localisation keys are expected to be of the format Mods.YourMod.BadProgress.GoalName
        public void reportBadProgress(params string[] substitutions) {
            BingoBoardCore.reportBadProgress(this.id, "Mods." + this.Mod.Name + ".BadProgress." + this.localId, substitutions);
        }

        protected sealed override void Register() {
            if (this is not DynamicGoal) {
                BingoBoardSystem.addGoal(this);
            }
        }
    }
}
