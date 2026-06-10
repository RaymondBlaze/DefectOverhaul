using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Gain 8(11) Block. Reduce this card's cost to 0[Energy].
///     </para>
/// </summary>
[CardPatch(nameof(Leap))]
public static class LeapPatch {
    public sealed class LeapCanonicalVars : IPatchMethod {
        public static string PatchId => "Leap.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Leap), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [new BlockVar(8, ValueProp.Move)];
            return false;
        }
    }

    public sealed class LeapOnPlay : IPatchMethod {
        public static string PatchId => "Leap.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Leap), "OnPlay")];
        }

        public static void Postfix(ref Task __result, Leap __instance) {
            __result = OnPlay(__result, __instance);
        }

        private static async Task OnPlay(Task task, Leap card) {
            await task;
            card.EnergyCost.SetThisCombat(0);
        }
    }
}