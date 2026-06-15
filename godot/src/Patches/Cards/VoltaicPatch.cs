using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Cost: 3 -> 3(2)
///     </para>
///     <para>
///         Effect -> Channel Lightning equal to the Lightning already Channeled this combat.
///     </para>
/// </summary>
[CardPatch(nameof(Voltaic))]
public static class VoltaicPatch {
    public sealed class VoltaicCanonicalKeywords : IPatchMethod {
        public static string PatchId => "Voltaic.CanonicalKeywords";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Voltaic), "CanonicalKeywords", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<CardKeyword> __result) {
            __result = [];
            return false;
        }
    }

    public sealed class VoltaicOnUpgrade : IPatchMethod {
        public static string PatchId => "Voltaic.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Voltaic), "OnUpgrade")];
        }

        public static bool Prefix(Voltaic __instance) {
            __instance.EnergyCost.UpgradeBy(-1);
            return false;
        }
    }
}