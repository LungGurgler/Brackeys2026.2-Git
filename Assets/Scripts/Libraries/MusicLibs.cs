using UnityEngine;
using System.Collections.Generic;
using UnityEngine;


public enum MusicKeys
{
    Null,
    ExampleKey,
    ExampleKey2,
}

public static class MusicLib
{


    private static readonly Dictionary<MusicKeys, string> FilePaths = new() //Static Readonly because you can't const a Dictionary in C# apparently 
    {
        [MusicKeys.ExampleKey] = "ExampleFile/ExampleKeySong",
        [MusicKeys.ExampleKey2] = "ExampleFile / ExampleKeySong2"
    };


    public static AudioClip getMusicClip(MusicKeys key)
    {
        return Resources.Load<AudioClip>(FilePaths[key]);
    }
}
