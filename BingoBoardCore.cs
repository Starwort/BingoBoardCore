global using static BingoBoardCore.Util.DrawingHelper;
using BingoBoardCore.AnimationHelpers;
using BingoBoardCore.Common;
using BingoBoardCore.Common.Configs;
using BingoBoardCore.Common.Systems;
using BingoBoardCore.Icons;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BingoBoardCore {
    public class BingoBoardCore : Mod {
        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoBoardCore";

        // Re-export in case any content plugins want it more easily
        public static uint animationPeriod => ModContent.GetInstance<BingoBoardUIConfig>().animationPeriod;

        public static bool registerGoal(Item icon, string description, string id, int difficultyTier, string[] synergyTypes, Func<BingoMode, int, bool> shouldEnable = null!, string iconText = "", Item? modifierIcon = null) {
            shouldEnable ??= Goal.alwaysInclude;
            ModContent.GetInstance<BingoBoardSystem>().addGoal(new(
                icon,
                description,
                id,
                difficultyTier,
                synergyTypes,
                shouldEnable,
                iconText,
                modifierIcon
            ));
            return true;
        }

        public static bool triggerGoal(string id, Player player) {
            ModContent.GetInstance<BingoBoardSystem>().triggerGoal(
                id, (Team) player.team
            );
            return true;
        }

        public static bool untriggerGoal(string id, Player player) {
            ModContent.GetInstance<BingoBoardSystem>().untriggerGoal(
                id, (Team) player.team
            );
            return true;
        }

        // Useful for 'never do X' goals, to trigger them at the beginning of the game
        public static bool onGameStart(Action callback) {
            ModContent.GetInstance<BingoBoardSystem>().gameStartCallbacks.Add(callback);
            return true;
        }

        internal static bool shouldReportProgress(string goalId) {
            var system = ModContent.GetInstance<BingoBoardSystem>();
            var goalState = system.activeGoals?.Where(goalState => goalState.goal.id == goalId).First();
            if (goalState is null) {
                return false;
            }
            if (system.mode != BingoMode.Lockout) {
                return true;
            }
            return goalState.packedClear == 0;
        }

        public static void reportProgress(string goalId, string progressText, params string[] substitutions) {
            if (Main.netMode != NetmodeID.Server) {
                var player = Main.player[Main.myPlayer];
                if (shouldReportProgress(goalId)) {
                    PopupText.NewText(new AdvancedPopupRequest() {
                        Text = Language.GetTextValue(progressText, substitutions.Select(subsitution => Language.GetTextValue(subsitution)).ToArray()),
                        DurationInFrames = 60,
                        Velocity = -7 * Vector2.UnitY,
                        Color = Color.Green,
                    }, player.Center);
                }
            }
        }

        // E.G., when a goal can no longer be achieved in the current attempt (such as dealing disallowed damage to a boss)
        public static void reportBadProgress(string goalId, string progressText, params string[] substitutions) {
            if (Main.netMode != NetmodeID.Server) {
                var player = Main.player[Main.myPlayer];
                if (shouldReportProgress(goalId)) {
                    PopupText.NewText(new AdvancedPopupRequest() {
                        Text = Language.GetTextValue(progressText, substitutions.Select(subsitution => Language.GetTextValue(subsitution)).ToArray()),
                        DurationInFrames = 60,
                        Velocity = -7 * Vector2.UnitY,
                        Color = Color.Red,
                    }, player.Center);
                }
            }
        }

        public static Item registerCycleAnimation(int[] itemIds) {
            Item rv = new(itemIds[0]);
            ModContent.GetInstance<IconAnimationSystem>().seqAnimations.Add((rv, itemIds));
            return rv;
        }

        public static Item registerRandAnimation(int[] itemIds) {
            Item rv = new(itemIds[0]);
            ModContent.GetInstance<IconAnimationSystem>().randAnimations.Add((rv, itemIds));
            return rv;
        }

        private object dispatch(object[] args, string[] functions) {
            var type = this.GetType();
            var allMethodInfo = functions.ToDictionary(name => name, name => {
                var methInfo = type.GetMethod(name);
                if (methInfo is null) {
                    throw new InvalidOperationException($"INTERNAL ERROR: {name} did not exist!");
                }
                return methInfo;
            });
            if (args[0] is not string meth) {
                throw new ArgumentException("The first parameter to a BingoBoardCore ModCall must be the method to invoke");
            }
            MethodInfo methInfo;
            if (!allMethodInfo.TryGetValue(meth, out methInfo!)) {
                throw new ArgumentException($"Unknown method '{meth}'");
            }
            var parameters = methInfo.GetParameters();
            var i = 0;
            for (; i < args.Length; i++) {
                var expectedType = parameters[i].ParameterType;
                var actualType = args[i].GetType();
                if (expectedType != actualType) {
                    throw new ArgumentException($"Parameter '{parameters[i].Name}': Expected type '{expectedType}', found type '{actualType}'");
                }
            }
            for (; i < parameters.Length; i++) {
                var paramInfo = parameters[i];
                if (!paramInfo.HasDefaultValue) {
                    throw new ArgumentException($"Missing value for non-default argument '{paramInfo.Name}'");
                }
            }
            return methInfo.Invoke(this, args)!;
        }

        public override object Call(params object[] args) {
            return dispatch(args, new[] {
                nameof(registerGoal),
                nameof(triggerGoal),
                nameof(untriggerGoal),
                nameof(onGameStart),
                nameof(reportProgress),
                nameof(reportBadProgress),
                nameof(registerCycleAnimation),
                nameof(registerRandAnimation),
            });
        }

        private static Item? _dieIcon;
        public static Item dieIcon {
            get {
                _dieIcon ??= ModContent.GetInstance<Die>().Item;
                return _dieIcon;
            }
        }

        private static Item? _disallowIcon;
        public static Item disallowIcon {
            get {
                _disallowIcon ??= ModContent.GetInstance<Disallow>().Item;
                return _disallowIcon;
            }
        }
    }
}