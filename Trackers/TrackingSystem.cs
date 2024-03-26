using Terraria.ModLoader;

namespace BingoBoardCore.Trackers {
    public abstract class TrackerSystem : ModSystem {
        public override string Name => this.GetType().AssemblyQualifiedName ?? this.GetType().Name;
    }
}
