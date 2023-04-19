using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Containers;
using Friend.Container.Friend;
using Friend.Container.FindPlayer;
using Friend.Container.Request;
using Friend.Container.Invitation;
using System;

public class FriendUI : MonoBehaviour
{
    public static FriendUI Instance;


    #region UIComponent
    public Image FriendBtnBG, RequestBtnBG, FindBtnBG, InvitationBtnBG;
    public UIContainer FriendContainer, RequestContainer, FindContainer, InvitationContainer;
    public Transform detailPanel;

    public GameObject AcceptButton;
    public GameObject RefuseButton;
    public GameObject AddfriendButton;
    public GameObject RemovefriendButton;
    public GameObject CancelRequestButton;

    #endregion

    #region Variables
    public enum FriendContainerUIType
    {
        Friend,
        Request,
        Find,
        Invitation
    }
    private FriendContainerUIType currentContainer;



    public PlayerDetailUI detail;
    public FriendListAdapter friendList;
    public FindPlayerListAdapter findList;
    public RequestListAdapter requestList;
    public InvitationListAdapter invitationList;


    #endregion

    #region Unity
    private void Awake()
    {
        Instance = this;

        FriendBtnBG.color = new Color(FriendBtnBG.color.r, FriendBtnBG.color.g, FriendBtnBG.color.b, 1);
        RequestBtnBG.color = new Color(RequestBtnBG.color.r, RequestBtnBG.color.g, RequestBtnBG.color.b, 0);
        FindBtnBG.color = new Color(FindBtnBG.color.r, FindBtnBG.color.g, FindBtnBG.color.b, 0);
        InvitationBtnBG.color = new Color(InvitationBtnBG.color.r, InvitationBtnBG.color.g, InvitationBtnBG.color.b, 0);
        FriendContainer.InstantShow();
        RequestContainer.InstantHide();
        FindContainer.InstantHide();
        InvitationContainer.InstantHide();
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }
    #endregion

    #region Actions
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
        List<Friend.Container.FindPlayer.MyListItemModel> items = new List<Friend.Container.FindPlayer.MyListItemModel>();
        foreach (var player in OtherPlayersController.Instance.GetTempAllPlayers().Values)
        {
            items.Add(new Friend.Container.FindPlayer.MyListItemModel() { player = player });
        }
        findList.SetItems(items);
    }
    public void ShowInvitationContainer()
    {
        ShowContainer(FriendContainerUIType.Invitation);
    }
    public void ShowContainer(FriendContainerUIType container)
    {
        if (container == currentContainer) return;
        currentContainer = container;

        FriendBtnBG.DOFade(0, 0.2f);
        RequestBtnBG.DOFade(0, 0.2f);
        FindBtnBG.DOFade(0, 0.2f);
        InvitationBtnBG.DOFade(0, 0.2f);

        FriendContainer.Hide();
        RequestContainer.Hide();
        FindContainer.Hide();
        InvitationContainer.Hide();


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
        else if (container == FriendContainerUIType.Invitation)
        {
            InvitationBtnBG.DOKill(); InvitationBtnBG.DOFade(1, 0.2f);
            InvitationContainer.Show();
        }

    }
    #endregion

    #region ShowDetail
    public void ShowDetail(PlayerPlayfabInformation player)
    {
        if (player == null)
        {
            detailPanel.gameObject.SetActive(false);
            OtherPlayersController.Instance.currentFocus = null;
        }
        else if (player.PlayFabId == OtherPlayersController.Instance.GetIdFocus())
        {
            detailPanel.gameObject.SetActive(false);
            OtherPlayersController.Instance.currentFocus = null;
        }
        else
        {
            detailPanel.gameObject.SetActive(true);
            OtherPlayersController.Instance.currentFocus = player;
        }
        OtherPlayersController.OnPlayerFocusChange?.Invoke();

        if (OtherPlayersController.Instance.currentFocus != null)
        {
            detail.DisplayInfomation(OtherPlayersController.Instance.currentFocus);
            RemovefriendButton.SetActive(false);
            AcceptButton.SetActive(false);
            RefuseButton.SetActive(false);
            CancelRequestButton.SetActive(false);
            AddfriendButton.SetActive(false);
            if (OtherPlayersController.Instance.IsPlayerInFriendList(OtherPlayersController.Instance.currentFocus.PlayFabId))
            {
                RemovefriendButton.SetActive(true);
            }else if (OtherPlayersController.Instance.IsPlayerInInvitationList(OtherPlayersController.Instance.currentFocus.PlayFabId))
            {
                AcceptButton.SetActive(true);
                RefuseButton.SetActive(true);
            }
            else if (OtherPlayersController.Instance.IsPlayerInRequestList(OtherPlayersController.Instance.currentFocus.PlayFabId))
            {
                CancelRequestButton.SetActive(true);
            }
            else
            {
                AddfriendButton.SetActive(true);
            }
        }
    }


    #endregion
}
