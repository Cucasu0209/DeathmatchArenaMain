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
        Lobby,
        Room,
        MainGame,
        EndGame
    }
    private static string[] SceneString = new string[]
    {
        "0.Root",
        "1.Loading",
        "2.Login",
        "3.MainMenu",
        "4.Lobby",
        "5.Room",
        "6.MainGame",
        "7.EndGame",
    };
    public static string GetSceneString(Type type)
    {
        return SceneString[(int)type];
    }
}
