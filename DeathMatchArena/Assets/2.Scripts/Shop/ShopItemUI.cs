using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public ItemType type;
    public int itemIndex;

    [Header("UI")]
    public TextMeshProUGUI attack;
    public TextMeshProUGUI defense;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI cost;
    public Button BuyButton;
    public TextMeshProUGUI owned;

    private void OnEnable()
    {
        Show();
    }

    public void Show()
    {
        ItemData data = ItemSkinController.GetData(type, itemIndex);

        attack.SetText("+" + data.Attack);
        defense.SetText("+" + data.Defense);
        speed.SetText("+" + data.Speed);
        cost.SetText(data.Cost.ToString());

        bool isowned = false;
        switch (type)
        {
            case ItemType.Weapon:
                isowned = PlayerData.GetWeaponOwned().Contains(itemIndex);
                break;
            case ItemType.Hat:
                isowned = PlayerData.GetHatOwned().Contains(itemIndex);
                break;
            case ItemType.Shoe:
                isowned = PlayerData.GetShoeOwned().Contains(itemIndex);
                break;
        }

        if (isowned)
        {
            owned.gameObject.SetActive(true);
            BuyButton.gameObject.SetActive(false);
        }
        else
        {
            owned.gameObject.SetActive(false);
            BuyButton.gameObject.SetActive(true);
            BuyButton.onClick.AddListener(() =>
            {
                PopupController.ShowYesNoPopup($"Are you sure to buy this item with cost {data.Cost}?", () =>
                {
                },
                () =>
                {
                });
            });
        }
    }
}
