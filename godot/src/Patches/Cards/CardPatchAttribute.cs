using MegaCrit.Sts2.Core.Helpers;

namespace DefectOverhaul.Patches.Cards;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CardPatchAttribute(string card) : Attribute {
    public string CardId { get; } = StringHelper.Slugify(card);
}