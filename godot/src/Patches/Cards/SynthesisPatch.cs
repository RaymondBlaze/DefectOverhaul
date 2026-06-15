using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 14(20) damage. A random card in your Hand is free to play this turn.
///     </para>
/// </summary>
[CardPatch(nameof(Synthesis))]
public static class SynthesisPatch {
    public sealed class SynthesisOnPlay : IPatchMethod {
        public static string PatchId => "Synthesis.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Synthesis), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            Synthesis __instance,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(Synthesis card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd
                .Attack(card.DynamicVars.Damage.BaseValue)
                .FromCard(card)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            var combatCardSelection = card.Owner.RunState.Rng.CombatCardSelection;
            var cards = PileType.Hand.GetPile(card.Owner).Cards;
            var list = cards.Where(c => c.EnergyCost.GetWithModifiers(CostModifiers.None) > 0 || c.BaseStarCost > 0).ToList();
            var selected = combatCardSelection.NextItem(list.Where(c => c.CostsEnergyOrStars(true)));
            selected ??= combatCardSelection.NextItem(cards.Where(c => c.CostsEnergyOrStars(true)));
            selected ??= combatCardSelection.NextItem(list);
            selected ??= combatCardSelection.NextItem(cards);
            selected?.SetToFreeThisTurn();
        }
    }
}