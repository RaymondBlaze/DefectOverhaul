using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 6(9) damage to ALL enemies. Draw 1 card for each enemy.
///     </para>
/// </summary>
[CardPatch(nameof(SweepingBeam))]
public static class SweepingBeamPatch {
    private const string CalculatedCardsKey = "CalculatedCards";

    public sealed class SweepingBeamCanonicalVars : IPatchMethod {
        public static string PatchId => "SweepingBeam.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(SweepingBeam), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new DamageVar(6, ValueProp.Move),
                new CalculationBaseVar(0),
                new CalculationExtraVar(1),
                new CalculatedVar(CalculatedCardsKey).WithMultiplier((card, _) =>
                    card.CombatState!.HittableEnemies.Count
                )
            ];
            return false;
        }
    }

    public sealed class SweepingBeamOnPlay : IPatchMethod {
        public static string PatchId => "SweepingBeam.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(SweepingBeam), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            SweepingBeam __instance,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(SweepingBeam card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            var calculatedCards = ((CalculatedVar)card.DynamicVars[CalculatedCardsKey]).Calculate(cardPlay.Target);
            await CreatureCmd.TriggerAnim(card.Owner.Creature, "Attack", card.Owner.Character.AttackAnimDelay);
            await DamageCmd
                .Attack(card.DynamicVars.Damage.BaseValue)
                .FromCard(card, cardPlay)
                .TargetingAllOpponents(card.CombatState!)
                .WithAttackerAnim("Cast", 0.5f)
                .BeforeDamage(
                    async delegate {
                        var targets = card.CombatState!.HittableEnemies.ToList();
                        var vfx = NSweepingBeamVfx.Create(card.Owner.Creature, targets);
                        if (vfx != null) {
                            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);
                            await Cmd.Wait(0.5f);
                        }
                    }
                )
                .Execute(choiceContext);
            await CardPileCmd.Draw(choiceContext, calculatedCards, card.Owner);
        }
    }
}