using BepInEx.Bootstrap;
using Faust.QoLChests.Handlers;
using Faust.QoLChests.Api;
using System.Runtime.CompilerServices;

namespace LunarPrinter.Compatibility;

internal static class QoLChestsCompatibility
{
    private static bool? _enabled;

    internal static bool Enabled
    {
        get
        {
            _enabled ??= Chainloader.PluginInfos.ContainsKey("Faust.QoLChests");
            return (bool)_enabled;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void RegisterLunarPrinterHighlighting()
    {
        InteractableRegistry.Register(Content.LunarPrinter.name, InteractableCategory.Duplicator);
    }
}
