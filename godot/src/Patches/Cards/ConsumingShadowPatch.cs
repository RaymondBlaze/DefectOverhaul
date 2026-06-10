using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Channel 2(3) Dark. Whenever you Evoke a Dark Orb, trigger the passive ability of all other Dark Orbs.
///     </para>
/// </summary>
[CardPatch(nameof(ConsumingShadow))]
public static class ConsumingShadowPatch {
    public sealed class ConsumingShadowCanonicalVars : IPatchMethod {
        public static string PatchId => "ConsumingShadow.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(ConsumingShadow), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new RepeatVar(2),
                new PowerVar<ConsumingShadowPower>(1)
            ];
            return false;
        }
    }

    public sealed class ConsumingShadowPowerAfterSideTurnEnd : IPatchMethod {
        public static string PatchId => "ConsumingShadowPower.AfterSideTurnEnd";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(ConsumingShadowPower), nameof(ConsumingShadowPower.AfterSideTurnEnd))];
        }

        public static bool Prefix(ref Task __result) {
            __result = Task.CompletedTask;
            return false;
        }
    }

    [RegisterSingleton]
    public sealed class DefectOverhaulConsumingShadowCombatHooks() : HookedSingletonModel(HookType.Combat) {
        public override async Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets) {
            if (orb is not DarkOrb evokedDarkOrb) return;
            var player = orb.Owner;
            var darkOrbs = player.PlayerCombatState!.OrbQueue.Orbs.OfType<DarkOrb>();
            var power = player.Creature.GetPower<ConsumingShadowPower>();
            if (power == null) return;
            foreach (var darkOrb in darkOrbs) {
                if (darkOrb == evokedDarkOrb) continue;
                for (var i = 0; i < power.Amount; i++) await OrbCmd.Passive(choiceContext, darkOrb, null);
            }
        }
    }
}