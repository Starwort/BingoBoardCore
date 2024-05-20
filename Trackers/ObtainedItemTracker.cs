using System;
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

        Dictionary<(Type, Type), dynamic?> genericStorage = new();

        internal T storage<T>(Type owner, Func<T> createDefault) {
            var key = (owner, typeof(T));
            if (!genericStorage.ContainsKey(key)) {
                genericStorage.Add(key, createDefault());
            }
            // this is fine - can only be null if T is a nullable type
            // and the type will always match
            return (T)genericStorage[key]!;
        }

        internal T storage<T>(Type owner) {
            // this is fine - can only be null if T is a nullable type
            // and the type will always match
            return (T) genericStorage[(owner, typeof(T))]!;
        }

        internal void reset() {
            usedAccs.Clear();
            obtainedItems.Clear();
            genericStorage.Clear();
            foreach (var tracker in trackers) {
                tracker.prepare(Player);
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

    public abstract class ObtainedItemTracker : ModType {
        public virtual void onEquipAccessory(Player player, Item acc) {
        }
        public virtual void onCraftItem(Player player, Item item) {
        }
        public virtual void onAnyObtain(Player player, Item item) {
        }
        /// <summary>
        /// Prepare the tracker to track its goal.
        /// </summary>
        /// <param name="player"></param>
        public virtual void prepare(Player player) {
        }

        /// <summary>
        /// Player-specific storage, accessible only to this tracker. Missing values will be created using <paramref name="createDefault"/>.
        /// <br/>
        /// Only one value can be stored per type.
        /// <br/>
        /// Will be cleared <em>before</em> <see cref="prepare(Player)"/> is called.
        /// <br/>
        /// <strong>Do not</strong> attempt to store value types here - they will not be updated.
        /// </summary>
        /// <typeparam name="T">The type of storage to use</typeparam>
        /// <param name="player">The <see cref="Player"/> owning the value</param>
        /// <param name="createDefault">A factory to create the value if it doesn't exist yet (always true on first call)</param>
        /// <returns>The storage value</returns>
        protected T storage<T>(Player player, Func<T> createDefault) {
            return player.GetModPlayer<ObtainedItemTrackerImpl>().storage(this.GetType(), createDefault);
        }
        /// <summary>
        /// Player-specific storage, specifically for this tracker. Missing values will cause a <see cref="KeyNotFoundException"/>.
        /// <br/>
        /// Only one value can be stored per type.
        /// <br/>
        /// Will be cleared <em>before</em> <see cref="prepare(Player)"/> is called.
        /// <br/>
        /// <strong>Do not</strong> attempt to store value types here - they will not be updated.
        /// </summary>
        /// <typeparam name="T">The type of storage to use</typeparam>
        /// <param name="player">The <see cref="Player"/> owning the value</param>
        /// <returns>The storage value</returns>
        protected T storage<T>(Player player) {
            return player.GetModPlayer<ObtainedItemTrackerImpl>().storage<T>(this.GetType());
        }

        protected sealed override void Register() {
            ObtainedItemTrackerImpl.trackers.Add(this);
        }
    }
}
