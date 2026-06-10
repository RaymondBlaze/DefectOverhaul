using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 3(5) damage for each 0[Energy] card played
///         this combat.
///     </para>
/// </summary>
[CardPatch(nameof(HelixDrill))]
public static class HelixDrillPatch {
    public sealed class HelixDrillCanonicalVars : IPatchMethod {
        public static string PatchId => "HelixDrill.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(HelixDrill), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new DamageVar(3, ValueProp.Move),
                new CalculationBaseVar(0),
                new CalculationExtraVar(1),
                new CalculatedVar(HelixDrill._calculatedHitsKey).WithMultiplier((card, _) =>
                    CombatManager.Instance.History.Entries
                        .OfType<CardPlayFinishedEntry>()
                        .Count(entry => entry.CardPlay.Card.Owner == card.Owner &&
                                        entry.CardPlay.Resources.EnergyValue == 0
                        )
                )
            ];
            return false;
        }
    }
}