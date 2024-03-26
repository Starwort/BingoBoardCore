using Terraria.ModLoader;

namespace BingoBoardCore.Trackers {
    public abstract class ItemTracker : GlobalItem {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
}
