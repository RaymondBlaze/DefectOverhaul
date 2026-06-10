using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 8(11) damage. Channel 1 Frost.
///     </para>
/// </summary>
[CardPatch(nameof(ColdSnap))]
public static class ColdSnapPatch {
    public sealed class ColdSnapCanonicalVars : IPatchMethod {
        public static string PatchId => "ColdSnap.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(ColdSnap), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new DamageVar(8M, ValueProp.Move)];
            return false;
        }
    }
}