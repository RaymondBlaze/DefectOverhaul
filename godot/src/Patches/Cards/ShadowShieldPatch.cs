using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Gain 12(16) Block. Channel 1 Dark.
///     </para>
/// </summary>
[CardPatch(nameof(ShadowShield))]
public static class ShadowShieldPatch {
    public sealed class ShadowShieldCanonicalVars : IPatchMethod {
        public static string PatchId => "ShadowShield.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(ShadowShield), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new BlockVar(12m, ValueProp.Move)];
            return false;
        }
    }
}