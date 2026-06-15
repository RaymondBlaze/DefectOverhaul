using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> At the start of your turn, choose 1 of 3 random Power cards to add into your Hand.
///     </para>
/// </summary>
[CardPatch(nameof(CreativeAi))]
public static class CreativeAiPatch {
    public sealed class CreativeAiPowerBeforeHandDraw : IPatchMethod {
        public static string PatchId => "CreativeAi.BeforeHandDraw";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(CreativeAiPower), "BeforeHandDraw")];
        }

        public static bool Prefix(ref Task __result, CreativeAiPower __instance, Player player, PlayerChoiceContext choiceContext) {
            __result = BeforeHandDraw(__instance, player, choiceContext);
            return false;
        }

        private static async Task BeforeHandDraw(CreativeAiPower power, Player player, PlayerChoiceContext choiceContext) {
            if (!CardPatches.IsCardPatched<CreativeAi>()) return;
            if (player != power.Owner.Player)
                return;
            for (var i = 0; i < power.Amount; ++i) {
                var list = CardFactory
                    .GetDistinctForCombat(
                        player,
                        player.Character.CardPool
                            .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
                            .Where(c => c.Type == CardType.Power),
                        3,
                        player.RunState.Rng.CombatCardGeneration
                    )
                    .ToList();
                var card = await CardSelectCmd.FromChooseACardScreen(choiceContext, list, player, true);
                if (card == null)
                    return;
                await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, card.Owner);
            }
        }
    }
}