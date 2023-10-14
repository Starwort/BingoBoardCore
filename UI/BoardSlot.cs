using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using BingoBoardCore.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.UI;

namespace BingoBoardCore.UI {
    internal class BoardSlot : UIElement {
        public readonly int index;
        private Item displayItem;
        private bool _redCleared = false;
        public bool redCleared {
            get => _redCleared;
            set {
                _redCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        3, // ChestItem, best I could do
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) {
                        return;
                    }
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
                    if (cyclePosition is null) {
                        return;
                    }
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
            set {
                _blueCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        31, // Count (whatever that is)
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) {
                        return;
                    }
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
            set {
                _yellowCleared = value;
                if (value) {
                    cyclePosition = new LLNode<int>(
                        15, // ShopItem; ok it's not yellow but it's the best I've got
                        cyclePosition?.getEnd,
                        cyclePosition
                    );
                    cycleTimer = 30;
                } else {
                    if (cyclePosition is null) {
                        return;
                    }
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
                    if (cyclePosition is null) {
                        return;
                    }
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
        internal bool isMarked;

        public BoardSlot(int index, Goal goal) {
            this.index = index;

            displayItem = goal.icon;
            this.goal = goal;
            this.isMarked = false;

            Top.Set((index / 5) * (TextureAssets.InventoryBack9.Value.Height * Main.UIScale + 4) + 2, 0f);
            Left.Set((index % 5) * (TextureAssets.InventoryBack9.Value.Width * Main.UIScale + 4) + 2, 0f);
            Width.Set(TextureAssets.InventoryBack9.Value.Width * Main.UIScale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Value.Height * Main.UIScale, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            var dims = this.GetDimensions();
            Vector2 origin = dims.Center();
            var possibleColours = new List<Color>();
            if (this.redCleared) { possibleColours.Add(Color.Red); }
            if (this.greenCleared) { possibleColours.Add(Color.Green); }
            if (this.blueCleared) { possibleColours.Add(Color.Blue); }
            if (this.yellowCleared) { possibleColours.Add(Color.Yellow); }
            if (this.pinkCleared) { possibleColours.Add(Color.Pink); }
            if (possibleColours.Count == 0) {
                possibleColours.Add(Color.Gray);
            }
            drawRectangle(spriteBatch, this.GetDimensions().ToRectangle(), possibleColours[(int)((Main.GameUpdateCount / 30) % possibleColours.Count)]);
            Main.DrawItemIcon(spriteBatch, displayItem, origin, Color.White, this.GetDimensions().Width - 8);
            if (this.isMarked) {
                var starTexture = TextureAssets.Item[ItemID.FallenStar].Value;
                var starAnim = Main.itemAnimations[ItemID.FallenStar];
                spriteBatch.Draw(starTexture, new Rectangle((int) origin.X + 8, (int) origin.Y + 8, 16, 16),
                    starAnim.GetFrame(starTexture),
                    Color.White);
            }
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

        public override void RightMouseDown(UIMouseEvent evt) {
            this.isMarked = !this.isMarked;
        }

        public override void MouseOver(UIMouseEvent evt) {
            BingoBoardSystem.mouseText = Language.GetTextValue(goal.description);
        }

        public override void MouseOut(UIMouseEvent evt) {
            BingoBoardSystem.mouseText = "";
        }
    }
}
