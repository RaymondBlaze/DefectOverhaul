using System.Reflection.Emit;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Utils.HarmonyIl;

namespace DefectOverhaul.Patches.Transpilers;

public static class CardModelConstructorTranspiler {
    private static readonly HarmonyIlPattern target = HarmonyIlPattern.Sequence(
        HarmonyIl.IsLdarg(0),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.IsLdcI4(),
        HarmonyIl.Is(
            OpCodes.Call, AccessTools.Constructor(
                typeof(CardModel),
                [typeof(int), typeof(CardType), typeof(CardRarity), typeof(TargetType), typeof(bool)]
            )
        )
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

        if (!rewriter.TryFind(target, out var match))
            throw new InvalidOperationException("Could not find target in IL.");

        var insns = rewriter.Instructions();
        var offset = match.Index;

        // ldc.i4 (canonicalEnergyCost)
        if (canonicalEnergyCost.HasValue)
            insns[offset + 1] = HarmonyIl.LdcI4(canonicalEnergyCost.Value);
        // ldc.i4 (type)
        if (type.HasValue)
            insns[offset + 2] = HarmonyIl.LdcI4((int)type.Value);
        // ldc.i4 (rarity)
        if (rarity.HasValue)
            insns[offset + 3] = HarmonyIl.LdcI4((int)rarity.Value);
        // ldc.i4 (targetType)
        if (targetType.HasValue)
            insns[offset + 4] = HarmonyIl.LdcI4((int)targetType.Value);
        // ldc.i4 (shouldShowInCardLibrary)
        if (shouldShowInCardLibrary.HasValue)
            insns[offset + 5] = HarmonyIl.LdcI4(shouldShowInCardLibrary.Value ? 1 : 0);

        return insns;
    }
}