using Terraria.ModLoader;

namespace BingoBoardCore.Trackers {
    public abstract class NpcTracker : GlobalNPC {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
}
