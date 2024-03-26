using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BingoBoardCore.Trackers {
    internal class ObtainedItemTrackerImpl : ModPlayer {
        internal static readonly List<ObtainedItemTracker> trackers = new();

        internal HashSet<int> usedAccs = new();
        internal void onEquipAccessory(Item acc) {
            if (usedAccs.Contains(acc.type)) {
                return;
            }
            usedAccs.Add(acc.type);
            foreach (var tracker in trackers) {
                tracker.onEquipAccessory(Player, acc);
            }
        }
        internal void onCraftItem(Item itm) {
            foreach (var tracker in trackers) {
                tracker.onCraftItem(Player, itm);
                tracker.onAnyObtain(Player, itm);
            }
        }
        internal HashSet<int> obtainedItems = new();
        internal void onAnyObtain(Item itm) {
            if (obtainedItems.Contains(itm.type)) {
                return;
            }
            obtainedItems.Add(itm.type);
            foreach (var tracker in trackers) {
                tracker.onAnyObtain(Player, itm);
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

    public abstract class ObtainedItemTracker : ModType {
        public virtual void onEquipAccessory(Player player, Item acc) {
        }
        public virtual void onCraftItem(Player player, Item item) {
        }
        public virtual void onAnyObtain(Player player, Item item) {
        }

        protected sealed override void Register() {
            ObtainedItemTrackerImpl.trackers.Add(this);
        }
    }
}
