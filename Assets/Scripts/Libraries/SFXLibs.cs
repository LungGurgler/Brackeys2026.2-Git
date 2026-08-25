using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public enum SFXKeys
{
    MenuBtnPressed,

}

public static class SFXLib
{


    private static readonly Dictionary<SFXKeys, string> filePaths = new() //Static Readonly because you can't const a Dictionary in C# apparently 
    {
        [SFXKeys.MenuBtnPressed] = "..."
    };

    public static AudioClip getSoundClip(SFXKeys key)
    {
        return Resources.Load<AudioClip>(filePaths[key]);
    }

}
