using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Orbs;
using STS2RitsuLib.Patching.Models;

namespace DefectOverhaul.Patches.Cards;

/// <summary>
///     <para>
///         Effect -> Channel 1 Dark. Trigger the passive of (your rightmost -> all) Dark Orb(s).
///     </para>
/// </summary>
[CardPatch(nameof(Darkness))]
public static class DarknessPatch {
    public sealed class DarknessOnPlay : IPatchMethod {
        public static string PatchId => "Darkness.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(Darkness), "OnPlay")];
        }

        public static bool Prefix(ref Task __result, Darkness __instance, PlayerChoiceContext choiceContext) {
            __result = OnPlay(__instance, choiceContext);
            return false;
        }

        private static async Task OnPlay(Darkness card, PlayerChoiceContext choiceContext) {
            await CreatureCmd.TriggerAnim(card.Owner.Creature, "Cast", card.Owner.Character.CastAnimDelay);
            await OrbCmd.Channel<DarkOrb>(choiceContext, card.Owner);

            var player = card.Owner;
            var darkOrbs = player.PlayerCombatState!.OrbQueue.Orbs.OfType<DarkOrb>();

            if (card.IsUpgraded) {
                foreach (var darkOrb in darkOrbs)
                    await OrbCmd.Passive(choiceContext, darkOrb, null);
            }
            else {
                var darkOrb = darkOrbs.FirstOrDefault();
                if (darkOrb != null)
                    await OrbCmd.Passive(choiceContext, darkOrb, null);
            }
        }
    }
}