using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 28(36) damage to ALL enemies. Lose 1 Orb Slot.
///     </para>
/// </summary>
[CardPatch(nameof(Hyperbeam))]
public static class HyperbeamPatch {
    public sealed class HyperbeamCanonicalVars : IPatchMethod {
        public static string PatchId => "Hyperbeam.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hyperbeam), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new DamageVar(28m, ValueProp.Move),
                new DynamicVar("OrbSlots", 1m)
            ];
            return false;
        }
    }

    public sealed class HyperbeamExtraHoverTips : IPatchMethod {
        public static string PatchId => "Hyperbeam.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hyperbeam), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result) {
            __result = [];
            return false;
        }
    }

    public sealed class HyperbeamOnPlay : IPatchMethod {
        public static string PatchId => "Hyperbeam.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Hyperbeam), "OnPlay")];
        }

        public static bool Prefix(ref Task __result, Hyperbeam __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(Hyperbeam card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            await DamageCmd.Attack(card.DynamicVars.Damage.BaseValue)
                .FromCard(card, cardPlay)
                .TargetingAllOpponents(card.CombatState!)
                .WithAttackerAnim("Cast", 0.5f)
                .BeforeDamage(
                    async delegate {
                        var enemies = card.CombatState!.Enemies.Where(e => e.IsAlive).ToList();
                        var hyperbeamVfx = NHyperbeamVfx.Create(card.Owner.Creature, enemies.Last());
                        if (hyperbeamVfx != null) {
                            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(hyperbeamVfx);
                            await Cmd.Wait(0.5f);
                        }

                        foreach (var enemy in enemies) {
                            var impactVfx = NHyperbeamImpactVfx.Create(card.Owner.Creature, enemy);
                            if (impactVfx != null)
                                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(impactVfx);
                        }
                    }
                )
                .Execute(choiceContext);

            OrbCmd.RemoveSlots(card.Owner, card.DynamicVars["OrbSlots"].IntValue);
        }
    }
}