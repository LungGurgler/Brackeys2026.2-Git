using System.Collections.Generic;
using UnityEngine;



public static class UnitLib 
{


    private static readonly Dictionary<UnitType, string> UnitPath = new()
    {
        [UnitType.Farmer] = "UnitPrefabs/Farmer",
        [UnitType.Knight] = "UnitPrefabs/Archer",
        [UnitType.GoldKnight] = "UnitPrefabs/GoldKnight",
        [UnitType.Golem] = "UnitPrefabs/Golem",
        [UnitType.Archer] = "UnitPrefabs/Swordsman",
        [UnitType.Wizard] = "UnitPrefabs/Wizard",
        [UnitType.Catapult] = "UnitPrefabs/",

    };

    public static GameObject getUnit(UnitType key)
    {
        return Resources.Load<GameObject>(UnitPath[key]); 
    }
}
