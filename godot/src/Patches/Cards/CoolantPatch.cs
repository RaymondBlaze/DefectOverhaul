using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
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
///         Rarity: Rare -> Uncommon
///     </para>
///     <para>
///         Effect -> Whenever you Channel an Orb, gain 3(4) Block.
///     </para>
/// </summary>
[CardPatch(nameof(Coolant))]
public class CoolantPatch {
    public sealed class CoolantConstructor : IPatchMethod {
        public static string PatchId => "Coolant.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Coolant), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, rarity: CardRarity.Uncommon);
        }
    }

    public sealed class CoolantExtraHoverTips : IPatchMethod {
        public static string PatchId => "Coolant.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Coolant), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result) {
            __result = [
                HoverTipFactory.Static(StaticHoverTip.Channeling),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ];
            return false;
        }
    }

    public sealed class CoolantPowerAfterSideTurnStart : IPatchMethod {
        public static string PatchId => "CoolantPower.AfterSideTurnStart";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(CoolantPower), nameof(CoolantPower.AfterSideTurnStart))];
        }

        public static bool Prefix(ref Task __result) {
            __result = Task.CompletedTask;
            return false;
        }
    }

    public sealed class CoolantPowerExtraHoverTips : IPatchMethod {
        public static string PatchId => "CoolantPower.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(CoolantPower), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result) {
            __result = [
                HoverTipFactory.Static(StaticHoverTip.Channeling),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ];
            return false;
        }
    }

    [RegisterSingleton]
    public sealed class DefectOverhaulCoolantCombatHooks() : HookedSingletonModel(HookType.Combat) {
        public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb) {
            if (!CardPatches.IsCardPatched<Coolant>()) return;
            var power = orb.Owner.Creature.GetPower<CoolantPower>();
            if (power == null) return;
            power.Flash();
            await CreatureCmd.GainBlock(orb.Owner.Creature, power.Amount, ValueProp.Unpowered, null);
        }
    }
}