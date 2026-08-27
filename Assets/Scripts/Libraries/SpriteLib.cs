using System.Collections.Generic;
using UnityEngine;

public static class SpriteLib
{

    private static readonly Dictionary<UnitType, string> SpritePath = new()
    {
        [UnitType.Farmer] = "UnitSprites/Farmer",
        [UnitType.Knight] = "UnitSprites/Knight",
        [UnitType.GoldKnight] = "UnitSprites/GoldKnight",
        [UnitType.Golem] = "UnitSprites/Golem",
        [UnitType.Archer] = "UnitSprites/Archer",
        [UnitType.Wizard] = "UnitSprites/Wizard",
    };

    public static Sprite getUnitSprite(UnitType key)
    {
        return Resources.Load<Sprite>(SpritePath[key]);
    }

}
