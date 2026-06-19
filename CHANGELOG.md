# Changelog

## 0.100.2

支持杀戮尖塔2正式版 v0.107.1。
Supports STS2 main branch v0.107.1.

### 修改 (Changes)
为了兼容创意工坊的发布工作流，修改了GitHub Release的打包方式，现在ZIP内不再嵌套一层`DefectOverhaul`文件夹，手动安装时请自行
创建文件夹。
Changed the way how GitHub Release ZIP is packaged for workflow compatibility with the Steam Workshop Release, it now 
does NOT include a `DefectOverhaul` folder, create one yourself when installing the mod manually.

## 0.100.1

<details>
<summary>English</summary>

Supports STS2 beta v0.107.0.

### Fixes
- Fixed the card Uproar being not playable due to referencing a removed `DynamicVar` named `Cards` in the `OnPlay` method.

</details>

<details>
<summary>简体中文</summary>

支持杀戮尖塔2 beta v0.107.0 版本。

### 修复
- 修复了骚动因为在`OnPlay`方法内引用了已移除的`Cards`变量而无法打出的问题。

</details>

## 0.100.0

<details>
<summary>English</summary>

Supports STS2 beta v0.107.0.

### Fixes

- Fixed subscribed combat hooks listeners ignoring Card Patches config, result in patches for certain cards can not be properly disabled.
Affected cards: 
  - Consuming Shadows
  - Coolant
  - Loop
  - Rainbow
  - Smokestack

### Gameplay Changes

#### Defect Regain Orb Slot

- Default: enabled
- Effect: When enabled, Defect regains an Orb Slot upon Channelling a new Orb with no Orb Slots, aligning with other characters.

### Card Changes

#### Biased Cognition

- Original Effect (vanilla): Gain 4(5) Focus. At the start of your turn, lose 1 Focus.
- New Effect: Gain 4(5) Focus. At the end of your turn, if you didn't gain Focus this turn, lose 1 Focus.

#### Coolant

- Original Effect: Whenever you Channel an Orb, gain 3(4) Block.
- New Effect: Whenever you Channel an Orb, gain 2(3) Block.

#### Creative AI

- Original Effect (vanilla): At the start of your turn, add a random Power into your Hand.
- New Effect: At the start of your turn, choose 1 of 3 random Power cards to add into your Hand.

#### Darkness

- Original Effect (vanilla): Channel 1 Dark. Trigger the passive ability of all Dark Orbs (twice).
- New Effect: Channel 1 Dark. Trigger the passive of (your rightmost -> all) Dark Orb(s).

#### Hyperbeam

- Original Effect (vanilla): Deal 28(36) damage to ALL enemies. Lose 3 Focus.
- New Effect: Deal 28(36) damage to ALL enemies. Lose 1 Orb Slot.

#### Ice Lance

- Original Effect: Deal 15(18) damage. Deals 5(6) additional damage for each Frost Orb Channeled this combat. Channel 3 Frost.
- New Effect: Deal 15(20) damage. Deals 3(4) additional damage for each Frost Orb Channeled this combat. Channel 3 Frost.

#### Iteration

- Original Effect: Whenever you draw a Status, draw 1(2) cards.
- New Effect (reverted to vanilla) -> The first time you draw a Status each turn, draw 2(3) cards.

#### Shadow Shield

- Original Effect (vanilla): Gain 11(15) Block. Channel 1 Dark.
- New Effect: Gain 12(16) Block. Channel 1 Dark.

#### Shatter

- Original Cost (vanilla): 1(1)
- New Cost: 2(2)
- Original Effect (vanilla): Deal 7(11) damage to ALL enemies. Evoke all of your Orbs twice.
- New Effect: Deal 14(20) damage to ALL enemies. Evoke all of your Orbs twice.

#### Smokestack

- Original Effect: Status gain Ethereal. Whenever you Exhaust a Status, gain 5(7) Block.
- New Effect: Status gain Ethereal. Whenever you Exhaust a Status, gain 4(5) Block.

#### Synthesis 

- Original Effect (vanilla): Deal 14(20) damage. The next Power you play costs 0 energy.
- New Effect: Deal 14(20) damage. A random card in your Hand is free to play this turn.

#### Uproar

- Original Effect: Deal 6 damage twice. Play 1(2) random Attacks from your Draw Pile.
- New Effect: Deal 6(8) damage twice. Play 1 random Attack from your Draw Pile (against the enemy).

#### Voltaic

- Original Cost (vanilla): 3
- New Cost: 3(2)
- Original Effect: Channel Lightning equal to the Lightning already Channeled this combat. (-Exhaust.)
- New Effect: Channel Lightning equal to the Lightning already Channeled this combat.
</details>

<details>
<summary>简体中文</summary>

支持杀戮尖塔2 beta v0.107.0 版本。

### 修复

- 修复了订阅的战斗事件监听器忽略卡牌修改配置，导致某些卡牌修改无法正确禁用的问题。
  受影响的卡牌：
  - 吞噬暗影
  - 冷却剂
  - 循环
  - 彩虹
  - 烟囱

