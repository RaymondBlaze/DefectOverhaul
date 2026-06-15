using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
///         Rarity: Uncommon -> Rare
///     </para>
///     <para>
///         Effect -> Status gain Ethereal. Whenever you Exhaust a Status, gain 4(5) Block.
///     </para>
/// </summary>
[CardPatch(nameof(Smokestack))]
public static class SmokestackPatch {
    public sealed class SmokestackConstructor : IPatchMethod {
        public static string PatchId => "Smokestack.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Smokestack), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, rarity: CardRarity.Rare);
        }
    }

    public sealed class SmokestackCanonicalVars : IPatchMethod {
        public static string PatchId => "Smokestack.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Smokestack), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new PowerVar<SmokestackPower>(4)
            ];
            return false;
        }
    }

    public sealed class CardModelExtraHoverTips : IPatchMethod {
        public static string PatchId => "CardModel.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(CardModel), "ExtraHoverTips", MethodType.Getter)];
        }

        public static void Postfix(ref IEnumerable<IHoverTip> __result, CardModel __instance) {
            if (__instance is not Smokestack) return;
            __result = [
                HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
                HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ];
        }
    }

    public sealed class PowerModelExtraHoverTips : IPatchMethod {
        public static string PatchId => "PowerModel.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(PowerModel), "ExtraHoverTips", MethodType.Getter)];
        }

        public static void Postfix(ref IEnumerable<IHoverTip> __result, PowerModel __instance) {
            if (__instance is not SmokestackPower) return;
            __result = [
                HoverTipFactory.FromKeyword(CardKeyword.Ethereal),
                HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
                HoverTipFactory.Static(StaticHoverTip.Block)
            ];
        }
    }

    public sealed class SmokestackOnUpgrade : IPatchMethod {
        public static string PatchId => "Smokestack.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Smokestack), "OnUpgrade")];
        }

        public static bool Prefix(Smokestack __instance) {
            __instance.DynamicVars[nameof(SmokestackPower)].UpgradeValueBy(1);
            return false;
        }
    }

    public sealed class SmokestackPowerAfterCardGeneratedForCombat : IPatchMethod {
        public static string PatchId => "SmokestackPower.AfterCardGeneratedForCombat";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(SmokestackPower), nameof(SmokestackPower.AfterCardGeneratedForCombat))];
        }

        public static bool Prefix(ref Task __result) {
            __result = Task.CompletedTask;
            return false;
        }
    }

    [RegisterSingleton]
    public sealed class DefectOverhaulSmokestackCombatHooks() : HookedSingletonModel(HookType.Combat) {
        public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource) {
            if (!CardPatches.IsCardPatched<Smokestack>()) return;
            if (power is not SmokestackPower) return;
            var combatState = power.Owner.Player?.PlayerCombatState;
            if (combatState == null) return;
            foreach (var card in combatState.AllCards.Where(card => card.Type == CardType.Status))
                CardCmd.ApplyKeyword(card, CardKeyword.Ethereal);
        }

        public override async Task AfterCardEnteredCombat(CardModel card) {
            if (!CardPatches.IsCardPatched<Smokestack>()) return;
            if (card.Type != CardType.Status) return;
            var player = card.Owner;
            var power = player.Creature.GetPower<SmokestackPower>();
            if (power == null) return;
            card.AddKeyword(CardKeyword.Ethereal);
        }

        public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal) {
            if (!CardPatches.IsCardPatched<Smokestack>()) return;
            if (card.Type != CardType.Status) return;
            var player = card.Owner;
            var power = player.Creature.GetPower<SmokestackPower>();
            if (power == null) return;
            power.Flash();
            await CreatureCmd.GainBlock(player.Creature, power.Amount, ValueProp.Unpowered, null);
        }
    }
}