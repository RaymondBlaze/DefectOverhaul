using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 6 damage twice. Play 1(2) random Attacks from your Draw Pile.
///     </para>
/// </summary>
[CardPatch(nameof(Uproar))]
public static class UproarPatch {
    public sealed class UproarCanonicalVars : IPatchMethod {
        public static string PatchId => "Uproar.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Uproar), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new DamageVar(6, ValueProp.Move),
                new CardsVar(1)
            ];
            return false;
        }
    }

    public sealed class UproarOnUpgrade : IPatchMethod {
        public static string PatchId => "Uproar.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Uproar), "OnUpgrade")];
        }

        public static bool Prefix(Uproar __instance) {
            __instance.DynamicVars.Cards.UpgradeValueBy(1);
            return false;
        }
    }

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
            await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue).FromCard(card).WithHitCount(2)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            for (var i = 0; i < card.DynamicVars.Cards.IntValue; i++) {
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
                if (cardModel != null) await CardCmd.AutoPlay(choiceContext, cardModel, null);
            }
        }
    }
}