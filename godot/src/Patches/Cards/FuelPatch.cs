using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Gain 1 Energy. Draw 1(2) card.
///     </para>
/// </summary>
[CardPatch(nameof(Fuel))]
public static class FuelPatch {
    public sealed class FuelCanonicalVars : IPatchMethod {
        public static string PatchId => "Fuel.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Fuel), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new EnergyVar(1),
                new CardsVar(1)
            ];
            return false;
        }
    }

    public sealed class FuelOnPlay : IPatchMethod {
        public static string PatchId => "Fuel.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Fuel), "OnPlay")];
        }

        public static bool Prefix(ref Task __result, Fuel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(Fuel card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            await PlayerCmd.GainEnergy(card.DynamicVars.Energy.BaseValue, card.Owner);
            await CardPileCmd.Draw(choiceContext, card.DynamicVars.Cards.BaseValue, card.Owner);
        }
    }

    public sealed class FuelOnUpgrade : IPatchMethod {
        public static string PatchId => "Fuel.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Fuel), "OnUpgrade")];
        }

        public static bool Prefix(Fuel __instance) {
            __instance.DynamicVars.Cards.UpgradeValueBy(1);
            return false;
        }
    }
}