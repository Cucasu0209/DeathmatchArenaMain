using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSkinController : MonoBehaviour
{
    public const string LinkHatPrefab = "Skin/Hat/";
    public static string[] HatNames = new string[]
    {
        "Hat1",
        "Hat2",
        "Hat3",
        "Hat4",
        "Hat5",
        "Hat6",
     };

    public static string LinkShoePrefab = "Skin/Shoe/";
    public static string[] ShoeNames = new string[]
    {
        "Shoe1",
        "Shoe2",
        "Shoe3",
        "Shoe4",
        "Shoe5",
        "Shoe6",
    };


    public const string LinkWeaponPrefab = "Skin/Weapon/";
    public static string[] WeaponNames = new string[]
    {
        "Weapon1",
        "Weapon2",
        "Weapon3",
        "Weapon4",
        "Weapon5",
        "Weapon6",
    };

    public static ItemData GetData(ItemType type, int index)
    {
        switch (type)
        {
            case ItemType.Weapon:
                if (index < 0 || index >= WeaponNames.Length) break;
                BaseWeapon weapon = Resources.Load<BaseWeapon>(LinkWeaponPrefab + WeaponNames[index]);
                return weapon.data;
            case ItemType.Hat:
                if (index < 0 || index >= HatNames.Length) break;
                Hat hat = Resources.Load<Hat>(LinkHatPrefab + HatNames[index]);
                return hat.data;
            case ItemType.Shoe:
                if (index < 0 || index >= ShoeNames.Length) break;
                Shoe shoe = Resources.Load<Shoe>(LinkShoePrefab + ShoeNames[index]);
                return shoe.data;
        }
        return null;
    }
}
public enum ItemType
{
    Weapon,
    Hat,
    Shoe
}