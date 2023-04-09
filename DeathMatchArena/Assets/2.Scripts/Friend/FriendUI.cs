using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Containers;

public class FriendUI : MonoBehaviour
{
    public Image FriendBtnBG, RequestBtnBG, FindBtnBG;
    public UIContainer FriendContainer, RequestContainer, FindContainer;
    public enum FriendContainerUIType
    {
        Friend,
        Request,
        Find
    }

    private FriendContainerUIType currentContainer;

    private void Awake()
    {
        FriendBtnBG.color = new Color(FriendBtnBG.color.r, FriendBtnBG.color.g, FriendBtnBG.color.b, 1);
        RequestBtnBG.color = new Color(RequestBtnBG.color.r, RequestBtnBG.color.g, RequestBtnBG.color.b, 0);
        FindBtnBG.color = new Color(FindBtnBG.color.r, FindBtnBG.color.g, FindBtnBG.color.b, 0);
        FriendContainer.InstantShow();
        RequestContainer.InstantHide();
        FindContainer.InstantHide();
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }

    public void ShowFriendContainer()
    {
        ShowContainer(FriendContainerUIType.Friend);
    }
    public void ShowRequestContainer()
    {
        ShowContainer(FriendContainerUIType.Request);
    }
    public void ShowFindContainer()
    {
        ShowContainer(FriendContainerUIType.Find);
    }

    public void ShowContainer(FriendContainerUIType container)
    {
        if (container == currentContainer) return;
        currentContainer = container;

        FriendBtnBG.DOFade(0, 0.2f);
        RequestBtnBG.DOFade(0, 0.2f);
        FindBtnBG.DOFade(0, 0.2f);

        FriendContainer.Hide();
        RequestContainer.Hide();
        FindContainer.Hide();


        if (container == FriendContainerUIType.Friend)
        {
            FriendBtnBG.DOKill(); FriendBtnBG.DOFade(1, 0.2f);
            FriendContainer.Show();
        }
        else if (container == FriendContainerUIType.Request)
        {
            RequestBtnBG.DOKill(); RequestBtnBG.DOFade(1, 0.2f);
            RequestContainer.Show();
        }
        else if (container == FriendContainerUIType.Find)
        {
            FindBtnBG.DOKill(); FindBtnBG.DOFade(1, 0.2f);
            FindContainer.Show();
        }


    }

}
