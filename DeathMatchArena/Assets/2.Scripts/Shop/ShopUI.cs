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
    public void ShowWeaponList()
    {
        ShowList(ItemType.Weapon);
    }
    public void ShowHatList()
    {
        ShowList(ItemType.Hat);
    }
    public void ShowShoeList()
    {
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

        switch(type)
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

}
