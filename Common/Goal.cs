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
        internal Item? iconCache;
        internal Item cachedIcon { get => iconCache ??= icon; }
        public string description => "Mods." + id;
        public virtual string localId => this.GetType().Name.Replace('_', '.');
        public string id => Mod is BingoBoardCore ? localId : Mod.Name + '.' + localId;
        // Difficulty tier, from 0 to 24
        public abstract int difficultyTier { get; }
        public virtual IList<string> synergyTypes => [];
        public virtual bool enable(BingoMode mode, int numPlayers, bool isSharedWorld) {
            return true;
        }
        public virtual string modifierText => "";
        public virtual Item? modifierIcon => null;
        public Goal() {
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
        public void trigger(Player player) {
            trigger((Team) player.team);
        }
        public void untrigger(Team team) {
            system.untriggerGoal(this.id, team);
        }
        public void untrigger(Player player) {
            untrigger((Team) player.team);
        }
        public virtual string? progressTextFor(Player player) {
            return null;
        }
        public virtual void onGameStart(Player player) {}
        public virtual void onGameEnd(Player player) {}

        // Report progress towards this goal, if this goal is present.
        // Localisation keys are expected to be of the format Mods.YourMod.Progress.GoalName
        public void reportProgress(Player player, params string[] substitutions) {
            if (player.whoAmI == Main.myPlayer) {
                BingoBoardCore.reportProgress(this.id, "Mods." + this.Mod.Name + ".Progress." + this.localId, substitutions);
            }
        }
        // Report progress towards this goal, if this goal is present.
        // Localisation keys are expected to be of the format Mods.YourMod.BadProgress.GoalName
        public void reportBadProgress(Player player, params string[] substitutions) {
            if (player.whoAmI == Main.myPlayer) {
                BingoBoardCore.reportBadProgress(this.id, "Mods." + this.Mod.Name + ".BadProgress." + this.localId, substitutions);
            }
        }

        protected sealed override void Register() {
            ModTypeLookup<Goal>.Register(this);
        }
    }
}
