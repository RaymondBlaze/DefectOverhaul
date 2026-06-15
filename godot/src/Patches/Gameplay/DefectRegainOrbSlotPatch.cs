using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Utils.HarmonyIl;

namespace DefectOverhaul.Patches.Gameplay;

/// <summary>
///     <para>
///         Patches <c>OrbCmd.Channel</c> to not check for <c>CharacterModel.BaseOrbSlotCount</c> before gaining a new
///         Orb Slot when the character has no Orb Slots, making the Defect being able to regain an Orb Slot when all Orb
///         Slots are lost, aligning with other characters.
///     </para>
/// </summary>
[GameplayPatch(
    "defect_regain_orb_slot",
    "Defect Regain Orb Slot",
    "When enabled, Defect regains an Orb Slot upon Channelling a new Orb with no Orb Slots, aligning with other characters."
)]
public static class DefectRegainOrbSlotPatch {
    public sealed class Patch : IPatchMethod {
        private static readonly HarmonyIlPattern target = HarmonyIlPattern.Sequence(
            HarmonyIl.IsCall(
                AccessTools.PropertyGetter(
                    typeof(CharacterModel), nameof(CharacterModel.BaseOrbSlotCount)
                )
            ),
            HarmonyIl.Is(OpCodes.Brtrue_S)
        );

        public static string PatchId => "OrbCmd.Channel";

        public static ModPatchTarget[] GetTargets() {
            return [
                new ModPatchTarget(
                    typeof(OrbCmd),
                    nameof(OrbCmd.Channel),
                    [typeof(PlayerChoiceContext), typeof(OrbModel), typeof(Player)],
                    MethodType.Async
                )
            ];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            var rewriter = HarmonyIlRewriter.From(instructions);

            if (!rewriter.TryFind(target, out var match))
                throw new InvalidOperationException("Could not find target in IL.");

            var insns = rewriter.Instructions();
            var offset = match.Index;

            // brtrue.s -> pop
            // Pop the result of get_BaseOrbSlotCount and remove the brtrue.s jump, fall through to the next check
            insns[offset + 1] = new CodeInstruction(OpCodes.Pop);
            return insns;
        }
    }
}