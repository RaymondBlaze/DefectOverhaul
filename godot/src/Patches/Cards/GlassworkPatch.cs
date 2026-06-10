using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Gain 6(9) Block. Channel 1 Glass.
///     </para>
/// </summary>
[CardPatch(nameof(Glasswork))]
public static class GlassworkPatch {
    public sealed class GlassworkCanonicalVars : IPatchMethod {
        public static string PatchId => "Glasswork.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Glasswork), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new BlockVar(6, ValueProp.Move)];
            return false;
        }
    }
}