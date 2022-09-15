using BingoMod.Common;
using BingoMod.Common.Systems;
using BingoMod.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.UI;

namespace BingoMod.UI {
    internal class BoardSlot : UIElement {
        public readonly int index;
        private Item displayItem;
        private bool _redCleared = false;
        public bool redCleared {
            get => _redCleared; 
            set  {
                _redCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        3, // ChestItem, best I could do
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) { return; }
                    var nodeBefore = cyclePosition.findBefore(3);
                    if (nodeBefore.next == cyclePosition) {
                        cyclePosition = cyclePosition.next;
                        cycleTimer = 30;
                    }
                    nodeBefore.next = nodeBefore.next.next;
                }
            }
        }
        private bool _greenCleared = false;
        public bool greenCleared {
            get => _greenCleared;
            set {
                _greenCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        18, // 8, 10, 16-20 => equipment; 18 is EquipMinecart.
                            // There's no real difference between them when an item is in the slot
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) { return; }
                    var nodeBefore = cyclePosition.findBefore(18);
                    if (nodeBefore.next == cyclePosition) {
                        cyclePosition = cyclePosition.next;
                        cycleTimer = 30;
                    }
                    nodeBefore.next = nodeBefore.next.next;
                }
            }
        }
        private bool _blueCleared = false;
        public bool blueCleared {
            get => _blueCleared; 
            set  {
                _blueCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        31, // Count (whatever that is)
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) { return; }
                    var nodeBefore = cyclePosition.findBefore(31);
                    if (nodeBefore.next == cyclePosition) {
                        cyclePosition = cyclePosition.next;
                        cycleTimer = 30;
                    }
                    nodeBefore.next = nodeBefore.next.next;
                }
            }
        }
        private bool _yellowCleared = false;
        public bool yellowCleared {
            get => _yellowCleared; 
            set  {
                _yellowCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        15, // ShopItem; ok it's not yellow but it's the best I've got
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) { return; }
                    var nodeBefore = cyclePosition.findBefore(15);
                    if (nodeBefore.next == cyclePosition) {
                        cyclePosition = cyclePosition.next;
                        cycleTimer = 30;
                    }
                    nodeBefore.next = nodeBefore.next.next;
                }
            }
        }
        private bool _pinkCleared = false;
        public bool pinkCleared {
            get => _pinkCleared;
            set {
                _pinkCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        4, // BankItem - not perfect but pretty good
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) { return; }
                    var nodeBefore = cyclePosition.findBefore(4);
                    if (nodeBefore.next == cyclePosition) {
                        cyclePosition = cyclePosition.next;
                        cycleTimer = 30;
                    }
                    nodeBefore.next = nodeBefore.next.next;
                    }
            }
        }
        internal Goal goal;
        private int cycleTimer = -1;
        private LLNode<int>? cyclePosition = null;
        internal bool pendingScaleChange = false;

        public BoardSlot(int index, Goal goal) {
            this.index = index;

            displayItem = goal.icon;
            // hack to remove the number 1 and selection highlight
            // you can probably trick the highlight into coming back via auto select though
            this.goal = goal;

            Top.Set((index / 5) * (TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) + 2, 0f);
            Left.Set((index % 5) * (TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) + 2, 0f);
            Width.Set(TextureAssets.InventoryBack9.Value.Width * Main.UIScale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height * Main.UIScale, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            Vector2 origin = GetDimensions().Position();
            float tmp = Main.inventoryScale;
            Main.inventoryScale = Main.UIScale;
            int context;
            if (cyclePosition is not null) {
                if (cycleTimer-- == 0) {
                    cyclePosition = cyclePosition.next;
                    cycleTimer = 30;
                }
                context = cyclePosition.value;
            } else {
                context = 28; // GoldDebug. Highlights when hovered
            }
            ItemSlot.Draw(
                spriteBatch,
                ref displayItem,
                context,
                origin
            );
            Main.inventoryScale = tmp;
        }

        public override void Update(GameTime gameTime) {
            if (pendingScaleChange) {
                Top.Set((index / 5) * (TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) + 4, 0f);
                Left.Set((index % 5) * (TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) + 4, 0f);
                Width.Set(TextureAssets.InventoryBack9.Value.Width * Main.UIScale, 0f);
                Height.Set(TextureAssets.InventoryBack9.Value.Height * Main.UIScale, 0f);
                pendingScaleChange = false;
            }
        }

        public override void MouseOver(UIMouseEvent evt) {
            BingoUI.mouseText = Language.GetTextValue(goal.description);
        }

        public override void MouseOut(UIMouseEvent evt) {
            BingoUI.mouseText = "";
        }
    }
}
