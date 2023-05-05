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
using TMPro;

public class FriendUI : MonoBehaviour
{
    public static FriendUI Instance;


    #region UIComponent
    public Image FriendBtnBG, RequestBtnBG, FindBtnBG, InvitationBtnBG;
    public UIContainer FriendContainer, RequestContainer, FindContainer, InvitationContainer;
    public RectTransform detailPanel;
    public TextFieldComponentUI SearchField;

    public GameObject AcceptButton;
    public GameObject RefuseButton;
    public GameObject AddfriendButton;
    public GameObject RemovefriendButton;
    public GameObject CancelRequestButton;

    public TextMeshProUGUI RequestCount;
    public TextMeshProUGUI InvitationCount;


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
        OtherPlayersController.OnTempListChange += OnChatMessageCome;
    }
    private void OnDisable()
    {
        OtherPlayersController.OnTempListChange -= OnChatMessageCome;
    }
    private void Update()
    {
        if (OtherPlayersController.Instance.GetTempRequest().Count > 0)
        {
            RequestCount.transform.parent.gameObject.SetActive(true);
            RequestCount.SetText(OtherPlayersController.Instance.GetTempRequest().Count.ToString());
        }
        else RequestCount.transform.parent.gameObject.SetActive(false);

        if (OtherPlayersController.Instance.GetTempInvitaion().Count > 0)
        {
            InvitationCount.transform.parent.gameObject.SetActive(true);
            InvitationCount.SetText(OtherPlayersController.Instance.GetTempInvitaion().Count.ToString());
        }
        else InvitationCount.transform.parent.gameObject.SetActive(false);
    }
    private void OnChatMessageCome(ChatMessage_Photon message)
    {
        string cache = OtherPlayersController.Instance.GetIdFocus();
        ShowDetail(null);
        if (message.type == ChatMessageType_Photon.RequestFriend)
        {
            if (currentContainer == FriendContainerUIType.Invitation) ShowInvitationContainer();
        }
        else if (message.type == ChatMessageType_Photon.CancelRequestFriend)
        {
            if (currentContainer == FriendContainerUIType.Invitation) ShowInvitationContainer();
        }
        else if (message.type == ChatMessageType_Photon.AcceptRequestFriend)
        {
            if (currentContainer == FriendContainerUIType.Friend) ShowFriendContainer();
            if (currentContainer == FriendContainerUIType.Request) ShowRequestContainer();
        }
        else if (message.type == ChatMessageType_Photon.RefuserequestFriend)
        {
            if (currentContainer == FriendContainerUIType.Request) ShowRequestContainer();
        }
        else if (message.type == ChatMessageType_Photon.RemoveFriend)
        {
            if (currentContainer == FriendContainerUIType.Friend) ShowFriendContainer();
        }
        ShowDetail(OtherPlayersController.Instance.GetInfFromId(cache));
    }
    #endregion

    #region Actions
    public void ShowFriendContainer()
    {
        ShowContainer(FriendContainerUIType.Friend);
        List<Friend.Container.Friend.MyListItemModel> items = new List<Friend.Container.Friend.MyListItemModel>();
        foreach (var playerId in FriendController.Instance.GetTempFriend())
        {
            items.Add(new Friend.Container.Friend.MyListItemModel() { player = OtherPlayersController.Instance.GetInfFromId(playerId) });
        }
        friendList.SetItems(items);
        ShowDetail(null);
    }
    public void ShowRequestContainer()
    {
        ShowContainer(FriendContainerUIType.Request);
        List<Friend.Container.Request.MyListItemModel> items = new List<Friend.Container.Request.MyListItemModel>();
        foreach (var playerId in OtherPlayersController.Instance.GetTempRequest())
        {
            items.Add(new Friend.Container.Request.MyListItemModel() { player = OtherPlayersController.Instance.GetInfFromId(playerId) });
        }
        requestList.SetItems(items);
        ShowDetail(null);
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
        ShowDetail(null);
    }
    public void ShowInvitationContainer()
    {
        ShowContainer(FriendContainerUIType.Invitation);
        List<Friend.Container.Invitation.MyListItemModel> items = new List<Friend.Container.Invitation.MyListItemModel>();
        foreach (var playerId in OtherPlayersController.Instance.GetTempInvitaion())
        {
            items.Add(new Friend.Container.Invitation.MyListItemModel() { player = OtherPlayersController.Instance.GetInfFromId(playerId) });
        }
        invitationList.SetItems(items);
        ShowDetail(null);
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
    public void SearchPlayer()
    {
        string _p = SearchField.GetText().ToLower();
        findList.ResetItems(0);
        List<Friend.Container.FindPlayer.MyListItemModel> items = new List<Friend.Container.FindPlayer.MyListItemModel>();
        foreach (var player in OtherPlayersController.Instance.GetTempAllPlayers().Values)
        {
            if (player.DisplayName.ToLower().Contains(_p) || player.PlayFabId.Contains(_p))
            {
                items.Add(new Friend.Container.FindPlayer.MyListItemModel() { player = player });
            }

        }
        findList.SetItems(items);
    }
    #endregion

    #region ShowDetail
    public void ShowDetail(PlayerPlayfabInformation player)
    {
        if (player == null)
        {
            detailPanel.DOKill();
            detailPanel.DOAnchorPosX(-300, 0.2f);
            //detailPanel.gameObject.SetActive(false);
            OtherPlayersController.Instance.currentFocus = null;
        }
        else if (player.PlayFabId == OtherPlayersController.Instance.GetIdFocus())
        {
            detailPanel.DOKill();
            detailPanel.DOAnchorPosX(-300, 0.2f);
            //detailPanel.gameObject.SetActive(false);
            OtherPlayersController.Instance.currentFocus = null;
        }
        else
        {
            OtherPlayersController.Instance.currentFocus = player;
            if (detailPanel.anchoredPosition.x > 0)
            {
                detailPanel.DOKill();
                detailPanel.DOAnchorPosX(-300, 0.2f).OnComplete(() =>
                {
                    DisplayInforDetail();
                    detailPanel.DOAnchorPosX(341, 0.2f).SetDelay(0.1f).SetEase(Ease.InOutSine);
                });
            }
            else
            {
                detailPanel.DOKill();
                DisplayInforDetail();
                detailPanel.DOAnchorPosX(341, 0.2f).SetDelay(0.1f).SetEase(Ease.InOutSine);
            }
            //detailPanel.gameObject.SetActive(true);

        }
        OtherPlayersController.OnPlayerFocusChange?.Invoke();


    }
    private void DisplayInforDetail()
    {
        if (OtherPlayersController.Instance.currentFocus != null)
        {
            detail.DisplayInfomation(OtherPlayersController.Instance.currentFocus);
            RemovefriendButton.SetActive(false);
            AcceptButton.SetActive(false);
            RefuseButton.SetActive(false);
            CancelRequestButton.SetActive(false);
            AddfriendButton.SetActive(false);
            if (OtherPlayersController.Instance.IsMe(OtherPlayersController.Instance.currentFocus.PlayFabId))
            {

            }
            else if (OtherPlayersController.Instance.IsPlayerInFriendList(OtherPlayersController.Instance.currentFocus.PlayFabId))
            {
                RemovefriendButton.SetActive(true);
            }
            else if (OtherPlayersController.Instance.IsPlayerInInvitationList(OtherPlayersController.Instance.currentFocus.PlayFabId))
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
    public void AddfriendButtonClick()
    {
        OtherPlayersController.Instance.AddFriend();
        DisplayInforDetail();
    }
    public void CancelRequestButtonClick()
    {
        OtherPlayersController.Instance.CancelRequestFriend();
        DisplayInforDetail();
        if (currentContainer == FriendContainerUIType.Request) ShowRequestContainer();
    }
    public void AcceptInvitationButtonClick()
    {
        OtherPlayersController.Instance.AcceptInvitationFriend();
        DisplayInforDetail();
        if (currentContainer == FriendContainerUIType.Invitation) ShowInvitationContainer();
    }
    public void RefuseInvitationButtonClick()
    {
        OtherPlayersController.Instance.RefuseInvitationFriend();
        DisplayInforDetail();
        if (currentContainer == FriendContainerUIType.Invitation) ShowInvitationContainer();
    }
    public void RemoveFriendButtonClick()
    {
        string notifyMessage = $"Are you sure to Remove friend \"{OtherPlayersController.Instance.GetInfFromId().DisplayName}\"?";
        PopupController.ShowYesNoPopup(notifyMessage, () =>
        {
            OtherPlayersController.Instance.RemoveFriend();
            DisplayInforDetail();
            if (currentContainer == FriendContainerUIType.Friend) ShowFriendContainer();
        }, null); 
    }

    #endregion
}
