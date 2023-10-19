global using static BingoBoardCore.Util.DrawingHelper;
using BingoBoardCore.Common;
using BingoBoardCore.Common.Systems;
using System;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;

namespace BingoBoardCore {
    public class BingoBoardCore : Mod {
        public static string GithubUserName => "Starwort";
        public static string GithubProjectName => "BingoBoardCore";

        private static bool registerGoal(Item icon, string description, string id, int difficultyTier, string[] synergyTypes, Func<BingoMode, int, bool> shouldEnable = null!) {
            shouldEnable ??= Goal.alwaysInclude;
            ModContent.GetInstance<BingoBoardSystem>().addGoal(new(
                icon,
                description,
                id,
                difficultyTier,
                synergyTypes,
                shouldEnable
            ));
            return true;
        }

        private static bool triggerGoal(string id, Player player) {
            ModContent.GetInstance<BingoBoardSystem>().triggerGoal(
                id, (Team) player.team
            );
            return true;
        }

        private static bool untriggerGoal(string id, Player player) {
            ModContent.GetInstance<BingoBoardSystem>().untriggerGoal(
                id, (Team) player.team
            );
            return true;
        }

        // Useful for 'never do X' goals, to trigger them at the beginning of the game
        private static bool onGameStart(Func<object> callback) {
            ModContent.GetInstance<BingoBoardSystem>().gameStartCallbacks.Add(callback);
            return true;
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
            });
        }
    }
}