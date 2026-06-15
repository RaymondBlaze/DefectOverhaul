using System.Reflection;
using DefectOverhaul.Patches.Cards;
using DefectOverhaul.Patches.Gameplay;
using DefectOverhaul.Settings;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Interop;

namespace DefectOverhaul;

[ModInitializer(nameof(Initialize))]
public static class DefectOverhaul {
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(Consts.ModId);
    public static readonly ModDataStore DataStore = RitsuLibFramework.GetDataStore(Consts.ModId);

    public static void Initialize() {
        ModTypeDiscoveryHub.RegisterModAssembly(Consts.ModId, Assembly.GetExecutingAssembly());
        CardPatches.Initialize();
        GameplayPatches.Initialize();
        DefectOverhaulSettingsPage.Initialize();
        Logger.Info($"{Consts.ModId} v{Consts.ModVersion} for STS2 v{Consts.Sts2Version} initialized.");
    }
}