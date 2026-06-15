using DefectOverhaul.Patches.Cards;
using DefectOverhaul.Patches.Gameplay;
using STS2RitsuLib;
using STS2RitsuLib.Settings;
using STS2RitsuLib.Utils;

namespace DefectOverhaul.Settings;

public static class DefectOverhaulSettingsPage {
    private static readonly I18N i18N = new(
        $"{Consts.ModId}.Settings",
        pckFolders: ["res://DefectOverhaul/settings/localization"]
    );

    internal static ModSettingsText Loc(string key, string fallback) {
        return ModSettingsText.I18N(i18N, key, fallback);
    }

    public static void Initialize() {
        RitsuLibFramework.RegisterModSettings(
            Consts.ModId, page => {
                page.WithModDisplayName(Loc("defectoverhaul.mod.display_name", "Defect Overhaul"))
                    .WithTitle(Loc("defectoverhaul.page.title", "Settings"));
                GameplayPatches.ConfigureSettingsPage(page, Loc);
                CardPatches.ConfigureSettingsPage(page, Loc);
            }
        );
    }
}