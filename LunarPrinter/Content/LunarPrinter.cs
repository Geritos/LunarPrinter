using LunarPrinter.Compatibility;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace LunarPrinter.Content;

public static class LunarPrinter
{
    public static GameObject? prefab;
    public static InteractableSpawnCard? spawnCard;
    public static readonly string name = "DuplicatorBlue";
    internal static readonly string _nameToken = "DUPLICATOR_BLUE";
    public static void Init()
    {
        GameObject baseDuplicator = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/DuplicatorLarge/DuplicatorLarge.prefab").WaitForCompletion();
        prefab = PrefabAPI.InstantiateClone(baseDuplicator, name, true);

        if (prefab == null) return;
        ApplyVisuals();
        ApplyInspectInfo();
        ApplyBehavior();
        ApplyPurchaseInteraction();

        LanguageAPI.Add(_nameToken + "_NAME", "Lunar Printer");
        string lunarPrinterInspectDescription = "Allows survivors to print a lunar item if they sacrifice a random lunar item.";
        LanguageAPI.Add(_nameToken + "_INSPECT_DESCRIPTION", 
            LunarScrapCompatibility.Enabled ? 
            LunarScrapCompatibility.InspectDescription : 
            lunarPrinterInspectDescription);

        PrefabAPI.RegisterNetworkPrefab(prefab);
        CreateSpawnCard();
    }

    private static void ApplyVisuals()
    {
        SkinnedMeshRenderer renderer = prefab!.GetComponentInChildren<SkinnedMeshRenderer>(true);
        Material baseMat = new(renderer.sharedMaterials[0]);

        var mat = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon/matMoonBridge.mat").WaitForCompletion();
        var decorMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Duplicator/matDuplicator.mat").WaitForCompletion();

        baseMat.SetTexture("_MainTex", mat.GetTexture("_BlueChannelTex"));
        baseMat.SetTexture("_GreenChannelTex", decorMat.GetTexture("_MainTex"));

        renderer.sharedMaterials = [baseMat];
    }

    private static void ApplyInspectInfo()
    {
        // The inspect information of the printer itself is here, ShopTerminalBehavior holds the inspect information of the provided item.
        if (prefab!.TryGetComponent<GenericInspectInfoProvider>(out var inspectInfoProvider))
        {
            InspectDef baseInspectDef = Object.Instantiate(Addressables.LoadAssetAsync<InspectDef>("RoR2/Base/DuplicatorLarge/LargeDuplicatorInspectDef.asset").WaitForCompletion());
            baseInspectDef.Info.TitleToken = _nameToken + "_NAME";
            baseInspectDef.Info.DescriptionToken = _nameToken + "_INSPECT_DESCRIPTION";
            inspectInfoProvider.InspectInfo = baseInspectDef;
        }
    }

    private static void ApplyBehavior()
    {
        if (prefab!.TryGetComponent<ShopTerminalBehavior>(out var shopTerminal))
        {
            shopTerminal.dropTable = ScriptableObject.CreateInstance<LunarDropTable>();
            shopTerminal.itemTier = ItemTier.Lunar;
        }
    }

    private static void ApplyPurchaseInteraction()
    {
        if (prefab!.TryGetComponent<PurchaseInteraction>(out var purchaseInteraction))
        {
            purchaseInteraction.displayNameToken = _nameToken + "_NAME"; // Name used in ping tooltip
            purchaseInteraction.costType = CostTypeIndex.LunarItemOrEquipment;
            purchaseInteraction.cost = 1;

            LunarItemCostType.PurchaseInteraction = purchaseInteraction;
        }
    }

    private static void CreateSpawnCard()
    {
        InteractableSpawnCard baseCard = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion();
        spawnCard = Object.Instantiate(baseCard);
        spawnCard.name = "iscDuplicatorBlue";
        spawnCard.prefab = prefab;
        spawnCard.directorCreditCost = LunarPrinterPlugin.CreditCost!.Value;
        
    }

    public static void Register()
    {
        DirectorAPI.DirectorCardHolder duplicatorBlueCard = new()
        {
            Card = new DirectorCard
            {
                spawnCard = spawnCard,
                selectionWeight = LunarPrinterPlugin.SelectionWeight!.Value,
                minimumStageCompletions = 0,
            },
            InteractableCategory = DirectorAPI.InteractableCategory.Duplicator,
        };
        DirectorAPI.Helpers.AddNewInteractable(duplicatorBlueCard);

        LunarItemCostType.Register();
    }

}
