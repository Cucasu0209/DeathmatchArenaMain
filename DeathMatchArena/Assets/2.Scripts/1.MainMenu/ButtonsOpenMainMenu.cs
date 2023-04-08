using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonsOpenMainMenu : MonoBehaviour
{
    public Image PlayGameArrow;
    public Image SettingArrow;
    public Image FriendArrow;
    public Image RankArrow;
    public Image ShopArrow;
    private void Awake()
    {
        PlayGameArrow.color = new Color(PlayGameArrow.color.r, PlayGameArrow.color.g, PlayGameArrow.color.b, 1);
        SettingArrow.color = new Color(SettingArrow.color.r, SettingArrow.color.g, SettingArrow.color.b, 0);
        FriendArrow.color = new Color(FriendArrow.color.r, FriendArrow.color.g, FriendArrow.color.b, 0);
        RankArrow.color = new Color(RankArrow.color.r, RankArrow.color.g, RankArrow.color.b, 0);
        ShopArrow.color = new Color(ShopArrow.color.r, ShopArrow.color.g, ShopArrow.color.b, 0);
    }

    public void ShowPlayHome()
    {
        ShowContainer(MainMenuSceneController.ContainerType.PlayGame);
    }
    public void ShowSetting()
    {
        ShowContainer(MainMenuSceneController.ContainerType.Setting);
    }
    public void ShowFriend()
    {
        ShowContainer(MainMenuSceneController.ContainerType.Friend);
    }
    public void ShowRank()
    {
        ShowContainer(MainMenuSceneController.ContainerType.Rank);
    }
    public void ShowShop()
    {
        ShowContainer(MainMenuSceneController.ContainerType.Shop);
    }
    public void ShowContainer(MainMenuSceneController.ContainerType type)
    {
        PlayGameArrow.DOFade(0, 0.2f);
        SettingArrow.DOFade(0, 0.2f);
        FriendArrow.DOFade(0, 0.2f);
        RankArrow.DOFade(0, 0.2f);
        ShopArrow.DOFade(0, 0.2f);


        if (type == MainMenuSceneController.ContainerType.PlayGame)
        {
            PlayGameArrow.DOKill(); PlayGameArrow.DOFade(1, 0.2f);
        }
        if (type == MainMenuSceneController.ContainerType.Setting)
        {
            SettingArrow.DOKill(); SettingArrow.DOFade(1, 0.2f);
        }
        if (type == MainMenuSceneController.ContainerType.Friend)
        {
            FriendArrow.DOKill(); FriendArrow.DOFade(1, 0.2f);
        }
        if (type == MainMenuSceneController.ContainerType.Rank)
        {
            RankArrow.DOKill(); RankArrow.DOFade(1, 0.2f);
        }
        if (type == MainMenuSceneController.ContainerType.Shop)
        {
            ShopArrow.DOKill(); ShopArrow.DOFade(1, 0.2f);
        }
        MainMenuSceneController.Instance.ShowContainer(type);
    }
}
