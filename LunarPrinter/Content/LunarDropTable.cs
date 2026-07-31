using RoR2;
using System.Collections.Generic;

namespace LunarPrinter.Content;

/// <summary>
/// A drop table that contains only lunar items (no equipment).
/// </summary>
public class LunarDropTable : PickupDropTable
{
    private readonly List<PickupIndex> _lunarItemPickups = [];

    public override void Regenerate(Run run)
    {
        _lunarItemPickups.Clear();
        foreach (PickupIndex p in run.availableLunarItemDropList)
        {
            _lunarItemPickups.Add(p);
        }
    }

    public override int GetPickupCount()
    {
        return _lunarItemPickups.Count;
    }

    public override UniquePickup GeneratePickupPreReplacement(Xoroshiro128Plus rng)
    {
        if (_lunarItemPickups.Count == 0)
        {
            return UniquePickup.none;
        }
        return new UniquePickup(_lunarItemPickups[rng.RangeInt(0, _lunarItemPickups.Count)]);
    }
}


