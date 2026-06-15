using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Models;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Cost: 2 -> 3
///     </para>
///     <para>
///         Effect -> Channel 1 Lightning. Channel 1 Frost. Channel 1 Dark.
///         Whenever you Channel an Orb, this card costs 1[Energy] less until played. (Retain.)
///     </para>
/// </summary>
[CardPatch(nameof(Rainbow))]
public static class RainbowPatch {
    public sealed class RainbowConstructor : IPatchMethod {
        public static string PatchId => "Rainbow.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Rainbow), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, 3);
        }
    }

    public sealed class RainbowCanonicalKeywords : IPatchMethod {
        public static string PatchId => "Rainbow.CanonicalKeywords";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Rainbow), "CanonicalKeywords", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<CardKeyword> __result) {
            __result = [];
            return false;
        }
    }

    public sealed class CardModelCanonicalVars : IPatchMethod {
        public static string PatchId => "CardModel.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(CardModel), "CanonicalVars", MethodType.Getter)];
        }

        public static void Postfix(ref IEnumerable<DynamicVar> __result, CardModel __instance) {
            if (__instance is not Rainbow) return;
            __result = [new EnergyVar(1)];
        }
    }

    public sealed class RainbowExtraHoverTips : IPatchMethod {
        public static string PatchId => "Rainbow.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Rainbow), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result, Rainbow __instance) {
            __result = [
                HoverTipFactory.Static(StaticHoverTip.Channeling),
                HoverTipFactory.FromOrb<LightningOrb>(),
                HoverTipFactory.FromOrb<FrostOrb>(),
                HoverTipFactory.FromOrb<DarkOrb>(),
                HoverTipFactory.ForEnergy(__instance)
            ];
            return false;
        }
    }

    public sealed class RainbowOnUpgrade : IPatchMethod {
        public static string PatchId => "Rainbow.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Rainbow), "OnUpgrade")];
        }

        public static bool Prefix(Rainbow __instance) {
            __instance.AddKeyword(CardKeyword.Retain);
            return false;
        }
    }

    [RegisterSingleton]
    public sealed class DefectOverhaulRainbowCombatHooks() : HookedSingletonModel(HookType.Combat) {
        public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb) {
            if (!CardPatches.IsCardPatched<Rainbow>()) return;
            var cards = orb.Owner.PlayerCombatState!.AllPiles
                .Where(pile => pile.Type != PileType.Play)
                .SelectMany(pile => pile.Cards)
                .OfType<Rainbow>();
            foreach (var card in cards)
                card.EnergyCost.AddUntilPlayed(-1);
        }
    }
}