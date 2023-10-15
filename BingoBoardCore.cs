global using static BingoBoardCore.Util.DrawingHelper;
using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using BingoBoardCore.UI;
using BingoBoardCore.UI.States;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.Chat;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BingoBoardCore {
    public class BingoBoardCore : Mod {
        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoBoardCore";

        public override void Load() {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            for (int i = 0; i < 25; i++) {
                system.addGoal(new Goal(
                    new Item(ItemID.Zenith),
                    $"Test {i}",
                    $"BingoBoardCore.TestGoal{i}"
                ));
            }
        }
    }
}