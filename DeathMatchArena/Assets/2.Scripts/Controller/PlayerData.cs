using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    #region Variables
    private static string Id = "";
    private static string NickName = "";

    private static int currentWeaponIndex = 0;
    private static int currentHatIndex = -1;
    private static int currentShoeIndex = -1;

    private static List<int> weaponOwned = new List<int>() { 0, 2, 4, 5 };
    private static List<int> hatOwned = new List<int>() { 0, 1, 3 };
    private static List<int> shoeOwned = new List<int>() { 2, 3, 5 };
    #endregion

    #region Event
    public static event Action OnNickNameChange;
    public static event Action OnIdChange;

    public static event Action OnCurrentWeaponChange;
    public static event Action OnCurrentHatChange;
    public static event Action OnCurrentShoeChange;

    public static event Action OnWeaponOwnedChange;
    public static event Action OnHatOwnedChange;
    public static event Action OnShoeOwnedChange;
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
    public static int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }
    public static int GetCurrentHatIndex()
    {
        return currentHatIndex;
    }
    public static int GetCurrentShoeIndex()
    {
        return currentShoeIndex;
    }
    public static List<int> GetWeaponOwned()
    {
        return weaponOwned;
    }
    public static List<int> GetHatOwned()
    {
        return hatOwned;
    }
    public static List<int> GetShoeOwned()
    {
        return shoeOwned;
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

    public static void SetCurrentWeaponIndex(int value)
    {
        currentWeaponIndex = value;
        OnCurrentWeaponChange?.Invoke();
    }
    public static void SetCurrentHatIndex(int value)
    {
        currentHatIndex = value;
        OnCurrentHatChange?.Invoke();
    }
    public static void SetCurrentShoeIndex(int value)
    {
        currentShoeIndex = value;
        OnCurrentShoeChange?.Invoke();
    }

    public static void AddWeaponOwned(int value)
    {
        if (weaponOwned.Contains(value) == false) weaponOwned.Add(value);
        OnWeaponOwnedChange?.Invoke();
    }
    public static void AddHatOwned(int value)
    {
        if (hatOwned.Contains(value) == false) hatOwned.Add(value);
        OnHatOwnedChange?.Invoke();
    }
    public static void AddShoeOwned(int value)
    {
        if (shoeOwned.Contains(value) == false) shoeOwned.Add(value);
        OnShoeOwnedChange?.Invoke();
    }

    #endregion
}
