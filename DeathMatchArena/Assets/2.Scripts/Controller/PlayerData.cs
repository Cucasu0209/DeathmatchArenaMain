using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    #region Variables
    private static string Id = "";
    private static string NickName = "";
    #endregion

    #region Event
    public static event Action OnNickNameChange;
    public static event Action OnIdChange;
    #endregion

    #region Get 
    public static string GetId()
    {
        return Id;
    }
    public static string GetNickName()
    {
        return NickName;
    }
    #endregion

    #region Set
    public static void SetId(string value)
    {
        Id = value;
        OnIdChange?.Invoke();
    }
    public static void SetNickName(string value)
    {
        NickName = value;
        OnNickNameChange?.Invoke();
    }
    #endregion
}
