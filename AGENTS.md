# Slay the Spire 2 Mod

This is a Mod project for the game Slay the Spire 2, using Godot.NET.Sdk/4.5.1.

## Project Structure

```
<ProjectName>/
├── godot/                          # Mod Godot project root
│   ├── src/                        # Other C# source files
│   │   ├── .../
│   │   └── <ProjectName>Mod.cs     # Mod entrypoint
│   ├── <ProjectName>/              # Mod Resources
│   ├── <ProjectName>.json          # Mod manifest
│   ├── <ProjectName>.csproj        # Mod C# project (Godot.NET.Sdk/4.5.1)
│   ├── export_presets.cfg          # Godot PCK export presets
│   ├── project.godot               # Godot project config
│   ├── local.props.template        # Copy to local.props and fill in paths
│   └── local.props                 # Local project configs
├── images/                         # Images for .md files and external links
│   └── cards/                      # Exported images for patched cards
├── scripts/                        # Scripts
│   ├── utils/                      # Shared utilities
│   ├── docs/                       # Update external documents
│   ├── libs/                       # Update external libraries sources
│   ├── configs/                    # Configs for scripts
│   └── tools/                      # Executable tools for scripts
├── libs/                           # Decompiled libs references (read-only)
│   ├── 0Harmony/                   # Harmony sources
│   ├── GodotSharp/                 # GodotSharp sources
│   ├── sts2/                       # Game sources + extracted PCK resources
│   └── STS2-RitsuLib/              # RitsuLib sources
└── build/                          # Project build output
    ├── <ProjectName>/              # Mod build result
    └── <ProjectName>.zip           # Mod artifact for distribution
```

## Rules

The following rules MUST be followed:

- Never modify project level skills: Unless the user EXPLICITLY requested for modifying project level skill files, DO NOT 
write to the `.agents/skills` directory by any means.
- Always use IDE tools: When IDE tools provided by MCP are available, prioritize them over corresponding native tools and 
cli tools. NEVER write files WITHOUT using IDE tools, this could break IDE index badly.
- Plan before implementing: Unless the user EXPLICITLY requested for implementation NOW, DO NOT start implement anything, 
always present a plan and then ask for permission on implementing the plan. If the plan is bound to be complicated, DO 
NOT abstract it for terminal output, just write it to `.agentwork/plans` for the user to read.
- Use proper temp directory: For project related work, use `.agentwork` as the temp directory, DO NOT touch global 
temp directories.
- Use proper executables: When running inside WSL, avoid using I/O-heavy executables like git in WSL when operating project 
files on Windows, use corresponding executables on Windows instead. This rule applies recursively, e.g. if a Python script 
is known to call said executables, run the script using Window's python.
- Steady enumeration: If a work involves processing a known enumeration of items (classes, lines, etc.), split the work 
into processing only a few or even a single item at a time, focus on what you are processing, summarize the progress 
after each step and present it to the user. NEVER try loading the whole context of the enumerated items beforehand.
- Never silent degrade: If a problem feels complicated or unsolvable, DO NOT struggle alone or degrade the original goal, 
always ask for permissions before moving to alternatives that won't produce the exact expected result.

## Scripts

### docs/

- `update_docs.py`: Update all external docs to `docs/`.
- `update_ritsulib_docs.py`: Update STS2-RitsuLib docs to `docs/STS2-RitsuLib`
- `update_modding_tutorials.py`: Update SlayTheSpire2ModdingTutorials to `docs/SlayTheSpire2ModdingTutorials`.
- `create_cards_md.py`: Creates `CARDS.md`, listing all exported card images from `images/cards`.

### libs/

- `update_libs.py`: Update all specified libraries sources and game resources to `libs/`.
- `decompile_game_libs.py`: Decompile specified game DLLs.
- `decompile_nuget_packages.py`: Decompile specified NuGet packages.
- `extract_game_pck.py`: Extract resources from game PCK to `libs/sts2/resources`.

Configured by `scripts/configs/update_libs.json`:
```json lines
{
  "game_dlls": [
    // Name of the dlls under ($Sts2DataDir) to decompile
  ],
  "nuget_packages": [
    // Name of the NuGet packages to decompile
  ],
  "extract_game_pck": true // Whether to extract the game PCK for resources
}
```

## References

### Project Libraries

The project's `libs` directory contains binary, decompiled sources and resources of the game and other used libraries, 
these are always the most reliable source of information.

If an ILSpy MCP is available, combine ILSpy MCP tool calls with decompiled sources reading to improve efficiency.

### STS2-RitsuLib

This project uses the [`STS2-RitsuLib`](https://github.com/BAKAOLC/STS2-RitsuLib) modding framework library.

Refer to [STS2-RitsuLib documents](https://sts2-ritsulib.ritsukage.com/guide/) for basic API usage, but before implementing don't forget 
to check the decompiled sources to make sure.

Prefer to use the scripts to download the documents and read from local files instead of fetching from web.

### Modding Tutorials

You may also refer to [SlayTheSpire2ModdingTutorials](https://tutorials.sts2modding.com).

Keep in mind that since this project uses RitsuLib, you should check the RitsuLib section and avoid the BaseLib section.

Prefer to use the scripts to download the documents and read from local files instead of fetching from web.

### Wiki & Database

- [STS2 Wiki](https://slaythespire2.gg/): Community wiki site (English) for STS2.
- [STS2 Chinese Wiki](https://sts2.huijiwiki.com/wiki): Community wiki site (Chinese) for STS2.
- [Spire Codex](https://spire-codex.com): Community database site for STS2, provides API endpoints for searching 
game data, see the [documents](https://spire-codex.com/docs).
