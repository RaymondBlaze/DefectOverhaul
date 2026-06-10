using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Cost: 2(1) -> 1
///     </para>
///     <para>
///         Effect -> (Innate.) The first time you play a 0[Energy] Attack each
///         turn, return it to your Hand.
///     </para>
/// </summary>
[CardPatch(nameof(Feral))]
public static class FeralPatch {
    public sealed class FeralConstructor : IPatchMethod {
        public static string PatchId => "Feral.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Feral), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, 1);
        }
    }

    public sealed class FeralOnUpgrade : IPatchMethod {
        public static string PatchId => "Feral.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Feral), "OnUpgrade")];
        }

        public static bool Prefix(Feral __instance) {
            __instance.AddKeyword(CardKeyword.Innate);
            return false;
        }
    }
}