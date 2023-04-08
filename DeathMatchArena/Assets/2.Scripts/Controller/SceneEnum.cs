using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneEnum
{
    public enum Type
    {
        Root,
        Loading,
        Login,
        MainMenu,

    }
    private static string[] SceneString = new string[]
    {
        "0.Root",
        "1.Loading",
        "2.Login",
        "3.MainMenu",
    };
    public static string GetSceneString(Type type)
    {
        return SceneString[(int)type];
    }
}
