using System.Collections.Generic;
using UnityEngine;

public static class SpriteLib
{

    private static readonly Dictionary<UnitType, string> SpritePath = new()
    {
        [UnitType.Farmer] = "UnitSprites/FarmerGarrison",
        [UnitType.Knight] = "UnitSprites/KnightGarrison",
        [UnitType.GoldKnight] = "UnitSprites/GoldenKnightGarrison",
        [UnitType.Golem] = "UnitSprites/GolemGarrison",
        [UnitType.Archer] = "UnitSprites/ArcherGarrison",
        [UnitType.Wizard] = "UnitSprites/WizardGarrison",
    };

    public static Sprite getUnitSprite(UnitType key)
    {
        return Resources.Load<Sprite>(SpritePath[key]);
    }

}
