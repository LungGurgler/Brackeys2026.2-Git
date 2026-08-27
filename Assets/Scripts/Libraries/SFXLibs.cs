using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public enum SFXKeys
{
    MenuBtnPressed,
    UIButtonHover,
}

public static class SFXLib
{


    private static readonly Dictionary<SFXKeys, string> filePaths = new() //Static Readonly because you can't const a Dictionary in C# apparently
    {
        [SFXKeys.MenuBtnPressed] = "Sounds/SFX/UI_Buttons/Retro8",
        [SFXKeys.UIButtonHover] = "Sounds/SFX/UI_Buttons/Retro2",
    };

    public static AudioClip getSoundClip(SFXKeys key)
    {
        return Resources.Load<AudioClip>(filePaths[key]);
    }

}
