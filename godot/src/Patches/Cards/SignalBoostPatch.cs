using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Innate. The next Power you play is played an additional time. Exhaust.
///     </para>
/// </summary>
[CardPatch(nameof(SignalBoost))]
public static class SignalBoostPatch {
    public sealed class SignalBoostCanonicalKeywords : IPatchMethod {
        public static string PatchId => "SignalBoost.CanonicalKeywords";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(SignalBoost), "CanonicalKeywords", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<CardKeyword> __result) {
            __result = [CardKeyword.Innate, CardKeyword.Exhaust];
            return false;
        }
    }
}