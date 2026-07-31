using BepInEx.Bootstrap;
using LunarScrap;
using LunarScrap.Content;
using RoR2;
using System.Runtime.CompilerServices;

namespace LunarPrinter.Compatibility;

internal static class LunarScrapCompatibility
{
    private static bool? _enabled;

    internal static bool Enabled
    {
        get
        {
            _enabled ??= Chainloader.PluginInfos.ContainsKey(LunarScrapPlugin.PluginGUID);
            return (bool)_enabled;
        }
    }

    internal static string InspectDescription => "Allows survivors to print a lunar item if they sacrifice a lunar scrap or a random lunar item. The Lunar Printer will always take scrap before a random item.";

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static bool TryPayWithLunarScrap(Inventory inventory, CostTypeDef.PayCostResults result)
    {
        int lunarScrapCount = inventory.GetItemCountPermanent(LunarScrapItem.LunarScrapItemDef.itemIndex);
        if (lunarScrapCount <= 0) return false;

        var transform = new Inventory.ItemTransformation
        {
            allowWhenDisabled = true,
            forbidTempItems = true,
            minToTransform = 1,
            maxToTransform = 1,
            originalItemIndex = LunarScrapItem.LunarScrapItemDef.itemIndex,
            newItemIndex = ItemIndex.None
        };
        if (transform.TryTransform(inventory, out var transformResult))
        {
            result.AddTakenItemsFromTransformation(in transformResult);
        }
        return true;
    }
}
