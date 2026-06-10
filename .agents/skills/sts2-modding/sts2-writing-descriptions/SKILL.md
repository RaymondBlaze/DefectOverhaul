---
name: sts2-writing-descriptions
description: "Translate a card/power/relic/potion description intent into the correct localization markup. Use when given a natural-language description and need to produce the cards.json/powers.json string."
---

# Writing Descriptions

Given a player-facing description (e.g. "Deal 7(10) damage. Channel 1(2) Lightning."), produce the correct localization entry.

Related Docs: 
- [`TERMS.md`](references/TERMS.md) — Read this document for understanding existing terms and related hover tips.
- [`SlayTheSpire2ModdingTutorials`](../../../../docs/SlayTheSpire2ModdingTutorials): Refer to `Basics/05-变量与描述`.

## Highlighting

- Every term in the description should be highlighted with `[gold]...[/gold]` labels. Past tense of verb terms should also be accounted for.
- For Power and Relic description, numbers should be highlighted with `[blue]...[/blue]` labels.

## Using `DynamicVar`s

Variable numbers in the description correspond to `DynamicVar` entries from `CanonicalVars`. 

Intentional fixed numbers (e.g. `twice` vs `{Repeat:diff()} times` where `Repeat` is effectively 2) can stay hardcoded.

Common usecases:

| Phrase                 | Var Name       | Markup                           |
|------------------------|----------------|----------------------------------|
| X damage               | `{Damage}`     | `{Damage:diff()}`                |
| X Block                | `{Block}`      | `{Block:diff()}`                 |
| Gain X Power           | `{PowerClass}` | `{FocusPower:diff()}`            |
| Draw X cards           | `{Cards}`      | `{Cards:diff()}`                 |
| Repeat X times         | `{Repeat}`     | `{Repeat:diff()}`                |
| Energy icons (dynamic) | `{Energy}`     | `{Energy:energyIcons()}`         |
| Energy icons (fixed)   | `energyPrefix` | `0{energyPrefix:energyIcons(1)}` |

Verify the actual name against the card's `CanonicalVars` implementation — it may differ from the convention.

## Using Formatters

Formatters control how a variable is displayed, powered by the `SmartFormat` library.

For example `{Energy:energyIcons()}` renders N energy icons, where N is the value of `Energy`. See the corresponding formatter class for implementation details.

STS2 custom formatters:

| Name                              | Description                                                                               | Example                                          |
|-----------------------------------|-------------------------------------------------------------------------------------------|--------------------------------------------------|
| `diff()`                          | Turns green when above base, red when below. Used in combat and upgrade previews.         | `Deal {Damage:diff()} damage.`                   |
| `inverseDiff()`                   | Red when above base, green when below.                                                    | `Lose {HpLoss:inverseDiff()} HP.`                |
| `energyIcons()`                   | Renders value as energy icons.                                                            | `Gain {Energy:energyIcons()}.`                   |
| `starIcons()`                     | Renders value as star icons.                                                              | `Gain {Stars:starIcons()}.`                      |
| `IfUpgraded:show`                 | Shows different text based on upgrade state.                                              | `{IfUpgraded:show:upgraded_text\|base_text}`     |
| `abs`                             | Absolute value.                                                                           | `{Damage:abs()}`                                 |
| `percentMore()` / `percentLess()` | Percentage display. `PercentMore` turns 1.25 into 25%. `PercentLess` turns 0.75 into 25%. | `Deal {Boost:percentMore()}% additional damage.` |

Built-in `SmartFormat` formatters:

https://github.com/axuno/SmartFormat/wiki

| Name     | Description                                          | Example                                                                     |
|----------|------------------------------------------------------|-----------------------------------------------------------------------------|
| `cond`   | Conditional branching, e.g. `{X:cond:>0?applies\|}`. | `{FanOfKnivesAmount:cond:>0? to ALL enemies\|}`                             |
| `choose` | Selection by index or value.                         | `Play the next {Skills:choose(1):one\|{:diff()}} Skill an additional time.` |
| `plural` | Pluralization.                                       | `Draw {Cards:diff()} {Cards:plural:card\|cards}.`                           |
| `list`   | List concatenation.                                  | https://github.com/axuno/SmartFormat/wiki/v2-Lists                          |

**IMPORTANT**: Remember to use `plural` for English(`eng`) description, general plurals and indefinite article "a" 
should all be covered. Chinese(`zhs`) usually don't need `plural`.

## Card Description Only

Cards have additional context variables:

| Name             | Meaning                                  | Typical Use                                                                           |
|------------------|------------------------------------------|---------------------------------------------------------------------------------------|
| `singleStarIcon` | Star icon                                | `Whenever you gain {singleStarIcon}`                                                  |
| `InCombat`       | Whether in combat                        | `{InCombat:\n(Hits {CalculatedHits:diff()} times)\|}`                                 |
| `IsTargeting`    | Whether targeting a target               | `{IsTargeting:\n(Deal {CalculatedDamage:diff()} damage)\|}`                           |
| `OnTable`        | Whether the card is in hand or play area | `{OnTable:on the field\|not on field}`                                                |
| `IfUpgraded`     | Whether upgraded                         | `[gold]Upgrade[/gold] {IfUpgraded:show:ALL cards\|a card} in your [gold]Hand[/gold].` |

**IMPORTANT**: Card's `CanonicalKeywords` will automatically inject to the description, do not add them yourself, even if 
the provided player-facing description does contain these keywords. This rule applies to the keywords OWNED by the card, 
not the keywords referenced as a part of a condition or effect.

## Power Description Only

Power localization has three fields: `description`, `smartDescription`, and optionally `remoteDescription` for multiplayer.

- **`description`**: Static text. Used when the power is non-variable (e.g. the tooltip shown on the card popup). No special variables.
- **`smartDescription`**: Dynamic text. Used when the power is variable (the tooltip shown when hovering the player character in combat) and `smartDescription` is configured. Injects context variables listed below and merges with `DynamicVars`.
- **`remoteDescription`**: Multiplayer only. When the power is applied by another player (applier exists and is not the local player) and this key is configured, it replaces `smartDescription`.

Available context variables for `smartDescription` / `remoteDescription`:

| Name            | Meaning                               | Typical Use                                          |
|-----------------|---------------------------------------|------------------------------------------------------|
| `Amount`        | Current stack/value                   | `Gain [blue]{Amount}[/blue] [gold]Strength[/gold].`  |
| `OnPlayer`      | Whether owner is the player           | `{OnPlayer:You\|The enemy} gains {Amount} Strength.` |
| `IsMultiplayer` | Whether current combat is multiplayer | `{IsMultiplayer:(multiplayer)\|}`                    |
| `PlayerCount`   | Number of players in current combat   | `{PlayerCount} players in combat.`                   |
| `OwnerName`     | Owner name                            | `{OwnerName} gains {Amount} Strength.`               |
| `ApplierName`   | Applier name                          | `Applied by {ApplierName}.`                          |
| `TargetName`    | Target name                           | `Affects {TargetName}.`                              |
