using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Cost: 1 -> 2(1)
///     </para>
///     <para>
///         Rarity: Uncommon -> Rare
///     </para>
///     <para>
///         Effect -> At the end of your turn, trigger the passive ability of all your Orbs 1 additional time.
///     </para>
/// </summary>
[CardPatch(nameof(Loop))]
public static class LoopPatch {
    public sealed class LoopConstructor : IPatchMethod {
        public static string PatchId => "Loop.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Loop), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, 2, rarity: CardRarity.Rare);
        }
    }

    public sealed class LoopOnUpgrade : IPatchMethod {
        public static string PatchId => "Loop.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Loop), "OnUpgrade")];
        }

        public static bool Prefix(Loop __instance) {
            __instance.EnergyCost.UpgradeBy(-1);
            return false;
        }
    }

    public sealed class LoopPowerAfterPlayerTurnStart : IPatchMethod {
        public static string PatchId => "LoopPower.AfterPlayerTurnStart";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(LoopPower), nameof(LoopPower.AfterPlayerTurnStart))];
        }

        public static bool Prefix(ref Task __result) {
            __result = Task.CompletedTask;
            return false;
        }
    }

    [RegisterSingleton]
    public sealed class DefectOverhaulLoopCombatHooks() : HookedSingletonModel(HookType.Combat) {
        public override int ModifyOrbPassiveTriggerCounts(OrbModel orb, int triggerCount) {
            var power = orb.Owner.Creature.GetPower<LoopPower>();
            if (power != null) triggerCount += power.Amount;
            return triggerCount;
        }
    }
}