using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalClientData
{
    #region Key
    private static readonly string KEY_USER_NAME = "KEY_USER_NAME";
    private static readonly string KEY_PASSWORD = "KEY_PASSWORD";

    private static readonly string Default_Username = "trunghb123";
    private static readonly string Default_Password = "trunghb123";
    #endregion

    #region Get Key
    public static string GetUsernameKey()
    {
        return KEY_USER_NAME;
    }
    public static string GetPasswordKey()
    {
        return KEY_PASSWORD;
    }
    #endregion

    #region Load Data
    public static string LoadUsername()
    {
        if (PlayerPrefs.GetString(GetUsernameKey(), Default_Username) == "trung1234") return "trung123";
        if (PlayerPrefs.GetString(GetUsernameKey(), Default_Username) == "trung123") return "trung1234";
        return PlayerPrefs.GetString(GetUsernameKey(),Default_Username);
    }
    public static string LoadPassword()
    {
        return PlayerPrefs.GetString(GetPasswordKey(), Default_Password);
    }
    #endregion

    #region Save Data
    public static void SaveUsername(string username)
    {
        Debug.Log("Save UserName: " + username);
        PlayerPrefs.SetString(GetUsernameKey(), username);
    }
    public static void SavePassword(string password)
    {
        PlayerPrefs.SetString(GetPasswordKey(), password);
    }
    #endregion
}
