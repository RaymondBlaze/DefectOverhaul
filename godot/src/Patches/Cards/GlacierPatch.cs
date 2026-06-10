using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Gain 7(10) Block. Channel 2 Frost.
///     </para>
/// </summary>
[CardPatch(nameof(Glacier))]
public static class GlacierPatch {
    public sealed class GlacierCanonicalVars : IPatchMethod {
        public static string PatchId => "Glacier.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Glacier), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new BlockVar(7, ValueProp.Move)];
            return false;
        }
    }
}