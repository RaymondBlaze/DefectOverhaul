using System.Reflection;
using MegaCrit.Sts2.Core.Localization;
using STS2RitsuLib;
using STS2RitsuLib.Patching.Models;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;
using STS2RitsuLib.Utils.Persistence;

namespace DefectOverhaul.Patches.Cards;

public static class CardPatches {
    private const string configKey = "config/card_patches";
    private const string configFile = "config/card_patches.json";
    private const string locPath = "res://DefectOverhaul/card_patches/localization";
    private static readonly string[] patchedLocTables = ["cards", "powers"];

    private static readonly List<(Type Type, string CardId)> cardPatchScanResults = [];
    private static Dictionary<string, bool> cardPatchResults = new();

    static CardPatches() {
        foreach (var type in typeof(CardPatches).Assembly.GetTypes()) {
            var cardPatch = type.GetCustomAttribute<CardPatchAttribute>();
            if (cardPatch != null)
                cardPatchScanResults.Add((type, cardPatch.CardId));
        }
    }

    public static bool IsCardPatched(string cardId) {
        return cardPatchResults.GetValueOrDefault(cardId, false);
    }

    internal static void Initialize() {
        LoadConfig();
        PatchAll();
        RitsuLibFramework.SubscribeLifecycle<GameReadyEvent>(_ => {
                PatchLocalization();
                LocManager.Instance.SubscribeToLocaleChange(PatchLocalization);
            }
        );
    }

    internal static void ConfigureSettingsPage(ModSettingsPageBuilder page, Func<string, string, ModSettingsText> loc) {
        page.AddSection(
            "cardPatches", section => {
                section
                    .WithTitle(
                        loc(
                            "defectoverhaul.section.cardPatches.title",
                            "Card Patches"
                        )
                    )
                    .WithDescription(
                        loc(
                            "defectoverhaul.section.cardPatches.description",
                            "Enable/Disable specific card patches. Changes apply on next game launch."
                        )
                    );
                var cardIds = cardPatchScanResults.Select(e => e.CardId).Order();
                foreach (var cardId in cardIds) {
                    var binding = new ModSettingsValueBinding<Dictionary<string, bool>, bool>(
                        Consts.Id, configKey, SaveScope.Global,
                        config => config.GetValueOrDefault(cardId, true),
                        (config, value) => config[cardId] = value
                    );

                    section.AddToggle(
                        cardId,
                        ModSettingsText.LocString("cards", $"{cardId}.title", cardId),
                        binding
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
            () => cardPatchScanResults.ToDictionary(e => e.CardId, _ => true),
            true
        );

        DefectOverhaul.DataStore.Modify<Dictionary<string, bool>>(
            configKey, saved => {
                var merged = cardPatchScanResults.ToDictionary(
                    e => e.CardId,
                    e => saved.GetValueOrDefault(e.CardId, true)
                );
                saved.Clear();
                foreach (var kv in merged)
                    saved[kv.Key] = kv.Value;
                cardPatchResults = new Dictionary<string, bool>(saved);
            }
        );
    }

    private static void PatchAll() {
        foreach (var (type, cardId) in cardPatchScanResults) {
            if (!cardPatchResults[cardId]) {
                DefectOverhaul.Logger.Debug($"[{nameof(CardPatches)}] Patches for '{cardId}' disabled, skipping...");
                continue;
            }

            var patcher = RitsuLibFramework.CreatePatcher(Consts.Id, $"Cards.{cardId}");

            foreach (var nested in type.GetNestedTypes()) {
                if (!typeof(IPatchMethod).IsAssignableFrom(nested) || nested.IsAbstract)
                    continue;

                var infos = (ModPatchInfo[])typeof(IPatchMethod).GetMethod(nameof(IPatchMethod.CreatePatchInfos))!
                    .MakeGenericMethod(nested)
                    .Invoke(null, null)!;

                patcher.RegisterPatches(infos);
            }

            if (patcher.PatchAll())
                continue;

            DefectOverhaul.Logger.Error($"[{nameof(CardPatches)}] Failed to apply patches for Card: {cardId}!");
            cardPatchResults[cardId] = false;
        }
    }

    private static void PatchLocalization() {
        var lang = LocManager.Instance.Language;
        foreach (var table in patchedLocTables) {
            var patchesJson = FileOperations.ReadJson<Dictionary<string, Dictionary<string, string>>>($"{locPath}/{lang}/{table}.json");
            if (lang != "eng")
                if (!patchesJson.Success || patchesJson.Data == null) {
                    DefectOverhaul.Logger.Warn($"[{nameof(CardPatches)}] No localization patches for '{lang}/{table}.json' available, using eng localization patches instead...");
                    patchesJson = FileOperations.ReadJson<Dictionary<string, Dictionary<string, string>>>($"{locPath}/eng/{table}.json");
                }

            if (!patchesJson.Success || patchesJson.Data == null) {
                DefectOverhaul.Logger.Error($"[{nameof(CardPatches)}] No localization patches for 'eng/{table}.json' available, failed to apply localization patches.");
                continue;
            }

            var patches = new Dictionary<string, string>();
            foreach (var (cardId, entries) in patchesJson.Data) {
                if (!IsCardPatched(cardId))
                    continue;
                foreach (var entry in entries)
                    patches[entry.Key] = entry.Value;
            }

            if (patches.Count <= 0)
                continue;
            LocManager.Instance.GetTable(table).MergeWith(patches);
            DefectOverhaul.Logger.Info($"[{nameof(CardPatches)}] Patched LocTable '{lang}/{table}.json' with {patches.Count} entries.");
        }
    }
}