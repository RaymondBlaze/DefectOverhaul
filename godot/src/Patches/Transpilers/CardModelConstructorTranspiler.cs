using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Utils.HarmonyIl;

namespace DefectOverhaul.Patches.Transpilers;

public static class CardModelConstructorTranspiler {
    private static readonly ConstructorInfo constructor = AccessTools.Constructor(
        typeof(CardModel),
        [typeof(int), typeof(CardType), typeof(CardRarity), typeof(TargetType), typeof(bool)]
    );

    private static readonly HarmonyIlPattern pattern = HarmonyIlPattern.Sequence(
        HarmonyIl.IsLdarg(0),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        instr => instr.opcode == OpCodes.Call && ReferenceEquals(instr.operand, constructor)
    );

    public static IEnumerable<CodeInstruction> ModifyArgs(
        IEnumerable<CodeInstruction> instructions,
        int? canonicalEnergyCost = null,
        CardType? type = null,
        CardRarity? rarity = null,
        TargetType? targetType = null,
        bool? shouldShowInCardLibrary = null
    ) {
        var rewriter = HarmonyIlRewriter.From(instructions);

        if (!rewriter.TryFind(pattern, out var match))
            throw new InvalidOperationException("CardModel constructor call pattern not found in IL.");

        var insns = rewriter.Instructions();
        var offset = match.Index;

        // ldarg.0
        offset++;
        // ldc.i4 (canonicalEnergyCost)
        if (canonicalEnergyCost.HasValue)
            insns[offset] = HarmonyIl.LdcI4(canonicalEnergyCost.Value);
        offset++;
        // ldc.i4 (type)
        if (type.HasValue)
            insns[offset] = HarmonyIl.LdcI4((int)type.Value);
        offset++;
        // ldc.i4 (rarity)
        if (rarity.HasValue)
            insns[offset] = HarmonyIl.LdcI4((int)rarity.Value);
        // ldc.i4 (targetType)
        offset++;
        if (targetType.HasValue)
            insns[offset] = HarmonyIl.LdcI4((int)targetType.Value);
        // ldc.i4 (shouldShowInCardLibrary)
        offset++;
        if (shouldShowInCardLibrary.HasValue)
            insns[offset] = HarmonyIl.LdcI4(shouldShowInCardLibrary.Value ? 1 : 0);

        return insns;
    }
}