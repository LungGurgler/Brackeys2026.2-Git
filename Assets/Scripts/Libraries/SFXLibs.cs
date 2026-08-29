using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public enum SFXKeys
{
    MenuBtnPressed,
    UIButtonHover,
    WaveOver,
    UnitKilled,
    WizardShoot,
    LoseGame,
    UnitHurt,
    IntroFanfare,
}

public static class SFXLib
{


    private static readonly Dictionary<SFXKeys, string> filePaths = new() //Static Readonly because you can't const a Dictionary in C# apparently
    {
        [SFXKeys.MenuBtnPressed] = "Sounds/SFX/UI_Buttons/Retro8",
        [SFXKeys.UIButtonHover] = "Sounds/SFX/UI_Buttons/Retro2",
        [SFXKeys.WaveOver] = "Sounds/SFX/Game/waveOver",
        [SFXKeys.UnitKilled] = "Sounds/SFX/Game/Units/unitKilled",
        [SFXKeys.WizardShoot] = "Sounds/SFX/Game/Units/wizardShoot",
        [SFXKeys.LoseGame] = "Sounds/SFX/Game/loseGame",
        [SFXKeys.UnitHurt] = "Sounds/SFX/Game/Units/Hurt/hurt2",
        [SFXKeys.IntroFanfare] = "Sounds/SFX/Game/introFanfare",
    };

    public static AudioClip getSoundClip(SFXKeys key)
    {
        return Resources.Load<AudioClip>(filePaths[key]);
    }

}
