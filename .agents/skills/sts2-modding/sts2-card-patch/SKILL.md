---
name: sts2-card-patch
description: "Write STS2 Card patches with Harmony + RitsuLib IPatchMethod, including XML doc summaries. Use when the user requests to patch a Card's behavior."
---

# STS2 Card Patch Workflow

Related Docs:

- [`STS2-RitsuLib docs`](../../../../docs/STS2-RitsuLib)
- [`SlayTheSpire2ModdingTutorials`](../../../../docs/SlayTheSpire2ModdingTutorials).

Related Libraries:

- sts2
- STS2-RitsuLib
- 0Harmony

Related Skill:

- [`sts2-modding/sts2-writing-descriptions`](../sts2-writing-descriptions/SKILL.md).

## Involved Files

- `TargetCardPatch.cs`: All-in-one class for all implementation
- `card_patches/localization/{lang}/cards.json`: localization patches for the Card, entry grouped by Card ID.
- `card_patches/localization/{lang}/powers.json`: localization patches for the Card's related Power, entry grouped by
  Card ID.

## Understanding the Need

When the user requested patch for a Card, breakdown the need with this path:

- Base properties changes: Does the need ask for patches for energy cost, Type (Attack, Skill, Power...),
  Rarity (Common, Uncommon, Rare...), Target type (Any enemy, all enemies...)?
- Value changes: Does the need ask for Damage, Block, Power count and other DynamicVar related changes?
- Keyword changes: Does the need add/remove the Card's keyword?
- Upgrade effect changes: Does the need affects the Card's upgrade effect?
- Description and hover tip changes: Does the need requires changing the raw description text? If so, does it require
  adding/
  removing new HoverTip? Use the `sts2-modding/sts2-writing-descriptions` skill for this.

## Patch Class

