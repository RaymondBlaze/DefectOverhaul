# 故障机器人重置 - 杀戮尖塔 2 模组

[English](README.md) | [简体中文](README.zhs.md)

<img src="godot/DefectOverhaul/mod_image.png" alt="base" width="50%">

重做了故障机器人的游戏体验。

## 目前的功能

- 修改了 25 张[卡牌](docs/zhs/CARDS.md)，每张卡牌的修改都可以通过 STS2-RitsuLib 的配置界面在游戏内启用/禁用。
- 支持英文和简体中文本地化。

## 前置

需要 [STS2-RitsuLib](https://github.com/BAKAOLC/STS2-RitsuLib) 作为前置。

## 安装

1. 从 [Releases](https://github.com/RaymondBlaze/DefectOverhaul/releases) 下载 `DefectOverhaul-<version>.zip`。
2. 解压其内容至游戏安装路径下的 `mods` 文件夹。
3. 按 STS2-RitsuLib 的 [README](https://github.com/BAKAOLC/STS2-RitsuLib/README.md) 安装 `STS2-RitsuLib`。
4. 启动游戏，在设置里中启用这两个模组。

## 从源码构建

- 环境：.NET 9.0 / Godot 4.5 / 杀戮尖塔2 本地安装.
- `git clone` 本项目。
- 从模板创建 `godot/local.props`，设置好杀戮尖塔2和 Godot 的路径。
- 运行`dotnet build godot/DefectOverhaul.csproj`.
- 模组和 `STS2-RitsuLib` 会自动部署到杀戮尖塔2的 `mods` 文件夹，同时会在项目 `build` 文件夹下创建分发用的ZIP。
