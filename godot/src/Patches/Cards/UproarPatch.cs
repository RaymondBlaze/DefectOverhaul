using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 6(8) damage twice. Play 1 random Attack from your Draw Pile (against the enemy).
///     </para>
/// </summary>
[CardPatch(nameof(Uproar))]
public static class UproarPatch {
    public sealed class UproarOnPlay : IPatchMethod {
        public static string PatchId => "Uproar.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Uproar), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            Uproar __instance,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(Uproar card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
                .FromCard(card, cardPlay)
                .WithHitCount(2)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            var cardModel = PileType.Draw
                .GetPile(card.Owner).Cards
                .Where(c => c.Type == CardType.Attack && !c.Keywords.Contains(CardKeyword.Unplayable))
                .ToList()
                .StableShuffle(card.Owner.RunState.Rng.Shuffle)
                .FirstOrDefault();
            cardModel ??= PileType.Draw.GetPile(card.Owner).Cards
                .Where(c => c.Type == CardType.Attack)
                .ToList()
                .StableShuffle(card.Owner.RunState.Rng.Shuffle)
                .FirstOrDefault();
            if (cardModel != null)
                await CardCmd.AutoPlay(choiceContext, cardModel, card.IsUpgraded ? cardPlay.Target : null);
        }
    }
}