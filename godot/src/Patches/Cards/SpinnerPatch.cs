using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Rarity: Rare -> Uncommon
///     </para>
/// </summary>
[CardPatch(nameof(Spinner))]
public static class SpinnerPatch {
    public sealed class SpinnerConstructor : IPatchMethod {
        public static string PatchId => "Spinner.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Spinner), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, rarity: CardRarity.Uncommon);
        }
    }
}