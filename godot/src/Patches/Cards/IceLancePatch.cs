using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Deal 15(20) damage. Deals 3(4) additional damage for each Frost Orb Channeled this combat.
///         Channel 3 Frost.
///     </para>
/// </summary>
[CardPatch(nameof(IceLance))]
public static class IceLancePatch {
    public sealed class IceLanceCanonicalVars : IPatchMethod {
        public static string PatchId => "IceLance.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(IceLance), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [
                new CalculationBaseVar(15),
                new ExtraDamageVar(3),
                new CalculatedDamageVar(ValueProp.Move).WithMultiplier((card, _) =>
                    CombatManager.Instance.History.Entries
                        .OfType<OrbChanneledEntry>()
                        .Count(entry => entry.Actor.Player == card.Owner && entry.Orb is FrostOrb)
                ),
                new RepeatVar(3)
            ];
            return false;
        }
    }

    public sealed class IceLanceOnUpgrade : IPatchMethod {
        public static string PatchId => "IceLance.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(IceLance), "OnUpgrade")];
        }

        public static bool Prefix(IceLance __instance) {
            __instance.DynamicVars.CalculationBase.UpgradeValueBy(5);
            __instance.DynamicVars.ExtraDamage.UpgradeValueBy(1);
            return false;
        }
    }

    public sealed class IceLanceOnPlay : IPatchMethod {
        public static string PatchId => "IceLance.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(IceLance), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            IceLance __instance,
            PlayerChoiceContext choiceContext,
            CardPlay cardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task OnPlay(IceLance card, PlayerChoiceContext choiceContext, CardPlay cardPlay) {
            ArgumentNullException.ThrowIfNull(cardPlay.Target);
            await DamageCmd
                .Attack(card.DynamicVars.CalculatedDamage.Calculate(cardPlay.Target))
                .FromCard(card, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
                .Execute(choiceContext);
            for (var i = 0; i < card.DynamicVars.Repeat.IntValue; i++)
                await OrbCmd.Channel<FrostOrb>(choiceContext, card.Owner);
        }
    }
}