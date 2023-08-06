using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Chat.Container.Friend;
using Chat.Container.Channel;
using Chat.Container.Invitation;
using TMPro;
using Doozy.Runtime.UIManager.Components;

public class ChatUI : MonoBehaviour
{
    private float countDownTime = 0;
    #region Variables
    public static ChatUI Instance;
    public RectTransform ChatPanel;
    public FriendChatListAdapter friendList;
    public ChannelChatListAdapter channelList;
    public InvitationsChatListAdapter invitationList;

    public TextFieldComponentUI ChatInput;
    public TextMeshProUGUI ChatContent;

    [Header("Chat Information")]
    public TextMeshProUGUI PartnerName;
    public UIButton AddMember;
    public UIButton ManagerMember;
    public TextMeshProUGUI MemberCount;
    public TextMeshProUGUI FriendStateOnline;
    public TextMeshProUGUI FriendStateOffline;
    public TextMeshProUGUI InvitationMessage;
    public UIButton AcceptChatGroup;
    public UIButton RefuseChatGroup;

    public GameObject InputChatPanel;
    public GameObject OutputChatPanel;


    private enum ChatPanelType { Channels, Friends, Invitations, None }
    private ChatPanelType currentPanel = ChatPanelType.None;
    [Header("Chat Panels")]
    public TextMeshProUGUI ChannelsOptionTxt;
    public TextMeshProUGUI FriendsOptionTxt, InvitationsOptionTxt;
    public Image ChannelsOptionBG, FriendsOptionBG, InvitationsOptionBG;
    public GameObject ChannelsPanel, FriendsPanel, InvitationsPanel;

    #endregion

