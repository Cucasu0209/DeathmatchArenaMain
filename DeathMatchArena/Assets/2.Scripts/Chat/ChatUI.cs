using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Chat.Container.Friend;
using Chat.Container.Channel;
using TMPro;

public class ChatUI : MonoBehaviour
{
    #region Variables
    public static ChatUI Instance;
    public RectTransform ChatPanel;
    public FriendChatListAdapter friendList;

    public AuthenticationComponentUI ChatInput;
    public TextMeshProUGUI ChatContent;
    #endregion

    #region Unity
    private void Awake()
    {
        Instance = this;
    }
    private void OnEnable()
    {
        Invoke(nameof(ShowFriendChatList), 0.5f);
        ShowChat(new ChatPartner());
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
    }

    public void ChangePartnerChatWith(ChatPartner newPartner)
    {
        ChatController.Instance.ChatPartnerForcus = newPartner;
        ShowChat(newPartner);
        ChatController.OnChatPartnerForcusChange?.Invoke();
    }

    public void ShowChat(ChatPartner partner)
    {
        ChatContent.SetText("");
    }
    public void EnterChat()
    {
        string message = ChatInput.GetText();
        ChatController.Instance.EnterChat(message);
    }
    public void OnChatFriendMessageCome(string id, ChatPartnerMessageInfomation msg)
    {
        if (ChatController.Instance.GetIdFocus() == id)
        {
            ChatInput.ClearString();
            ChatContent.SetText(ChatContent.text + "\n" + $"<color=#262626><size=100>{msg.MyName}</size></color> : {msg.message}" +
                $"\n<align=right><color=#000><size=50>\"{msg.time.ToString("HH:mm:ss dd-MM-yyyy")}\"</size></color> </align>");
        }
    }
    #endregion
}
