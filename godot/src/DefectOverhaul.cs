using System.Reflection;
using DefectOverhaul.Patches.Cards;
using DefectOverhaul.Settings;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Data;
using STS2RitsuLib.Interop;

namespace DefectOverhaul;

[ModInitializer(nameof(Initialize))]
public static class DefectOverhaul {
    public static readonly Logger Logger = RitsuLibFramework.CreateLogger(Consts.Id);
    public static readonly ModDataStore DataStore = RitsuLibFramework.GetDataStore(Consts.Id);

    public static void Initialize() {
        ModTypeDiscoveryHub.RegisterModAssembly(Consts.Id, Assembly.GetExecutingAssembly());
        CardPatches.Initialize();
        DefectOverhaulSettingsPage.Initialize();
        Logger.Info($"{Consts.Id} v{Consts.Version} for STS2 v{Consts.Sts2Version} initialized.");
    }
}