using LunarPrinter.Compatibility;
using RoR2;
using RoR2.Items;
using System;
using System.Collections.Generic;

namespace LunarPrinter.Content;

public static class LunarItemCostType
{
    public const string CostTypeName = "LunarItem";

    internal static PurchaseInteraction? PurchaseInteraction { get; set; }

    public static void Register()
    {
        RegisterCostType();
        On.RoR2.CostTypeCatalog.Init += (orig) =>
        {
            orig();
            ApplyCostTypeIndex();
        };
    }

    private static void ApplyCostTypeIndex()
    {
        for (int i = 0; i < CostTypeCatalog.costTypeCount; i++)
        {
            if (CostTypeCatalog.GetCostTypeDef((CostTypeIndex)i)?.name == CostTypeName)
            {
                PurchaseInteraction?.costType = (CostTypeIndex)i;
                return;
            }
        }
    }

    internal static void RegisterCostType()
    {
        CostTypeCatalog.modHelper.getAdditionalEntries += list =>
        {
            CostTypeDef baseCostType = CostTypeCatalog.GetCostTypeDef(CostTypeIndex.LunarItemOrEquipment);
            CostTypeDef lunarPrinterCostType = new()
            {
                name = CostTypeName,
                itemTier = baseCostType.itemTier,
                colorIndex = baseCostType.colorIndex,
                costStringFormatToken = baseCostType.costStringFormatToken,
                costStringStyle = baseCostType.costStringStyle,
                saturateWorldStyledCostString = baseCostType.saturateWorldStyledCostString,
                darkenWorldStyledCostString = baseCostType.darkenWorldStyledCostString,

                buildCostString = (ctd, ctx) => baseCostType.BuildCostString(ctx.cost, ctx.stringBuilder),
                getCostColor = (ctd, ctx) => baseCostType.GetCostColor(ctx.forWorldDisplay),
                buildCostStringStyled = (ctd, ctx) => baseCostType.BuildCostStringStyled(ctx.cost, ctx.stringBuilder, ctx.forWorldDisplay, ctx.includeColor),
                isAffordable = IsAffordable,

                payCost = PayCost,
            };

            list.Add(lunarPrinterCostType);
        };
    }

    internal static bool IsAffordable(CostTypeDef costTypeDef, CostTypeDef.IsAffordableContext context)
    {
        if (context.activator.TryGetComponent<CharacterBody>(out var body))
        {
            Inventory inventory = body.inventory;
            if (inventory == null || inventory.inventoryDisabled) return false;
            int cost = context.cost;
            int count = 0;
            for (int i = 0; i < ItemCatalog.lunarItemList.Count; i++)
            {
                count += inventory.GetItemCountPermanent(ItemCatalog.lunarItemList[i]);
                if (count >= cost) return true;
            }
        }
        return false;
    }

    internal static void PayCost(CostTypeDef.PayCostContext context, CostTypeDef.PayCostResults result)
    {
        Inventory inventory = context.activatorInventory;
        if (inventory == null) return;

        bool paidWithScrap = LunarScrapCompatibility.Enabled && LunarScrapCompatibility.TryPayWithLunarScrap(inventory, result);

        if (!paidWithScrap)
        {
            PayWithRandomLunarItems(inventory, context, result);
        }

        MultiShopCardUtils.OnNonMoneyPurchase(context);
    }

    internal static void PayWithRandomLunarItems(Inventory inventory, CostTypeDef.PayCostContext context, CostTypeDef.PayCostResults result)
    {
        ItemIndex avoided = context.avoidedItemIndex;
        var candidates = new List<ItemIndex>();
        foreach (ItemIndex itemIndex in ItemCatalog.lunarItemList)
        {
            if (itemIndex == avoided) continue;
            int count = inventory.GetItemCountPermanent(itemIndex);
            for (int i = 0; i < count; i++)
            {
                candidates.Add(itemIndex);
            }
        }
        // Only fall back to the avoided item if nothing else can cover the cost
        if (candidates.Count < context.cost)
        {
            int shortfall = context.cost - candidates.Count;
            int avoidedCount = avoided != ItemIndex.None ? inventory.GetItemCountPermanent(avoided) : 0;
            for (int i = 0; i < Math.Min(shortfall, avoidedCount); i++)
            {
                candidates.Add(avoided);
            }
        }
        for (int i = 0; i < context.cost && candidates.Count > 0; i++)
        {
            int pick = context.rng.RangeInt(0, candidates.Count);
            var transform = new Inventory.ItemTransformation
            {
                allowWhenDisabled = true,
                forbidTempItems = true,
                minToTransform = 1,
                maxToTransform = 1,
                originalItemIndex = candidates[pick],
                newItemIndex = ItemIndex.None
            };
            if (transform.TryTransform(inventory, out var transformResult))
            {
                result.AddTakenItemsFromTransformation(in transformResult);
            }
            candidates.RemoveAt(pick);
        }
    }

}
