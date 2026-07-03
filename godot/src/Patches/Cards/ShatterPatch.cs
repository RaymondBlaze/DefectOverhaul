using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Cost: 1 -> 2
///     </para>
///     <para>
///         Effect -> Deal 14(20) damage to ALL enemies. Evoke all of your Orbs twice.
///     </para>
/// </summary>
[CardPatch(nameof(Shatter))]
public static class ShatterPatch {
    public sealed class ShatterConstructor : IPatchMethod {
        public static string PatchId => "Shatter.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Shatter), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, 2);
        }
    }

    public sealed class ShatterCanonicalKeywords : IPatchMethod {
        public static string PatchId => "Shatter.CanonicalKeywords";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Shatter), "CanonicalKeywords", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<CardKeyword> __result) {
            __result = [];
            return false;
        }
    }

    public sealed class ShatterCanonicalVars : IPatchMethod {
        public static string PatchId => "Shatter.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Shatter), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new DamageVar(14, ValueProp.Move)];
            return false;
        }
    }

    public sealed class ShatterOnUpgrade : IPatchMethod {
        public static string PatchId => "Shatter.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Shatter), "OnUpgrade")];
        }

        public static bool Prefix(Shatter __instance) {
            __instance.DynamicVars.Damage.UpgradeValueBy(6);
            return false;
        }
    }
}