### 游戏机制改动

#### 故障机器人恢复充能球栏位

- 默认：启用
- 效果：启用时，故障机器人在没有充能球栏位时生成充能球会自动获得1个充能球栏位，与其他角色一致。

### 卡牌改动

#### 偏差认知

- 原效果（原版）：获得4(5)点集中。在你的回合开始时，失去1点集中。
- 新效果：获得4(5)点集中。在你的回合结束时，如果你本回合没有获得过集中，失去1点集中。

#### 冷却剂

- 原效果：每当你生成充能球时，获得3(4)点格挡。
- 新效果：每当你生成充能球时，获得2(3)点格挡。

#### 创造性AI

- 原效果（原版）：在你的回合开始时，将一张随机能力牌加入你的手牌。
- 新效果：在你的回合开始时，从3张随机能力牌中选择1张添加到你的手牌。

#### 漆黑

- 原效果（原版）：生成1黑暗充能球。触发所有黑暗充能球的被动（两次）。
- 新效果：生成1黑暗充能球。触发（你最右侧的 → 所有）黑暗充能球的被动。

#### 超能光束

- 原效果（原版）：对所有敌人造成28(36)点伤害。失去3点集中。
- 新效果：对所有敌人造成28(36)点伤害。失去1个充能球栏位。

#### 冰之长枪

- 原效果：造成15(18)点伤害。你在本场战斗中每生成过一个冰霜充能球，这张牌就额外造成5(6)点伤害。生成3个冰霜充能球。
- 新效果：造成15(20)点伤害。你在本场战斗中每生成过一个冰霜充能球，这张牌就额外造成3(4)点伤害。生成3个冰霜充能球。

#### 迭代

- 原效果：每当你抽到一张状态牌时，抽1(2)张牌。
- 新效果（回调原版）：每回合你第一次抽到状态牌时，抽2(3)张牌。

#### 暗影之盾

- 原效果（原版）：获得11(15)点格挡。生成1个黑暗充能球。
- 新效果：获得12(16)点格挡。生成1个黑暗充能球。

#### 打碎

- 原始费用（原版）：1
- 新费用：2
- 原效果（原版）：对所有敌人造成7(11)点伤害。激发你的所有充能球两次。
- 新效果：对所有敌人造成14(20)点伤害。激发你的所有充能球两次。

#### 烟囱

- 原效果：状态牌获得虚无。每当你消耗一张状态牌，获得5(7)点格挡。
- 新效果：状态牌获得虚无。每当你消耗一张状态牌，获得4(5)点格挡。

#### 人工合成

- 原效果（原版）：造成14(20)点伤害。你打出的下一张能力牌耗能变为0费。
- 新效果：造成14(20)点伤害。你手牌中的一张随机牌在本回合内免费打出。

#### 骚动

- 原效果：造成6点伤害两次。从你的抽牌堆中随机打出1(2)张攻击牌。
- 新效果：造成6(8)点伤害两次。(对该敌人)从你的抽牌堆中随机打出1张攻击牌。

#### 电流相生

- 原始费用（原版）：3
- 新费用：3(2)
- 原效果：生成等量于你在这场战斗中生成过的闪电充能球数量的闪电充能球。（-消耗。）
- 新效果：生成等量于你在这场战斗中生成过的闪电充能球数量的闪电充能球。

</details>

## 0.99.0

<details>

<summary>English</summary>

Supports STS2 beta v0.107.0.

### Card Changes

#### Adaptive Strike

- Effect: Deal 18(23) damage. Transform a card in your Discard Pile into a 0[Energy] copy of this card.

#### Barrage

- Effect: Deal 4(6) damage to ALL enemies for each Channeled Orb.

#### Cold Snap

- Effect: Deal 8(11) damage. Channel 1 Frost.

#### Consuming Shadow

- Effect: Channel 2(3) Dark. Whenever you Evoke a Dark Orb, trigger the passive ability of all other Dark Orbs.

#### Coolant

- Rarity: Rare -> Uncommon
- Effect: Whenever you Channel an Orb, gain 3(4) Block.

#### Defragment

- Rarity: Rare -> Uncommon

#### Feral

- Cost: 2(1) -> 1
- Effect: (Innate.) The first time you play a 0[Energy] Attack each turn, return it to your Hand.

#### FTL

- Effect: Deal 5(8) damage. Put a random 0[Energy] card from your Draw Pile into your Hand.

#### Glacier

- Effect: Gain 7(10) Block. Channel 2 Frost.

#### Glasswork

- Effect: Gain 6(9) Block. Channel 1 Glass.

#### Hailstorm

- Cost: 1 -> 2
- Rarity: Uncommon -> Rare
- Effect: Channel 2(3) Frost. Whenever you gain Block from a Frost Orb, deal that much damage to a random enemy.

#### Helix Drill

- Effect: Deal 3(5) damage for each 0[Energy] card played this combat.

#### Hotfix

