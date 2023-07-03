using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerReviewUI : MonoBehaviour
{
    [Serializable]
    public class ItemsUI
    {
        public GameObject[] objs;

        public void HideAll()
        {
            foreach (var obj in objs) obj.SetActive(false);
        }

        public void ShowAll()
        {
            foreach (var obj in objs) obj.SetActive(true);
        }
    }

    public ItemsUI[] Weapons;
    public ItemsUI[] Hats;
    public ItemsUI[] Shoes;

    [Header("Slider")]
    public Slider AttackSlider;
    public Slider DefenseSlider;
    public Slider SpeedSlider;

    [Header("TMP")]
    public TextMeshProUGUI AttackValue;
    public TextMeshProUGUI DefenseValue;
    public TextMeshProUGUI SpeedValue;


    private void OnEnable()
    {
        ShowMine();
        PlayerData.OnCurrentWeaponChange += ShowMine;
        PlayerData.OnCurrentHatChange += ShowMine;
        PlayerData.OnCurrentShoeChange += ShowMine;
    }

    private void OnDisable()
    {
        PlayerData.OnCurrentWeaponChange -= ShowMine;
        PlayerData.OnCurrentHatChange -= ShowMine;
        PlayerData.OnCurrentShoeChange -= ShowMine;
    }

    public void HideAll()
    {
        foreach (var weapon in Weapons) weapon.HideAll();
        foreach (var hat in Hats) hat.HideAll();
        foreach (var shoe in Shoes) shoe.HideAll();
    }

    public void ShowMine()
    {
        Show(PlayerReviewController.Instance.GetPlayerInformation(PlayerData.GetId()));
    }

    public void Show(PlayerReviewEntity entity)
    {
        HideAll();

        if (entity.WeaponIndex >= 0 && entity.WeaponIndex < Weapons.Length) Weapons[entity.WeaponIndex].ShowAll();
        if (entity.HatIndex >= 0 && entity.HatIndex < Hats.Length) Hats[entity.HatIndex].ShowAll();
        if (entity.ShoeIndex >= 0 && entity.ShoeIndex < Shoes.Length) Shoes[entity.ShoeIndex].ShowAll();


        ItemData weaponData = ItemSkinController.GetData(ItemType.Weapon, entity.WeaponIndex);
        ItemData hatData = ItemSkinController.GetData(ItemType.Hat, entity.HatIndex);
        ItemData shoeData = ItemSkinController.GetData(ItemType.Shoe, entity.ShoeIndex);

        if (weaponData == null) weaponData = new ItemData();
        if (hatData == null) hatData = new ItemData();
        if (shoeData == null) shoeData = new ItemData();

        float baseAttack = 30;
        float baseDefense = 30;
        float baseSpeed = 30;


        baseAttack += weaponData.Attack;
        baseAttack += hatData.Attack;
        baseAttack += shoeData.Attack;

        baseDefense += weaponData.Defense;
        baseDefense += hatData.Defense;
        baseDefense += shoeData.Defense;

        baseSpeed += weaponData.Speed;
        baseSpeed += hatData.Speed;
        baseSpeed += shoeData.Speed;

        AttackValue.SetText(baseAttack.ToString());
        DefenseValue.SetText(baseDefense.ToString());
        SpeedValue.SetText(baseSpeed.ToString());

        AttackSlider.value = baseAttack / 100;
        DefenseSlider.value = baseDefense / 100;
        SpeedSlider.value = baseSpeed / 100;
    }


}


