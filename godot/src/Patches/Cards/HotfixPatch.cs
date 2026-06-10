using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Gain 2(3) Focus this turn. Increase this card's cost by 1[Energy] this turn.
///     </para>
/// </summary>
[CardPatch(nameof(Hotfix))]
public static class HotfixPatch {
    public sealed class HotfixCanonicalKeywords : IPatchMethod {
        public static string PatchId => "Hotfix.CanonicalKeywords";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hotfix), "CanonicalKeywords", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<CardKeyword> __result) {
            __result = [];
            return false;
        }
    }

    public sealed class HotfixCanonicalVars : IPatchMethod {
        public static string PatchId => "Hotfix.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hotfix), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new PowerVar<FocusPower>(2),
                new EnergyVar(1)
            ];
            return false;
        }
    }

    public sealed class HotfixExtraHoverTips : IPatchMethod {
        public static string PatchId => "Hotfix.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hotfix), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result, Hotfix __instance) {
            __result = [
                HoverTipFactory.FromPower<FocusPower>(),
                HoverTipFactory.ForEnergy(__instance)
            ];
            return false;
        }
    }

    public sealed class HotfixOnUpgrade : IPatchMethod {
        public static string PatchId => "Hotfix.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hotfix), "OnUpgrade")];
        }

        public static bool Prefix(Hotfix __instance) {
            __instance.DynamicVars[nameof(FocusPower)].UpgradeValueBy(1);
            return false;
        }
    }

    public sealed class HotfixOnPlay : IPatchMethod {
        public static string PatchId => "Hotfix.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hotfix), "OnPlay")];
        }

        public static void Postfix(ref Task __result, Hotfix __instance) {
            __result = OnPlay(__result, __instance);
        }

        private static async Task OnPlay(Task task, Hotfix card) {
            await task;
            card.EnergyCost.AddThisTurn(1);
        }
    }
}