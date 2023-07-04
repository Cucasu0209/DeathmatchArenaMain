using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    public GameObject WeaponOptionBG;
    public GameObject HatOptionBG;
    public GameObject ShoeOptionBG;

    public GameObject WeaponList;
    public GameObject HatList;
    public GameObject ShoeList;

    public TextMeshProUGUI MyCoin;

    private void Start()
    {
        ShowWeaponList();
       
    }

    private void OnEnable()
    {
        ShowCurrency();
        PlayerData.OnCurrencyChange += ShowCurrency;
        PlayerData.OnHatOwnedChange += ShowCurrentList;
        PlayerData.OnShoeOwnedChange += ShowCurrentList;
        PlayerData.OnWeaponOwnedChange += ShowCurrentList;
    }

    private void OnDisable()
    {
        PlayerData.OnCurrencyChange -= ShowCurrency;
        PlayerData.OnHatOwnedChange -= ShowCurrentList;
        PlayerData.OnShoeOwnedChange -= ShowCurrentList;
        PlayerData.OnWeaponOwnedChange -= ShowCurrentList;
    }
    int currenlist = 0;
    public void ShowCurrentList()
    {
        if (currenlist == 0) ShowWeaponList();
        else if (currenlist == 1) ShowHatList();
        else if (currenlist == 2) ShowShoeList();
    }

    public void ShowWeaponList()
    {
        currenlist = 0;
        ShowList(ItemType.Weapon);
    }
    public void ShowHatList()
    {
        currenlist = 1;
        ShowList(ItemType.Hat);
    }
    public void ShowShoeList()
    {
        currenlist = 2;
        ShowList(ItemType.Shoe);
    }
    public void ShowList(ItemType type)
    {
        WeaponOptionBG.SetActive(false);
        HatOptionBG.SetActive(false);
        ShoeOptionBG.SetActive(false);

        WeaponList.SetActive(false);
        HatList.SetActive(false);
        ShoeList.SetActive(false);

        switch (type)
        {
            case ItemType.Weapon:
                WeaponOptionBG.SetActive(true);
                WeaponList.SetActive(true);
                break;
            case ItemType.Hat:
                HatOptionBG.SetActive(true);
                HatList.SetActive(true);
                break;
            case ItemType.Shoe:
                ShoeOptionBG.SetActive(true);
                ShoeList.SetActive(true);
                break;
        }
    }
    private void ShowCurrency()
    {
        Debug.Log("Currency update ui");
        MyCoin.SetText(PlayerData.GetCurrency().ToString());
    }
}
