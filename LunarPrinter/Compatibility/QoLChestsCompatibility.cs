using BepInEx.Bootstrap;
using Faust.QoLChests;
using Faust.QoLChests.Handlers;
using Faust.QoLChests.Configs;
using System.Runtime.CompilerServices;

namespace LunarPrinter.Compatibility;

internal static class QoLChestsCompatibility
{
    private static bool? _enabled;

    internal static bool Enabled
    {
        get
        {
            _enabled ??= Chainloader.PluginInfos.ContainsKey(QoLChests.PluginGUID);
            return (bool)_enabled;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void RegisterLunarPrinterHighlighting()
    {
        InteractableRegistry.Register(Content.LunarPrinter.name, InteractableCategory.Duplicator);
    }
}
