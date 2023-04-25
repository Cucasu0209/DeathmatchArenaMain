using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Chat.Container.Friend;
using Chat.Container.Channel;
using TMPro;
using Doozy.Runtime.UIManager.Components;

public class ChatUI : MonoBehaviour
{
    #region Variables
    public static ChatUI Instance;
    public RectTransform ChatPanel;
    public FriendChatListAdapter friendList;
    public ChannelChatListAdapter channelList;

    public AuthenticationComponentUI ChatInput;
    public TextMeshProUGUI ChatContent;

    [Header("Chat Information")]
    public TextMeshProUGUI PartnerName;
    public UIButton AddMember;
    public UIButton ManagerMember;
    public TextMeshProUGUI MemberCount;
    public TextMeshProUGUI FriendStateOnline;
    public TextMeshProUGUI FriendStateOffline;

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
        Invoke(nameof(ShowFriendChatList), 0.5f);
        Invoke(nameof(ShowChannelChatList), 0.5f);
        ChangePanel(ChatPanelType.Channels);
        //ShowChat(new ChatPartner());
        ChatContent.SetText("");
        ChatController.OnFriendChatMessageCome += OnChatFriendMessageCome;
    }
    private void OnDisable()
    {
        ChatController.OnFriendChatMessageCome -= OnChatFriendMessageCome;

    }
    #endregion
    #region Actions
    public void ToggleChatPanel()
    {
        float distanceToZero = Vector3.Distance(ChatPanel.localScale, Vector3.zero);
        float distanceToOne = Vector3.Distance(ChatPanel.localScale, Vector3.one);

        if (distanceToOne > distanceToZero)
        {
            ChatPanel.DOKill();
            ChatPanel.DOScale(1, 0.2f);
            ShowFriendChatList();
        }
        else
        {
            ChatPanel.DOKill();
            ChatPanel.DOScale(0, 0.2f);
            ShowChannelChatList();
        }
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

    public void ChangePartnerChatWith(ChatPartner newPartner)
    {
        if (newPartner == null)
        {
            ChatContent.SetText("");
            return;
        }
        ChatController.Instance.ChatPartnerForcus = newPartner;
        ShowChat(newPartner);
        ChatController.OnChatPartnerForcusChange?.Invoke();
    }

    public void ShowChat(ChatPartner partner)
    {
        string chatContent = "";
        if (partner.Type == ChatPartnerType.Friend)
        {
            if (ChatController.Instance._tempAllFriendChatMessage.ContainsKey(partner.Id))
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

        }
        ShowChatInfor(partner);
        ChatContent.SetText(chatContent);
    }
    public void ShowChatInfor(ChatPartner partner)
    {
        AddMember.gameObject.SetActive(false);
        ManagerMember.gameObject.SetActive(false);
        MemberCount.gameObject.SetActive(false);
        FriendStateOnline.gameObject.SetActive(false);
        FriendStateOffline.gameObject.SetActive(false);

        if (string.IsNullOrEmpty(partner.Id))
        {
            PartnerName.SetText("");
        }
        else if (partner.Type == ChatPartnerType.Friend)
        {
            PartnerName.SetText(OtherPlayersController.Instance.GetTempAllPlayers()[partner.Id].DisplayName);
            FriendStateOnline.gameObject.SetActive(true);
        }
        else if (partner.Type == ChatPartnerType.Channel)
        {
            PartnerName.SetText(ChatController.Instance.getTempChannelSub()[partner.Id].GroupName);
            AddMember.gameObject.SetActive(true);
            ManagerMember.gameObject.SetActive(true);
            MemberCount.gameObject.SetActive(true);
            MemberCount.SetText(ChatController.Instance.getTempChannelSub()[partner.Id].members.Count + " members");
        }
    }
    public void EnterChat()
    {
        string message = ChatInput.GetText();
        if (string.IsNullOrWhiteSpace(message.Trim())) return;
        ChatController.Instance.EnterChat(message);
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

    public void CreateNewGroupChatClick()
    {
        PopupController.ShowNewGroupChatPopup();
    }
    #endregion

    #region Panels
    public void ChangeToChannels()
    {
        ChangePanel(ChatPanelType.Channels);
    }
    public void ChangeToFriends()
    {
        ChangePanel(ChatPanelType.Friends);
    }
    public void ChangeToInvitations()
    {
        ChangePanel(ChatPanelType.Invitations);
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
