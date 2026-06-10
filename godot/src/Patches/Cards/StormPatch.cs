using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> (Innate.) Whenever you play a Power, Channel 2 Lightning.
///     </para>
/// </summary>
[CardPatch(nameof(Storm))]
public static class StormPatch {
    public sealed class StormCanonicalVars : IPatchMethod {
        public static string PatchId => "Storm.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Storm), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new PowerVar<StormPower>(2)];
            return false;
        }
    }

    public sealed class StormOnUpgrade : IPatchMethod {
        public static string PatchId => "Storm.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Storm), "OnUpgrade")];
        }

        public static bool Prefix(Storm __instance) {
            __instance.AddKeyword(CardKeyword.Innate);
            return false;
        }
    }
}