using DefectOverhaul.Patches.Transpilers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 4(6) damage to ALL enemies for each Channeled Orb.
///     </para>
/// </summary>
[CardPatch(nameof(Barrage))]
public static class BarragePatch {
    public sealed class BarrageConstructor : IPatchMethod {
        public static string PatchId => "Barrage.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Barrage), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, targetType: TargetType.AllEnemies);
        }
    }

    public sealed class BarrageCanonicalVars : IPatchMethod {
        public static string PatchId => "Barrage.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Barrage), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new DamageVar(4, ValueProp.Move),
                new CalculationBaseVar(0m),
                new CalculationExtraVar(1m),
                new CalculatedVar("CalculatedHits").WithMultiplier((card, _) =>
                    card.Owner.PlayerCombatState!.OrbQueue.Orbs.Count
                )
            ];
            return false;
        }
    }

    public sealed class BarrageOnPlay : IPatchMethod {
        public static string PatchId => "Barrage.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Barrage), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            Barrage __instance,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(Barrage card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            await DamageCmd
                .Attack(card.DynamicVars.Damage.BaseValue)
                .FromCard(card, cardPlay)
                .TargetingAllOpponents(card.CombatState!)
                .WithHitCount((int)((CalculatedVar)card.DynamicVars["CalculatedHits"]).Calculate(cardPlay.Target))
                .WithHitFx("vfx/vfx_attack_blunt")
                .Execute(choiceContext);
        }
    }
}