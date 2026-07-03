using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 5(8) damage. Put a random 0[Energy] card from your Draw Pile into your Hand.
///     </para>
/// </summary>
[CardPatch(nameof(Ftl))]
public static class FtlPatch {
    public sealed class FtlShouldGlowGoldInternal : IPatchMethod {
        public static string PatchId => "Ftl.ShouldGlowGoldInternal";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Ftl), "ShouldGlowGoldInternal", MethodType.Getter)];
        }

        public static bool Prefix(ref bool __result) {
            __result = false;
            return false;
        }
    }

    public sealed class FtlOnUpgrade : IPatchMethod {
        public static string PatchId => "Ftl.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Ftl), "OnUpgrade")];
        }

        public static bool Prefix(Ftl __instance) {
            __instance.DynamicVars.Damage.UpgradeValueBy(3);
            return false;
        }
    }

    public sealed class FtlOnPlay : IPatchMethod {
        public static string PatchId => "Ftl.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Ftl), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            Ftl __instance,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(Ftl card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd
                .Attack(card.DynamicVars.Damage.BaseValue)
                .FromCard(card, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            var cardsToAdd = PileType.Draw
                .GetPile(card.Owner).Cards
                .Where(Filter)
                .ToList()
                .UnstableShuffle(card.Owner.RunState.Rng.CombatCardSelection)
                .Take(card.DynamicVars.Cards.IntValue);
            foreach (var c in cardsToAdd) await CardPileCmd.Add(c, PileType.Hand);
        }

        private static bool Filter(CardModel card) {
            return card.EnergyCost.GetWithModifiers(CostModifiers.All) == 0 &&
                   !card.EnergyCost.CostsX &&
                   card.Type is CardType.Attack or CardType.Skill or CardType.Power;
        }
    }
}