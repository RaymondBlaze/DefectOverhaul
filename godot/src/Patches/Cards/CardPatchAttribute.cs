using MegaCrit.Sts2.Core.Helpers;

namespace DefectOverhaul.Patches.Cards;

[AttributeUsage(AttributeTargets.Class)]
public sealed class CardPatchAttribute(string card) : Attribute, IComparable<CardPatchAttribute> {
    public string Id { get; } = StringHelper.Slugify(card);

    public int CompareTo(CardPatchAttribute? other) {
        return string.Compare(Id, other?.Id, StringComparison.Ordinal);
    }
}