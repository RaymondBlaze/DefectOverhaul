using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Cost: 3 -> 2
///     </para>
///     <para>
///         Effect -> Deal 4(7) damage twice. Channel 2 Glass.
///     </para>
/// </summary>
[CardPatch(nameof(Refract))]
public static class RefractPatch {
    public sealed class RefractConstructor : IPatchMethod {
        public static string PatchId => "Refract.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Refract), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, 2);
        }
    }

    public sealed class RefractCanonicalVars : IPatchMethod {
        public static string PatchId => "Refract.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Refract), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new RepeatVar(2),
                new DamageVar(4, ValueProp.Move)
            ];
            return false;
        }
    }
}