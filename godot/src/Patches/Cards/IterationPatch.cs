using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Whenever you draw a Status, draw 1(2) cards.
///     </para>
/// </summary>
[CardPatch(nameof(Iteration))]
public static class IterationPatch {
    public sealed class IterationCanonicalVars : IPatchMethod {
        public static string PatchId => "Iteration.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Iteration), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new PowerVar<IterationPower>(1)];
            return false;
        }
    }

    public sealed class IterationPowerAfterCardDrawn : IPatchMethod {
        public static string PatchId => "IterationPower.AfterCardDrawn";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(IterationPower), nameof(IterationPower.AfterCardDrawn))];
        }

        public static bool Prefix(
            ref Task __result,
            IterationPower __instance,
            PlayerChoiceContext choiceContext,
            CardModel card
        ) {
            __result = AfterCardDrawn(__instance, choiceContext, card);
            return false;
        }

        private static async Task AfterCardDrawn(
            IterationPower power,
            PlayerChoiceContext choiceContext,
            CardModel card
        ) {
            if (card.Owner.Creature != power.Owner || power.Owner.Player == null || card.Type != CardType.Status)
                return;
            power.Flash();
            await CardPileCmd.Draw(choiceContext, power.Amount, power.Owner.Player);
        }
    }
}