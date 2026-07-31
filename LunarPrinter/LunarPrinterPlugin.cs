using BepInEx;
using BepInEx.Configuration;
using Faust.QoLChests;
using LunarPrinter.Compatibility;
using LunarScrap;
using R2API;
using R2API.Utils;

namespace LunarPrinter;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(PrefabAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(DirectorAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(LanguageAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency(LunarScrapPlugin.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(QoLChests.PluginGUID, BepInDependency.DependencyFlags.SoftDependency)]
[NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
public class LunarPrinterPlugin : BaseUnityPlugin
{
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "Geritos";
    public const string PluginName = "Lunar_Printer";
    public const string PluginVersion = "1.0.0";
    internal static ConfigEntry<int> CreditCost;
    internal static ConfigEntry<int> SelectionWeight;
    public void Awake()
    {
        Log.Init(Logger);
        BindConfig();
        Content.LunarPrinter.Init();
        Content.LunarPrinter.Register();
        ApplyExtensions();
    }

    private void ApplyExtensions()
    {
        if (QoLChestsCompatibility.Enabled)
        {
            QoLChestsCompatibility.RegisterLunarPrinterHighlighting();
        }
    }

    private void BindConfig()
    {
        CreditCost = Config.Bind("Lunar Printer", 
            "Credit Cost",
            15, 
            "Credit cost for the director, change this to adjust spawn cost (higher ≈ more rare). The default value is the same as Mili-Tech (Red) Printer on Sky Meadow."
        );
        SelectionWeight = Config.Bind("Lunar Printer", 
            "Selection Weight", 
            1, 
            "Selection weight for the director, change this to adjust spawn frequency (lower = more rare). The default value is the same as Mili-Tech (Red) Printer on Sky Meadow."
        );
    }
}
