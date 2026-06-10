using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 18(23) damage. Transform a card in your Discard Pile into a 0[Energy] copy of this card.
///     </para>
/// </summary>
[CardPatch(nameof(AdaptiveStrike))]
public static class AdaptiveStrikePatch {
    public sealed class AdaptiveStrikeCanonicalVars : IPatchMethod {
        public static string PatchId => "AdaptiveStrike.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(AdaptiveStrike), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new DamageVar(18, ValueProp.Move),
                new CardsVar(1)
            ];
            return false;
        }
    }

    public sealed class AdaptiveStrikeOnPlay : IPatchMethod {
        public static string PatchId => "AdaptiveStrike.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(AdaptiveStrike), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            AdaptiveStrike __instance,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(AdaptiveStrike card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
            var selectedCards = (await CardSelectCmd.FromCombatPile(
                choiceContext,
                PileType.Discard.GetPile(card.Owner),
                card.Owner,
                new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, card.DynamicVars.Cards.IntValue)
            )).ToList();
            foreach (var selectedCard in selectedCards) {
                var clone = card.CreateClone();
                clone.EnergyCost.SetThisCombat(0);
                await CardCmd.Transform(selectedCard, clone);
            }
        }
    }
}