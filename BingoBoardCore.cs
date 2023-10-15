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
                system.addGoal(new(
                    new Item(ItemID.Zenith),
                    $"Test {i}",
                    $"BingoBoardCore.TestGoal{i}",
                    // never include the 25 test goals
                    (_, _) => false
                ));
            }
        }

        public override object Call(params object[] args) {
            if (args[0] is string meth) {
                switch (meth) {
                    case "registerGoal":
                        Func<BingoMode, int, bool> shouldEnable = Goal.alwaysInclude;
                        if (args.Length > 4) {
                            shouldEnable = (Func<BingoMode, int, bool>) args[4];
                        }
                        ModContent.GetInstance<BingoBoardSystem>().addGoal(new(
                            (Item) args[1],
                            (string) args[2],
                            (string) args[3],
                            shouldEnable
                        ));
                        return true;
                }
                return null!;
            } else {
                return null!;
            }
        }
    }
}