```csharp
[CardPatch(nameof(TargetCard))]
public class TargetCardPatch {
    // Patching the Constructor: When you need to modify canonicalEnergyCost, type, rarity, targetType of the Card.
    // DON'T patch this if your intension was to ONLY modify the upgraded cost, patch `OnUpgrade` instead.
    public sealed class TargetCardConstructor : IPatchMethod {
        public static string PatchId => "TargetCard.Constructor";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(TargetCard), ".ctor", MethodType.Constructor)];
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            return CardModelConstructorTranspiler.ModifyArgs(instructions, ...);
        }
    }
    
    // Patching CanonicalKeywords getter: When you need to modify the CanonicalKeywords of the Card.
    // Note: DON'T patch this if your intension was to ONLY add a CardKeyword on upgrade, patch `OnUpgrade` instead.
    // Use `CardModel` as target class instead if the target class doesn't override this method, also remember to use 
    // `PostFix` and type checks so the patch is compatible with other patches to `CardModel`, this workaround applies 
    // to all other non-hook-listener methods.
    public sealed class TargetCardCanonicalKeywords : IPatchMethod {
        public static string PatchId => "TargetCard.CanonicalKeywords";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(TargetCard), "CanonicalKeywords", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<CardKeyword> __result) {
            __result = [...];
            return false;
        }
    }
    
    // Patching CanonicalVars getter: When you need to tweak the Card's DynamicVars to change the effect values, or when you
    // need to add new DynamicVars for implementing new effects.
    // Note: DON'T patch this if your intension was to ONLY change the upgrade value of a DynamicVar, patch `OnUpgrade` instead.
    public sealed class TargetCardCanonicalVars : IPatchMethod {
        public static string PatchId => "TargetCard.CanonicalVars";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(TargetCard), "CanonicalVars", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<DynamicVar> __result) {
            __result = [...];
            return false;
        }
    }
    
    // Patching ExtraHoverTips getter: When you need to tweak the Card's ExtraHoverTips due to implementing a completely 
    // different effect that does not include the same set of terms in description and thus should provide new hover tips.
    // Note: DON'T patch this if you didn't change the description at all or didn't add/remove any terms.
    public sealed class TargetCardExtraHoverTips : IPatchMethod {
        public static string PatchId => "TargetCard.ExtraHoverTips";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(TargetCard), "ExtraHoverTips", MethodType.Getter)];
        }

        public static bool Prefix(ref IEnumerable<IHoverTip> __result) {
            __result = [...];
            return false;
        }
    }
    
    // Patching OnUpgrade: When you need to change the upgrade effect of the Card.
    // Note: Not all upgrade effects are implemented via this method, clarify the design first.
    public sealed class TargetCardOnUpgrade : IPatchMethod {
        public static string PatchId => "TargetCard.OnUpgrade";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(TargetCard), "OnUpgrade")];
        }

        public static bool Prefix(TargetCard __instance) {
            __instance.AddKeyword(CardKeyword.Retain);
            return false;
        }
    }
    
    // Patching OnPlay: When you need to change the play effect of the Card.
    // Note: This example shows how you may overwrite OnPlay completely, but alternatively you can use PostFix to perform 
    // your effect after the original effect.
    public sealed class TargetCardOnPlay : IPatchMethod {
        public static string PatchId => "TargetCard.OnPlay";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(TargetCard), "OnPlay")];
        }

        public static bool Prefix(
            ref Task __result,
            TargetCard __instance,
            PlayerChoiceContext choiceContext,
            CardPlay CardPlay
        ) {
            __result = OnPlay(__instance, choiceContext, CardPlay);
            return false;
        }

        private static async Task OnPlay(TargetCard Card, PlayerChoiceContext choiceContext, CardPlay CardPlay) {
            // ...
        }
    }
    
    // Patching hook listeners: When you need to tweak the Card's existing behavior over a game hook.
    // Note: DON'T patch `AbstractModel`'s empty hook listeners, use `HookedSingletonModel` if you want to subscribe to 
    // new hooks when the Card didn't create an override.
    public sealed class TargetCardAfterSideTurnEnd : IPatchMethod {
        public static string PatchId => "TargetCard.AfterSideTurnEnd";

        public static ModPatchTarget[] GetTargets() {
            return [new ModPatchTarget(typeof(TargetCard), nameof(TargetCard.AfterSideTurnEnd))];
        }

        public static bool Prefix(ref Task __result) {
            __result = Task.CompletedTask;
            return false;
        }
    }
    
    // Patching related Power class: Some Cards implement parts of their behavior in the Power it applies, especially
    // Power Cards. Patching the Power class is similar to patching the Card class, `CanonicalVars`, `ExtraHoverTips` 
    // and hook listeners works basically the same in the Power class. Since our final target is to change the Card's 
    // behavior, patches for the related Power class should also be placed here.
    
    // Patching other classes: When you need to change a very specific behavior, existing game hooks may not be helpful 
    // at all, ONLY in this case should you consider patching other classes.
    
    // Listening to game hooks using `HookedSingletonModel`, see the docs for detailed usage guide.
    // Use ModName and TargetCard class name as prefix to avoid Model ID clashing
    [RegisterSingleton]
    public sealed class ModNameTargetCardCombatHooks() : HookedSingletonModel(HookType.Combat) {
        public override async Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb) {
            // ...
        }
    }
}
```

## Localization Patches

New descriptions for patched Cards and Powers are injected dynamically, so the localization patches schema is a bit
differnt
from normal localization files.

- cards.json

```json lines
{
  "CARD_ID": {
    "CARD_ID.description": "..."
  },
  // ...
}
```

- powers.json

```json lines
{
  "CARD_ID": {
    "POWER_ID.description": "...",
    "POWER_ID.smartDescription": "..."
  },
  // ...
}
```

The `CARD_ID` key identifies that these localization entries belongs to certain Card patch, when the Card patch is
disabled, these entries will not be injected.

Do note that you only need make localization patches when the raw description text needs to change, if you only changed
things like energy cost, rarity, values of DynamicVars, owned keywords etc., there's no need touching the localization
files.

Use `sts2-modding/sts2-writing-descriptions` skill to write descriptions.

IMPORTANT: DO NOT mix localization patches with normal localization tables!

## Steady Enumeration

When batch-processing multiple cards (descriptions, gallery generation, mass patches):

- Process **2-3 cards at a time**, verify each batch, report progress.
- Do NOT preload or search all card IDs at once — the localization JSON files are large (thousands of entries).
- Search for card IDs one small batch at a time.
- Cross-reference image existence (`images/cards/`) before looking up localization data.

## Verify

After building, extract the PCK and confirm your patches landed in the right files.
