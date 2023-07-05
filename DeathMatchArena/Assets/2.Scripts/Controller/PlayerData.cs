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
    private static int currentHatIndex = 0;
    private static int currentShoeIndex = 0;

    private static List<int> weaponOwned = new List<int>() { 0, 2, 4, 5 };
    private static List<int> hatOwned = new List<int>() { 0, 1, 3 };
    private static List<int> shoeOwned = new List<int>() { 0, 2, 3, 5 };

    public static PlayerPlayfabInformation[] eloLeaderboard = new PlayerPlayfabInformation[5];

    private static int currency = 0;
    private static int elo = 0;
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

    public static event Action OnCurrencyChange;
    public static event Action OnEloChange;

    public static event Action OnEloLeaderboardChange;
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
        if (weaponOwned.Count < 1) weaponOwned.Add(0);
        return weaponOwned;
    }
    public static List<int> GetHatOwned()
    {
        if (hatOwned.Count < 1) hatOwned.Add(0);
        return hatOwned;
    }
    public static List<int> GetShoeOwned()
    {
        if (shoeOwned.Count < 1) shoeOwned.Add(0);
        return shoeOwned;
    }
    public static int GetCurrency()
    {
        Debug.Log("Currency" + currency);
        return currency;
    }
    public static int GetElo()
    {
        Debug.Log("Elo" + elo);
        return elo;
    }
    public static PlayerPlayfabInformation GetEloLeaderboard(int index)
    {
        if (index >= 0 && index <= 4) return eloLeaderboard[index];
        return null;
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
    public static void SetWeaponOwned(List<int> value)
    {
        weaponOwned = value;
        OnWeaponOwnedChange?.Invoke();
    }
    public static void SetHatOwned(List<int> value)
    {
        hatOwned = value;
        OnHatOwnedChange?.Invoke();
    }
    public static void SetShoeOwned(List<int> value)
    {
        shoeOwned = value;
        OnShoeOwnedChange?.Invoke();
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
    public static void AddCurrency(int value)
    {
        currency += value;
        OnCurrencyChange?.Invoke();
    }
    public static void SetCurrency(int value)
    {
        currency = value;
        Debug.Log("currency +" + currency);
        OnCurrencyChange?.Invoke();
    }
    public static void AddElo(int value)
    {
        elo += value;
        OnEloChange?.Invoke();
    }
    public static void SetElo(int value)
    {
        elo = value;
        Debug.Log("elo +" + elo);
        OnEloChange?.Invoke();
    }
    public static void SetEloLeaderboard(List<PlayerPlayfabInformation> list)
    {
        eloLeaderboard = new PlayerPlayfabInformation[5];

        for (int i = 0; i < Mathf.Min(eloLeaderboard.Length, list.Count); i++)
        {
            eloLeaderboard[i] = list[i];
        }
        OnEloLeaderboardChange?.Invoke();
    }

    #endregion
}
