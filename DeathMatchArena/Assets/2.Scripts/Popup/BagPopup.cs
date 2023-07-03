using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagPopup : BasePopup
{
    public static BagPopup Instance;
    public PlayerReviewUI playerReview;

    [Header("Bag")]
    public BagItemUI[] BagWeapons;
    public BagItemUI[] BagHats;
    public BagItemUI[] BagShoes;
    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }
    private void OnEnable()
    {
        PlayerData.OnCurrentWeaponChange += ShowDetail;
        PlayerData.OnCurrentHatChange += ShowDetail;
        PlayerData.OnCurrentShoeChange += ShowDetail;
    }
    private void OnDisable()
    {
        PlayerData.OnCurrentWeaponChange -= ShowDetail;
        PlayerData.OnCurrentHatChange -= ShowDetail;
        PlayerData.OnCurrentShoeChange -= ShowDetail;
    }
    public void Close()
    {
        PopupController.HideBagPopup();
    }

    public override void Show()
    {
        base.Show();
        playerReview.ShowMine();
        ShowDetail();
    }

    private void ShowDetail()
    {
     

        foreach (var item in BagWeapons)
        {
            item.GetComponent<Button>()?.onClick.RemoveAllListeners();
            item.SetEquipped(false);
            item.SetOwned(false);
        }
        foreach (var item in BagShoes)
        {
            item.GetComponent<Button>()?.onClick.RemoveAllListeners();
            item.SetEquipped(false);
            item.SetOwned(false);
        }
        foreach (var item in BagHats)
        {
            item.GetComponent<Button>()?.onClick.RemoveAllListeners();
            item.SetEquipped(false);
            item.SetOwned(false);
        }

        foreach (var id in PlayerData.GetWeaponOwned()) if (id >= 0 && id < BagWeapons.Length)
            {
                BagWeapons[id].GetComponent<Button>()?.onClick.AddListener(() =>
                {
                    PlayfabController.Instance.SetEquipWeaponPlayfab(id, null);
                });
                BagWeapons[id].SetOwned(true);
            }
        foreach (var id in PlayerData.GetHatOwned()) if (id >= 0 && id < BagHats.Length)
            {
                BagHats[id].GetComponent<Button>()?.onClick.AddListener(() =>
                {
                    PlayfabController.Instance.SetEquipHatPlayfab(id, null);
                });
                BagHats[id].SetOwned(true);
            }
        foreach (var id in PlayerData.GetShoeOwned()) if (id >= 0 && id < BagShoes.Length)
            {
                BagShoes[id].GetComponent<Button>()?.onClick.AddListener(() =>
                {
                    PlayfabController.Instance.SetEquipShoePlayfab(id, null);
                });
                BagShoes[id].SetOwned(true);
            }

        if (PlayerData.GetCurrentWeaponIndex() >= 0 && PlayerData.GetCurrentWeaponIndex() < BagWeapons.Length) BagWeapons[PlayerData.GetCurrentWeaponIndex()].SetEquipped(true);
        if (PlayerData.GetCurrentHatIndex() >= 0 && PlayerData.GetCurrentHatIndex() < BagHats.Length) BagHats[PlayerData.GetCurrentHatIndex()].SetEquipped(true);
        if (PlayerData.GetCurrentShoeIndex() >= 0 && PlayerData.GetCurrentShoeIndex() < BagShoes.Length) BagShoes[PlayerData.GetCurrentShoeIndex()].SetEquipped(true);
    }
}
