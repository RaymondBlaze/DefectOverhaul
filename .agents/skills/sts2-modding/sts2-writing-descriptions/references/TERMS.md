# Terms

This document will introduce all commonly used game terms and their usage in description texts and hover tips.

## Powers

Powers are considered terms.

For Power Cards, it usually gains a dedicated Power with the same name as the Card (e.g. `AfterImage` vs `AfterImagePower`), 
this behavior is considered default and thus does not need to be mentioned in the Card description (usually the Power's 
behavior is described in the Card description instead).

Cards of other types (or other sources of Power like Potions) sometimes also implements non-instant effects using dedicated 
Powers (e.g. `LightningRod` vs `LightningRodPower`).
They also follow the Power Cards' pattern - no mentioning dedicated Power in the description.

Therefore, only common Powers would be mentioned in descriptions as terms, the following table lists all Powers commonly 
used in vanilla game descriptions. Feel free to add new common Powers introduced by the mod you're working on to the list.

| Chinese   |  English   | Example                                                   |
|-----------|------------|-----------------------------------------------------------|
| 力量      | Strength   | Gain {StrengthPower:diff()} [gold]Strength[/gold]         |
| 敏捷      | Dexterity  | Gain {DexterityPower:diff()} [gold]Dexterity[/gold]       |
| 集中      | Focus      | Gain {FocusPower:diff()} [gold]Focus[/gold]               |
| 活力      | Vigor      | Gain {VigorPower:diff()} [gold]Vigor[/gold]               |
| 荆棘      | Thorns     | Gain {ThornsPower:diff()} [gold]Thorns[/gold]             |
| 覆甲      | Plating    | Gain {PlatingPower:diff()} [gold]Plating[/gold]           |
| 无实体    | Intangible | Gain {IntangiblePower:diff()} [gold]Intangible[/gold]     |
| 易伤      | Vulnerable | Apply {VulnerablePower:diff()} [gold]Vulnerable[/gold]    |
| 虚弱      | Weak       | Apply {WeakPower:diff()} [gold]Weak[/gold]                |
| 脆弱      | Frail      | Gain {FrailPower:diff()} [gold]Frail[/gold]               |
| 灾厄      | Doom       | Apply {DoomPower:diff()} [gold]Doom[/gold]                |
| 中毒      | Poison     | Apply {PoisonPower:diff()} [gold]Poison[/gold]            |
| 人工制品  | Artifact   | Remove all [gold]Artifact[/gold] and Block from the enemy |

### HoverTip

Whenever a Power is mentioned in the description as a term, it should also be added to the `ExtraHoverTips` getter in the 
corresponding `CardModel`/`PowerModel`/`RelicModel`/`PotionModel` implementation. To create a `HoverTip` for a Power, use 
`HoverTipFactory.FromPower`.

**IMPORTANT NOTE**: Power Cards always add their dedicated Power to the `ExtraHoverTips`, other Cards and non-Cards don't.

## Cards

Card names are also considered terms.

Most Cards mentioned in descriptions are either Status (`CardType.Status`) or Token (`CardRarity.Token`) generated in combat.
Typical examples include Dazed (Status), Slimed (Status), Shivs (Token) and Sovereign Blade (Token).

### HoverTip

Cards mentioned as terms should also be added to the `ExtraHoverTips`. To create a `HoverTip` for a Power, use
`HoverTipFactory.FromCard` or `HoverTipFactory.FromCardWithCardHoverTips`.

## Card Keywords

Card Keywords are also considered terms.

Card Keywords owned by the Card itself would be automatically injected to the Card description in game, so these don't 
need to be mentioned in the description text in localization files. For the rest of the time, Card Keywords can be mentioned 
in descriptions as terms.

Refer to the `card_keywords.json` localization table for all vanilla Card Keywords.

**IMPORTANT NOTE**: The Card Keyword `Exhaust` is a verb so it could also come in past tense, which should also be considered 
the same term.

### HoverTip

Owned Card Keywords' `HoverTip` is also auto-injected. You only need to add `HoverTip` for the Card Keywords mentioned in 
description using `HoverTipFactory.FromKeyword`.

## Card Piles

Card Piles are also considered terms, all Card Pile terms are listed below.

| Chinese  | English      | Example                                                                             |
|----------|--------------|-------------------------------------------------------------------------------------|
| 手牌     | Hand         | Put a random Attack from your [gold]Discard Pile[/gold] into your [gold]Hand[/gold] |
| 抽牌堆   | Draw Pile    | Put every Rare card from your [gold]Draw Pile[/gold] into your Hand                 |
| 弃牌堆   | Discard Pile | Add a copy of this card into your [gold]Discard Pile[/gold]                         |
| 消耗牌堆 | Exhaust Pile | Deal additional damage for each card in your [gold]Exhaust Pile[/gold]              |
| 牌组     | Deck         | Remove this from your [gold]Deck[/gold]                                             |

Card Pile terms do not have `HoverTip`s.

## Orbs

Orb names are also considered keywords, all vanilla Orb types listed below.

| Chinese | English   | Example                          |
|---------|-----------|----------------------------------|
| 闪电    | Lightning | Channel 1 [gold]Lightning[/gold] |
| 冰霜    | Frost     | Channel 1 [gold]Frost[/gold]     |
| 黑暗    | Dark      | Channel 1 [gold]Dark[/gold]      |
| 等离子  | Plasma    | Channel 1 [gold]Plasma[/gold]    |
| 玻璃    | Glass     | Channel 1 [gold]Glass[/gold]     |

**IMPORTANT NOTE**: The word "Orb" itself is NOT a term and should not be formatted using `[gold][/gold]`. For Chinese(`zhs`) 
localization, when referring to a type of Orb like Lightning, use `[gold]闪电[/gold]充能球` instead of just `[gold]闪电[/gold]`.

### HoverTip

Use `HoverTipFactory.FromOrb` to create `HoverTip` for Orbs.

## Terms with Static HoverTip

This section covers misc terms with `HoverTip` created from `StaticHoverTip`.

| Chinese | English   | Example                                                       |
|---------|-----------|---------------------------------------------------------------|
| 生成    | Channel   | [gold]Channel[/gold] 1 Lightning                              |
| 激发    | Evoke     | At the end of your turn, [gold]Evoke[/gold] your leftmost Orb |
| 变化    | Transform | Choose a card in your Hand to [gold]Transform[/gold]          |
| 格挡    | Block     | Gain [gold]Block[/gold]                                       |
| 斩杀    | Fatal     | If [gold]Fatal[/gold], raise your Max HP                      |
| 击晕    | Stun      | [gold]Stun[/gold] the enemy                                   |
| 铸造    | Forge     | [gold]Forge[/gold] {CalculatedForge:diff()}                   |
| 召唤    | Summon    | [gold]Summon[/gold] {Summon:diff()}                           |
| 重放    | Replay    | A random card in your Draw Pile gains [gold]Replay[/gold]     |

IMPORTANT NOTES: Like the Card Keyword `Exhaust`, some of these terms are verbs so they could come in forms of past tense, 
which should also be considered the same term.

### HoverTip

Although these terms are defined in `StaticHoverTip`, some of them have dedicated factory method in `HoverTipFactory`, 
refer to related example when implementing.

## Energy

Energy, although using icons created `{Energy:energyIcons()}` instead of gold text, is also considered terms.

### HoverTip

Use `HoverTipFactory.ForEnergy` to create `HoverTip` for Energy icons.
