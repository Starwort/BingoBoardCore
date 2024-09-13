using Terraria.ModLoader;

namespace BingoBoardCore.Trackers {
    public abstract class TileTracker : GlobalTile {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
}
