using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Cost: 1 -> 2
///     </para>
///     <para>
///         Rarity: Uncommon -> Rare
///     </para>
///     <para>
///         Effect -> Channel 2(3) Frost. Whenever you gain Block from a
///         Frost Orb, deal that much damage to a random enemy.
///     </para>
/// </summary>
[CardPatch(nameof(Hailstorm))]
public static class HailstormPatch {
    public sealed class HailstormConstructor : IPatchMethod {
        public static string PatchId => "Hailstorm.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hailstorm), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, 2, rarity: CardRarity.Rare);
        }
    }

    public sealed class HailstormCanonicalVars : IPatchMethod {
        public static string PatchId => "Hailstorm.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hailstorm), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new RepeatVar(2),
                new PowerVar<HailstormPower>(1)
            ];
            return false;
        }
    }

    public sealed class HailstormExtraHoverTips : IPatchMethod {
        public static string PatchId => "Hailstorm.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hailstorm), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result) {
            __result = [
                HoverTipFactory.Static(StaticHoverTip.Block),
                HoverTipFactory.FromOrb<FrostOrb>()
            ];
            return false;
        }
    }

    public sealed class HailstormOnPlay : IPatchMethod {
        public static string PatchId => "Hailstorm.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hailstorm), "OnPlay")];
        }

        public static bool Prefix(ref Task __result, Hailstorm __instance, PlayerChoiceContext choiceContext) {
            __result = OnPlay(__instance, choiceContext);
            return false;
        }

        private static async Task OnPlay(Hailstorm card, PlayerChoiceContext choiceContext) {
            await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);
            for (var i = 0; i < card.DynamicVars.Repeat.IntValue; ++i)
                await OrbCmd.Channel<FrostOrb>(choiceContext, card.Owner);
            await PowerCmd.Apply<HailstormPower>(
                choiceContext, card.Owner.Creature, card.DynamicVars[nameof(HailstormPower)].BaseValue,
                card.Owner.Creature, card
            );
        }
    }

    public sealed class HailstormOnUpgrade : IPatchMethod {
        public static string PatchId => "Hailstorm.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hailstorm), "OnUpgrade")];
        }

        public static bool Prefix(Hailstorm __instance) {
            __instance.DynamicVars[RepeatVar.defaultName].UpgradeValueBy(1);
            return false;
        }
    }

    public sealed class HailstormPowerExtraHoverTips : IPatchMethod {
        public static string PatchId => "HailstormPower.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(HailstormPower), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result) {
            __result = [
                HoverTipFactory.FromOrb<FrostOrb>(),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ];
            return false;
        }
    }

    public sealed class HailstormPowerCanonicalVars : IPatchMethod {
        public static string PatchId => "HailstormPower.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(HailstormPower), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [];
            return false;
        }
    }

    public sealed class HailstormPowerBeforeSideTurnEnd : IPatchMethod {
        public static string PatchId => "HailstormPower.BeforeSideTurnEnd";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(HailstormPower), nameof(HailstormPower.BeforeSideTurnEnd))];
        }

        public static bool Prefix(ref Task __result) {
            __result = Task.CompletedTask;
            return false;
        }
    }

    public sealed class FrostOrbPassive : IPatchMethod {
        public static string PatchId => "FrostOrb.Passive";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(FrostOrb), nameof(FrostOrb.Passive))];
        }

        public static void Postfix(ref Task __result, FrostOrb __instance, PlayerChoiceContext choiceContext) {
            __result = Passive(__result, __instance, choiceContext);
        }

        private static async Task Passive(Task task, FrostOrb frostOrb, PlayerChoiceContext choiceContext) {
            await task;
            var player = frostOrb.Owner;
            var power = player.Creature.GetPower<HailstormPower>();
            if (power == null) return;
            power.Flash();
            for (var i = 0; i < power.Amount; i++) {
                var list = frostOrb.CombatState.GetOpponentsOf(frostOrb.Owner.Creature).Where(e => e.IsHittable).ToList();
                if (list.Count == 0) return;
                var target = frostOrb.Owner.RunState.Rng.CombatTargets.NextItem(list);
                if (target == null) return;
                await CreatureCmd.Damage(choiceContext, target, frostOrb.PassiveVal, ValueProp.Unpowered, frostOrb.Owner.Creature);
            }
        }
    }

    public sealed class FrostOrbEvoke : IPatchMethod {
        public static string PatchId => "FrostOrb.Evoke";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(FrostOrb), nameof(FrostOrb.Evoke))];
        }

        public static void Postfix(
            ref Task<IEnumerable<Creature>> __result,
            FrostOrb __instance,
            PlayerChoiceContext playerChoiceContext
        ) {
            __result = Evoke(__result, __instance, playerChoiceContext);
        }

        private static async Task<IEnumerable<Creature>> Evoke(
            Task<IEnumerable<Creature>> task,
            FrostOrb frostOrb,
            PlayerChoiceContext playerChoiceContext
        ) {
            var result = await task;
            var player = frostOrb.Owner;
            var power = player.Creature.GetPower<HailstormPower>();
            if (power == null) return result;
            power.Flash();
            for (var i = 0; i < power.Amount; i++) {
                var list = frostOrb.CombatState.GetOpponentsOf(frostOrb.Owner.Creature).Where(e => e.IsHittable).ToList();
                if (list.Count == 0) return result;
                var target = frostOrb.Owner.RunState.Rng.CombatTargets.NextItem(list);
                if (target == null) return result;
                await CreatureCmd.Damage(playerChoiceContext, target, frostOrb.EvokeVal, ValueProp.Unpowered, frostOrb.Owner.Creature);
            }

            return result;
        }
    }
}