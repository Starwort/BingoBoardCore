using System.Linq;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BingoBoardCore.Trackers {
    internal class ObtainedItemTrackerImpl : ModPlayer {
        internal static readonly List<ObtainedItemTracker> trackers = new();
        internal List<ObtainedItemTracker> myTrackers = new();

        public override void Initialize() {
            myTrackers = (
                from tracker in trackers
                select Player.GetModPlayer(tracker)
            ).ToList();
        }

        internal HashSet<int> usedAccs = new();
        internal void onEquipAccessory(Item acc) {
            if (usedAccs.Contains(acc.type)) {
                return;
            }
            usedAccs.Add(acc.type);
            foreach (var tracker in myTrackers) {
                tracker.onEquipAccessory(acc);
            }
        }
        internal void onCraftItem(Item itm) {
            foreach (var tracker in myTrackers) {
                tracker.onCraftItem(itm);
                tracker.onAnyObtain(itm);
            }
        }
        internal HashSet<int> obtainedItems = new();
        internal void onAnyObtain(Item itm) {
            if (obtainedItems.Contains(itm.type)) {
                return;
            }
            obtainedItems.Add(itm.type);
            foreach (var tracker in myTrackers) {
                tracker.onAnyObtain(itm);
            }
        }

        public override void PostUpdate() {
            foreach (var item in Player.inventory) {
                if (item.stack > 0) {
                    onAnyObtain(item);
                }
            }
        }

        internal void reset() {
            usedAccs.Clear();
            obtainedItems.Clear();
            foreach (var tracker in myTrackers) {
                tracker.prepare();
            }
        }
    }
    internal class CraftAndArmourSetTracker : GlobalItem {
        public override void UpdateAccessory(Item item, Player player, bool _) {
            player.GetModPlayer<ObtainedItemTrackerImpl>().onEquipAccessory(item);
        }

        public override void OnCreated(Item item, ItemCreationContext context) {
            if (context is RecipeItemCreationContext) {
                Main.LocalPlayer.GetModPlayer<ObtainedItemTrackerImpl>().onCraftItem(item);
            }
        }
    }

    public abstract class ObtainedItemTracker : PlayerTracker {
        public virtual void onEquipAccessory(Item acc) {
        }
        public virtual void onCraftItem(Item item) {
        }
        public virtual void onAnyObtain(Item item) {
        }
        /// <summary>
        /// Prepare the tracker to track its goal.
        /// </summary>
        public virtual void prepare() {
        }

        /// <summary>
        /// Replacement for <see cref="Load"/>, which is sealed as it performs important initialisation.
        /// </summary>
        public virtual void onLoad() {
        }

        public sealed override void Load() {
            ObtainedItemTrackerImpl.trackers.Add(this);
            onLoad();
        }
    }
}