    #region Unity
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Invoke(nameof(UpdateList), 0.5f);
        ChangePanel(ChatPanelType.Channels);
        //ShowChat(new ChatPartner());
        ChatContent.SetText("");
        ChatController.OnFriendChatMessageCome += OnChatFriendMessageCome;
        ChatController.OnChannelChatMessageCome += OnChatFriendMessageCome;
    }
    private void OnDisable()
    {
        ChatController.OnFriendChatMessageCome -= OnChatFriendMessageCome;
        ChatController.OnChannelChatMessageCome -= OnChatFriendMessageCome;

    }
    private void Update()
    {
        countDownTime += Time.deltaTime;
    }
    #endregion

    #region Actions
    public void ToggleChatPanel()
    {
        UpdateList();
        float distanceToZero = Vector3.Distance(ChatPanel.localScale, Vector3.zero);
        float distanceToOne = Vector3.Distance(ChatPanel.localScale, Vector3.one);

        if (distanceToOne > distanceToZero)
        {
            ChatPanel.DOKill();
            ChatPanel.DOScale(1, 0.2f);
            UpdateList();
        }
        else
        {
            ChatPanel.DOKill();
            ChatPanel.DOScale(0, 0.2f);

        }
    }
    public void UpdateList()
    {
        ShowFriendChatList();
        ShowChannelChatList();
        ShowInvitationsChatList();
    }
    public void ShowFriendChatList()
    {
        List<Chat.Container.Friend.MyListItemModel> items = new List<Chat.Container.Friend.MyListItemModel>();
        foreach (var playerId in FriendController.Instance.GetTempFriend())
        {
            items.Add(new Chat.Container.Friend.MyListItemModel() { player = OtherPlayersController.Instance.GetInfFromId(playerId) });
        }
        friendList.SetItems(items);
        ChangePartnerChatWith(ChatController.Instance.ChatPartnerForcus);
    }
    public void ShowChannelChatList()
    {
        List<Chat.Container.Channel.MyListItemModel> items = new List<Chat.Container.Channel.MyListItemModel>();
        foreach (var channel in ChatController.Instance.getTempChannelSub())
        {
            items.Add(new Chat.Container.Channel.MyListItemModel()
            {
                channelId = channel.Key,
                channelName = channel.Value.GroupName
            });
        }
        channelList.SetItems(items);
        ChangePartnerChatWith(ChatController.Instance.ChatPartnerForcus);
    }
    public void ShowInvitationsChatList()
    {
        List<Chat.Container.Invitation.MyListItemModel> items = new List<Chat.Container.Invitation.MyListItemModel>();
        foreach (var item in ChatController.Instance.getTempInvitations())
        {
            items.Add(new Chat.Container.Invitation.MyListItemModel()
            {
                InvitationItem = item.Value
            });
        }
        invitationList.SetItems(items);
        ChangePartnerChatWith(ChatController.Instance.ChatPartnerForcus);
    }
    public void ChangePartnerChatWith(ChatPartner newPartner)
    {
        ShowChat(newPartner);
        if (newPartner == null)
        {
            return;
        }
        ChatController.Instance.ChatPartnerForcus = newPartner;
        ChatController.OnChatPartnerForcusChange?.Invoke();
    }
    public void ShowChat(ChatPartner partner)
    {
        ShowChatInfor(partner);
        if (partner == null)
        {
            return;
        }
        string chatContent = "";
        Debug.Log("trunghehe" + partner.Id + (ChatController.Instance._tempAllFriendChatMessage == null));
        if (partner.Type == ChatPartnerType.Friend)
        {
            if (ChatController.Instance._tempAllFriendChatMessage.ContainsKey(partner.Id) && ChatController.Instance._tempAllFriendChatMessage[partner.Id] != null)
            {
                foreach (var msg in ChatController.Instance._tempAllFriendChatMessage[partner.Id])
                {
                    if (msg.SenderId == PlayerData.GetId())
                    {
                        chatContent += $"\n<align=center><color=#999><size=50>\"{msg.Time.ToString("HH:mm:ss dd-MM-yyyy")}\"</size></color> </align>\n" +
                            $"<align=right><color=#262626><size=60>{msg.SenderName}</size></color> \n {msg.Message}</align>\n";

                    }
                    else
                    {
                        chatContent += $"\n<align=center><color=#999><size=50>\"{msg.Time.ToString("HH:mm:ss dd-MM-yyyy")}\"</size></color> </align>\n" +
                            $"<color=#262626><size=60>{msg.SenderName}</size></color> \n {msg.Message}\n";
                    }

                }
            }
        }
        else if (partner.Type == ChatPartnerType.Channel)
        {
            if (ChatController.Instance._tempAllGroupChatMessage.ContainsKey(partner.Id) && ChatController.Instance._tempAllGroupChatMessage[partner.Id] != null)
            {
                foreach (var msg in ChatController.Instance._tempAllGroupChatMessage[partner.Id])
                {
                    if (msg.SenderId == PlayerData.GetId())
                    {
                        chatContent += $"\n<align=center><color=#999><size=50>\"{msg.Time.ToString("HH:mm:ss dd-MM-yyyy")}\"</size></color> </align>\n" +
                            $"<align=right><color=#262626><size=60>{msg.SenderName}</size></color> \n {msg.Message}</align>\n";

                    }
                    else
                    {
                        chatContent += $"\n<align=center><color=#999><size=50>\"{msg.Time.ToString("HH:mm:ss dd-MM-yyyy")}\"</size></color> </align>\n" +
                            $"<color=#262626><size=60>{msg.SenderName}</size></color> \n {msg.Message}\n";
                    }

                }
            }
        }
        ChatContent.SetText(chatContent);
    }
    public void ShowChatInfor(ChatPartner partner)
    {
        PartnerName.SetText("");
        AddMember.gameObject.SetActive(false);
        ManagerMember.gameObject.SetActive(false);
        MemberCount.gameObject.SetActive(false);
        FriendStateOnline.gameObject.SetActive(false);
        FriendStateOffline.gameObject.SetActive(false);
        InvitationMessage.SetText("");
        AcceptChatGroup.gameObject.SetActive(false);
        RefuseChatGroup.gameObject.SetActive(false);

        InputChatPanel.SetActive(false);
        OutputChatPanel.SetActive(false);


        if (partner == null || string.IsNullOrEmpty(partner.Id))
        {
            return;
        }
        else if (partner.Type == ChatPartnerType.Friend)
        {
            PartnerName.SetText(OtherPlayersController.Instance.GetTempAllPlayers()[partner.Id].DisplayName);
            FriendStateOnline.gameObject.SetActive(true);

            InputChatPanel.SetActive(true);
            OutputChatPanel.SetActive(true);
        }
        else if (partner.Type == ChatPartnerType.Channel)
        {
            if (ChatController.Instance.getTempInvitations().ContainsKey(partner.Id))
            {
                var inv = ChatController.Instance.getTempInvitations()[partner.Id];
                InvitationMessage.SetText($"\"{inv.InvitedByEntityName}\" invited you to join group chat \"{inv.group.GroupName}\"");
                AcceptChatGroup.gameObject.SetActive(true);
                RefuseChatGroup.gameObject.SetActive(true);
            }
            else if (ChatController.Instance.getTempChannelSub().ContainsKey(partner.Id))
            {
                PartnerName.SetText(ChatController.Instance.getTempChannelSub()[partner.Id].GroupName);
                if (ChatController.Instance.isAdmin(PlayerData.GetId(), ChatController.Instance.getTempChannelSub()[partner.Id]))
                    AddMember.gameObject.SetActive(true);
                else AddMember.gameObject.SetActive(false);
                ManagerMember.gameObject.SetActive(true);
                MemberCount.gameObject.SetActive(true);
                MemberCount.SetText(ChatController.Instance.getTempChannelSub()[partner.Id].members.Count + " members");

                InputChatPanel.SetActive(true);
                OutputChatPanel.SetActive(true);
            }
        }
    }
    public void RefreshInf()
    {
        ShowChatInfor(ChatController.Instance.ChatPartnerForcus);
    }
    public void OnChatFriendMessageCome(string id, ChatPartnerMessageInfomation msg)
    {
        if (ChatController.Instance.GetIdFocus() == id)
        {
            ChatInput.ClearString();
            if (msg.SenderId == PlayerData.GetId())
            {
                ChatContent.text += $"\n<align=center><color=#999><size=50>\"{msg.Time.ToString("HH:mm:ss dd-MM-yyyy")}\"</size></color> </align>\n" +
                    $"<align=right><color=#262626><size=60>{msg.SenderName}</size></color> \n {msg.Message}</align>\n";

            }
            else
            {
                ChatContent.text += $"\n<align=center><color=#999><size=50>\"{msg.Time.ToString("HH:mm:ss dd-MM-yyyy")}\"</size></color> </align>\n" +
                    $"<color=#262626><size=60>{msg.SenderName}</size></color> \n {msg.Message}\n";
            }

        }
    }

    #endregion

    #region Button Callbacks
    public void EnterChat()
    {
        string message = ChatInput.GetText();
        if (string.IsNullOrWhiteSpace(message.Trim())) return;
        ChatController.Instance.EnterChat(message);
    }
    public void CreateNewGroupChatClick()
    {
        PopupController.ShowNewGroupChatPopup();
    }
    public void AddMemberClick()
    {
        var ChannelForcus = ChatController.Instance.getChannelForcus();
        if (ChannelForcus == null) return;
        PopupController.ShowFriendListToAddGroupPopup(ChannelForcus);
    }
    public void ManageMemberClick()
    {
        var ChannelForcus = ChatController.Instance.getChannelForcus();
        if (ChannelForcus == null) return;
        PopupController.ShowGroupManagePopup(ChannelForcus);
    }
    public void AcceptInvitationChatClick()
    {
        ChatController.Instance.AcceptGroupInvitation(() =>
        {
            Invoke(nameof(UpdateList), 0.5f);
            ChangePartnerChatWith(null);
        });
    }
    public void RemoveInvitationChatClick()
    {
        ChatController.Instance.RemoveGroupInvitation(() =>
        {
            Invoke(nameof(UpdateList), 0.5f);
            ChangePartnerChatWith(null);
        });
    }
    #endregion

    #region Panels

    public void ChangeToChannels()
    {
        UpdateList();
        if (currentPanel == ChatPanelType.Channels) return;
        ChangePanel(ChatPanelType.Channels);
        if (countDownTime > 0.5f)
        {
            countDownTime = 0;
            ChatController.Instance.GetAllGroupChat(null);
        }

    }
    public void ChangeToFriends()
    {
        UpdateList();
        if (currentPanel == ChatPanelType.Friends) return;
        ChangePanel(ChatPanelType.Friends);

    }
    public void ChangeToInvitations()
    {
        UpdateList();
        if (currentPanel == ChatPanelType.Invitations) return;
        ChangePanel(ChatPanelType.Invitations);
        if (countDownTime > 0.5f)
        {
            countDownTime = 0;
            ChatController.Instance.GetMyMembershipOpportunities(null);
        }
    }
    private void ChangePanel(ChatPanelType panel)
    {
        if (currentPanel == panel) return;
        currentPanel = panel;
        ChannelsOptionTxt.color = Color.white;
        FriendsOptionTxt.color = Color.white;
        InvitationsOptionTxt.color = Color.white;

        ChannelsOptionBG.color = new Color(ChannelsOptionBG.color.r, ChannelsOptionBG.color.g, ChannelsOptionBG.color.b, 0);
        FriendsOptionBG.color = new Color(FriendsOptionBG.color.r, FriendsOptionBG.color.g, FriendsOptionBG.color.b, 0);
        InvitationsOptionBG.color = new Color(InvitationsOptionBG.color.r, InvitationsOptionBG.color.g, InvitationsOptionBG.color.b, 0);

        ChannelsPanel.transform.localScale = Vector3.zero;
        FriendsPanel.transform.localScale = Vector3.zero;
        InvitationsPanel.transform.localScale = Vector3.zero;

        if (panel == ChatPanelType.Channels)
        {
            ChannelsOptionTxt.color = Color.black;
            ChannelsOptionBG.color = new Color(ChannelsOptionBG.color.r, ChannelsOptionBG.color.g, ChannelsOptionBG.color.b, 1);
            ChannelsPanel.transform.localScale = Vector3.one;
        }
        else if (panel == ChatPanelType.Friends)
        {
            FriendsOptionTxt.color = Color.black;
            FriendsOptionBG.color = new Color(FriendsOptionBG.color.r, FriendsOptionBG.color.g, FriendsOptionBG.color.b, 1);
            FriendsPanel.transform.localScale = Vector3.one;
        }
        else if (panel == ChatPanelType.Invitations)
        {
            InvitationsOptionTxt.color = Color.black;
            InvitationsOptionBG.color = new Color(InvitationsOptionBG.color.r, InvitationsOptionBG.color.g, InvitationsOptionBG.color.b, 1);
            InvitationsPanel.transform.localScale = Vector3.one;
        }
    }
    #endregion
}
