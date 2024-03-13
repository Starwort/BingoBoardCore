using BingoBoardCore.Common.Systems;
using System;
using System.Collections.Generic;
using Terraria;
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
        public abstract string description { get; }
        protected readonly string localId;
        public string id => Mod.Name + '.' + localId;
        // Difficulty tier, from 0 to 24
        public abstract int difficultyTier { get; }
        public abstract IList<string> synergyTypes { get; }
        public virtual bool enable(BingoMode mode, int numPlayers, bool isSharedWorld) {
            return true;
        }
        public abstract string modifierText { get; }
        public abstract Item? modifierIcon { get; }
        public Goal(string localId) {
            this.localId = localId;
        }

        public virtual bool trigger() => false;
        public virtual bool untrigger() => false;
        public virtual string? reportProgress() => null;
        public virtual void onGameStart() {}

        public void reportProgress(string progressText, params string[] substitutions) {
            BingoBoardCore.reportProgress(this.id, progressText, substitutions);
        }
        public void reportBadProgress(string progressText, params string[] substitutions) {
            BingoBoardCore.reportBadProgress(this.id, progressText, substitutions);
        }

        protected sealed override void Register() {
            if (this is not DynamicGoal) {
                BingoBoardSystem.addGoal(this);
            }
        }
    }
}
