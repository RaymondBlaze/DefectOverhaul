using System.Reflection;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils.Persistence;

namespace DefectOverhaul.Patches.Gameplay;

public static class GameplayPatches {
    private const string configKey = "config/gameplay_patches";
    private const string configFile = "config/gameplay_patches.json";

    private static readonly List<(Type Type, GameplayPatchAttribute Patch)> scanResults = [];
    private static Dictionary<string, bool> patches = new();

    static GameplayPatches() {
        foreach (var type in typeof(GameplayPatches).Assembly.GetTypes()) {
            var patch = type.GetCustomAttribute<GameplayPatchAttribute>();
            if (patch != null)
                scanResults.Add((type, patch));
        }
    }

    internal static void Initialize() {
        LoadConfig();
        PatchAll();
    }

    internal static void ConfigureSettingsPage(ModSettingsPageBuilder page, Func<string, string, ModSettingsText> loc) {
        page.AddSection(
            "gameplay_patches", section => {
                section
                    .WithTitle(
                        loc(
                            "defectoverhaul.section.gameplay_patches.title",
                            "Gameplay Patches"
                        )
                    )
                    .WithDescription(
                        loc(
                            "defectoverhaul.section.gameplay_patches.description",
                            "Enable/Disable specific gameplay patches. Changes apply on next game launch."
                        )
                    );

                foreach (var (_, patch) in scanResults.OrderBy(e => e.Patch)) {
                    var binding = new ModSettingsValueBinding<Dictionary<string, bool>, bool>(
                        Consts.ModId, configKey, SaveScope.Global,
                        config => config.GetValueOrDefault(patch.Id, true),
                        (config, value) => config[patch.Id] = value
                    );

                    section.AddToggle(
                        patch.Id,
                        loc($"defectoverhaul.gameplay_patches.{patch.Id}.title", patch.Title),
                        binding,
                        loc($"defectoverhaul.gameplay_patches.{patch.Id}.description", patch.Description)
                    );
                }
            }
        );
    }

    private static void LoadConfig() {
        DefectOverhaul.DataStore.Register(
            configKey,
            configFile,
            SaveScope.Global,
            () => scanResults.ToDictionary(e => e.Patch.Id, _ => true),
            true
        );

        DefectOverhaul.DataStore.Modify<Dictionary<string, bool>>(
            configKey, saved => {
                var merged = scanResults.ToDictionary(
                    e => e.Patch.Id,
                    e => saved.GetValueOrDefault(e.Patch.Id, true)
                );
                saved.Clear();
                foreach (var kv in merged)
                    saved[kv.Key] = kv.Value;
                patches = new Dictionary<string, bool>(saved);
            }
        );
    }

    private static void PatchAll() {
        foreach (var (type, patch) in scanResults) {
            if (!patches[patch.Id]) {
                DefectOverhaul.Logger.Debug(
                    $"[{nameof(GameplayPatches)}] Patches for '{patch.Id}' disabled, skipping..."
                );
                continue;
            }

            var patcher = RitsuLibFramework.CreatePatcher(Consts.ModId, $"Gameplay.{patch.Id}");

            foreach (var nested in type.GetNestedTypes()) {
                if (!typeof(IPatchMethod).IsAssignableFrom(nested) || nested.IsAbstract)
                    continue;

                var infos = (ModPatchInfo[])typeof(IPatchMethod)
                    .GetMethod(nameof(IPatchMethod.CreatePatchInfos))!
                    .MakeGenericMethod(nested)
                    .Invoke(null, null)!;

                patcher.RegisterPatches(infos);
            }

            if (patcher.PatchAll())
                continue;

            DefectOverhaul.Logger.Error(
                $"[{nameof(GameplayPatches)}] Failed to apply patches for '{patch.Id}'!"
            );
            patches[patch.Id] = false;
        }
    }
}