namespace DefectOverhaul.Patches.Gameplay;

[AttributeUsage(AttributeTargets.Class)]
public sealed class GameplayPatchAttribute(string id, string title, string description) : Attribute, IComparable<GameplayPatchAttribute> {
    public string Id { get; } = id;
    public string Title { get; } = title;
    public string Description { get; } = description;

    public int CompareTo(GameplayPatchAttribute? other) {
        return string.Compare(Id, other?.Id, StringComparison.Ordinal);
    }
}