- Effect: Gain 2(3) Focus this turn. Increase this card's cost by 1[Energy] this turn.

#### Ice Lance

- Effect: Deal 15(18) damage. Deals 5(6) additional damage for each Frost Orb Channeled this combat. Channel 3 Frost.

#### Iteration

- Effect: Whenever you draw a Status, draw 1(2) cards.

#### Leap

- Effect: Gain 8(11) Block. Reduce this card's cost to 0[Energy].

#### Loop

- Cost: 1 -> 2(1)
- Rarity: Uncommon -> Rare
- Effect: At the end of your turn, trigger the passive ability of all your Orbs 1 additional time.

#### Rainbow

- Cost: 2 -> 3
- Effect: Channel 1 Lightning. Channel 1 Frost. Channel 1 Dark. Whenever you Channel an Orb, this card costs 1[Energy]
  less until played. (Retain.)

#### Refract

- Cost: 3 -> 2
- Effect: Deal 4(7) damage twice. Channel 2 Glass.

#### Signal Boost

- Effect: Innate. The next Power you play is played an additional time. (Exhaust.)

#### Smokestack

- Rarity: Uncommon -> Rare
- Effect: Status gain Ethereal. Whenever you Exhaust a Status, gain 5(7) Block.

#### Spinner

- Rarity: Rare -> Uncommon

#### Storm

- Effect: (Innate.) Whenever you play a Power, Channel 2 Lightning.

#### Sweeping Beam

- Effect: Deal 6(9) damage to ALL enemies. Draw 1 card for each enemy.

#### Uproar

- Effect: Deal 6 damage twice. Play 1(2) random Attacks from your Draw Pile.

</details>

<details>

<summary>简体中文</summary>

支持杀戮尖塔2 beta v0.107.0 版本。

### 卡牌改动

#### 适应打击

- 效果：造成18(23)点伤害。将你弃牌堆中的一张牌变化为这张牌的0费复制品。

#### 弹幕齐射

- 效果：当前每有一个充能球，对所有敌人造成4(6)点伤害。

#### 寒流

- 效果：造成8(11)点伤害。生成1个冰霜充能球。

#### 吞噬暗影

- 效果：生成2(3)个黑暗充能球。每当你激发黑暗充能球时，触发所有其他黑暗充能球的被动。

#### 冷却剂

- 稀有度：稀有 → 罕见
- 效果：每当你生成充能球时，获得3(4)点格挡。

#### 碎片整理

- 稀有度：稀有 → 罕见

#### 野性

- 费用：2(1) → 1
- 效果：(固有) 你每回合打出的第一张耗能为0费的攻击牌，会放回你的手牌。

#### 超越光速

- 效果：造成5(8)点伤害。将你抽牌堆中的一张随机0费牌放入你的手牌。

#### 冰川

- 效果：获得7(10)点格挡。生成2个冰霜充能球。

#### 玻璃工艺

- 效果：获得6(9)点格挡。生成1个玻璃充能球。

#### 冰雹风暴

- 费用：1 → 2
- 稀有度：罕见 → 稀有
- 效果：生成2(3)个冰霜充能球。每当你从冰霜充能球获得格挡时，对随机敌人造成等量的伤害。

#### 螺旋钻击

- 效果：本场战斗中每打出过一张0费牌，此牌就造成3(5)点伤害一次。

#### 热修复

- 效果：在本回合获得2(3)点集中。本回合这张牌的耗能增加1费。

#### 冰之长枪

- 效果：造成15(18)点伤害。你在本场战斗中每生成过一个冰霜充能球，这张牌就额外造成5(6)点伤害。生成3个冰霜充能球。

#### 迭代

- 效果：每当你抽到一张状态牌时，抽1(2)张牌。

#### 飞跃

- 效果：获得8(11)点格挡。这张牌的耗能降为0费。

#### 循环

- 费用：1 → 2(1)
- 稀有度：罕见 → 稀有
- 效果：在你的回合结束时，额外触发你所有的充能球的被动1次。

#### 彩虹

- 费用：2 → 3
- 效果：生成1个闪电充能球。生成1个冰霜充能球。生成1个黑暗充能球。每当你生成充能球时，此牌耗能在下一次打出前减少1费。(保留。)

#### 折射

- 费用：3 → 2
- 效果：造成4(7)点伤害两次。生成2个玻璃充能球。

#### 信号增强

- 效果：固有。你的下一张能力牌会额外打出一次。消耗。

#### 烟囱

- 稀有度：罕见 → 稀有
- 效果：状态牌获得虚无。每当你消耗一张状态牌，获得5(7)点格挡。

#### 旋转工艺

- 稀有度：稀有 → 罕见

#### 雷暴

- 效果：(固有。)每当你打出一张能力牌时，生成2个闪电充能球。

#### 扫荡射线

- 效果：对所有敌人造成6(9)点伤害。当前每有一名敌人，就抽一张牌。

#### 骚动

- 效果：造成6点伤害两次。从你的抽牌堆中随机打出1(2)张攻击牌。

</details>
