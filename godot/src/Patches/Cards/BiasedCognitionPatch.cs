using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Gain 4(5) Focus. At the end of your turn, if you didn't gain Focus this turn, lose 1 Focus.
///     </para>
/// </summary>
[CardPatch(nameof(BiasedCognition))]
public static class BiasedCognitionPatch {
    public sealed class BiasedCognitionPowerAfterSideTurnStart : IPatchMethod {
        public static string PatchId => "BiasedCognitionPower.AfterSideTurnStart";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(BiasedCognitionPower), nameof(BiasedCognitionPower.AfterSideTurnStart))];
        }

        public static bool Prefix(ref Task __result) {
            __result = Task.CompletedTask;
            return false;
        }
    }

    [RegisterSingleton]
    public sealed class DefectOverhaulBiasedCognitionCombatHooks() : HookedSingletonModel(HookType.Combat) {
        public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb) {
            if (!CardPatches.IsCardPatched<BiasedCognition>()) return;
            var power = orb.Owner.Creature.GetPower<BiasedCognitionPower>();
            if (power == null) return;
            power.Flash();
            await CreatureCmd.GainBlock(orb.Owner.Creature, power.Amount, ValueProp.Unpowered, null);
        }

        public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants) {
            if (!CardPatches.IsCardPatched<BiasedCognition>()) return;
            foreach (var creature in participants) {
                var power = creature.GetPower<BiasedCognitionPower>();
                if (power == null) continue;
                var gainedFocus = CombatManager.Instance.History.Entries
                    .OfType<PowerReceivedEntry>()
                    .Any(entry => entry.HappenedThisTurn(creature.CombatState) && entry.Power is FocusPower && entry.Power.Owner == creature);
                if (gainedFocus) continue;
                power.Flash();
                await PowerCmd.Apply<FocusPower>(new ThrowingPlayerChoiceContext(), creature, -power.Amount, creature, null);
            }
        }
    }
}