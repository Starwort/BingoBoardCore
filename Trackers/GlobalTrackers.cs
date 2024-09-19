using Terraria.ModLoader;

namespace BingoBoardCore.Trackers {
    public abstract class ItemTracker : GlobalItem {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
    public abstract class NpcTracker : GlobalNPC {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
    public abstract class PlayerTracker : ModPlayer {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
    public abstract class ProjectileTracker : GlobalProjectile {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
    public abstract class TileTracker : GlobalTile {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
    public abstract class TrackerSystem : ModSystem {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
}
