using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class NetworkController_Chat : MonoBehaviour, IChatClientListener
{
    #region Singleton
    protected NetworkController_Chat() { }

    private static NetworkController_Chat f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static NetworkController_Chat Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<NetworkController_Chat>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static NetworkController_Chat AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<NetworkController_Chat>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
    public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation = false) where T : MonoBehaviour
    {
        var component = FindObjectOfType<T>();
        if (component != null && isSingleton)
        {
            Debug.Log("Cannot add another " + typeof(T).Name + " to this Scene because you don't need more than one.");
#if UNITY_EDITOR
            UnityEditor.Selection.activeObject = component;
#endif
            return component;
        }

        component = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(component.gameObject, "Created " + gameObjectName);
        if (selectGameObjectAfterCreation) UnityEditor.Selection.activeObject = component.gameObject;
#endif
        return component;
    }
    private void Awake()
    {
        if (f_instance != null && f_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        f_instance = this;
        DontDestroyOnLoad(gameObject);


    }
    public void InitInstance() { }
    #endregion

    #region Variables
    public ChatClient chatClient;
    private Action OnConnecctCompleted;
    public static event Action<ChatMessage> OnChatMessageCome;
    #endregion

    #region Unity
    private void Update()
    {
        if (chatClient != null) chatClient.Service();
    }
    #endregion

    #region Chat Callbacks
    void IChatClientListener.DebugReturn(DebugLevel level, string message)
    {
        Debug.Log($"[{this.name}]: Problem - {message}.");
    }
    void IChatClientListener.OnDisconnected()
    {
        throw new System.NotImplementedException();
    }
    void IChatClientListener.OnConnected()
    {
        Debug.Log($"[{this.name}]: Connect Succced.");
        OnConnecctCompleted?.Invoke();
    }
    void IChatClientListener.OnChatStateChange(ChatState state)
    {
        Debug.Log($"[{this.name}]: State -  me: {state.ToString()}.");
    }
    void IChatClientListener.OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log($"[{this.name}]: PublicMessage - {channelName}");
    }
    void IChatClientListener.OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log($"[{this.name}]: PrivateMessage - {sender}: {message}");
        if (sender == PlayerData.GetId()) return;
        ChatMessage _m = JsonConvert.DeserializeObject<ChatMessage>(message.ToString());
        if (_m != null && _m.senderId == sender)
        {
            if (_m.type == ChatMessageType.RequestFriend)
            {
                Debug.Log($"[{this.name}]: RequestFriend message from {_m.senderDisplayName}-{_m.senderId}");
                OnChatMessageCome?.Invoke(_m);
            }
            else if (_m.type == ChatMessageType.CancelRequestFriend)
            {
                Debug.Log($"[{this.name}]: Cancel RequestFriend message from {_m.senderDisplayName}-{_m.senderId}");
                OnChatMessageCome?.Invoke(_m);
            }
            else if (_m.type == ChatMessageType.AcceptRequestFriend)
            {
                Debug.Log($"[{this.name}]: Aceept Friend message from {_m.senderDisplayName}-{_m.senderId}");
                OnChatMessageCome?.Invoke(_m);
            }
            else if (_m.type == ChatMessageType.RefuserequestFriend)
            {
                Debug.Log($"[{this.name}]: Refuse Friend message from {_m.senderDisplayName}-{_m.senderId}");
                OnChatMessageCome?.Invoke(_m);
            }
            else if (_m.type == ChatMessageType.RemoveFriend)
            {
                Debug.Log($"[{this.name}]:  Remove Friend message from {_m.senderDisplayName}-{_m.senderId}");
                OnChatMessageCome?.Invoke(_m);
            }
        }
    }
    void IChatClientListener.OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"[{this.name}]: Subscribed new Channel");
    }
    void IChatClientListener.OnUnsubscribed(string[] channels)
    {
        Debug.Log($"[{this.name}]: Unsubscribed new Channel");
    }
    void IChatClientListener.OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"[{this.name}]: Status update -  {user}: {status}.");
    }
    void IChatClientListener.OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"[{this.name}]: Subscribed {user}: {channel}");
    }
    void IChatClientListener.OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"[{this.name}]: UnSubscribed {user}: {channel}");
    }
    #endregion

    #region Public Action
    public void Connect(Action Oncompleted)
    {
        if (AuthenticationController.Instance.IsLogin() == false)
        {
            Debug.Log($"[{this.name}]: Playfab account didn't login yet.");
            return;
        }
        Debug.Log($"[{this.name}]: Playfab Connecting....");
        this.chatClient = new ChatClient(this);
        //this.chatClient.AuthValues = new AuthenticationValues(PlayerData.GetId());
        this.chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
            PhotonNetwork.AppVersion, new AuthenticationValues(PlayerData.GetId()));
        //this.chatClient.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings());
        OnConnecctCompleted = Oncompleted;
    }

    public void SendRequestFriendMessage(string playfabId)
    {
        ChatMessage _mess = new ChatMessage()
        {
            type = ChatMessageType.RequestFriend,
            senderId = PlayerData.GetId(),
            senderDisplayName = PlayerData.GetNickName(),
            message = ""
        };
        this.chatClient.SendPrivateMessage(playfabId, JsonConvert.SerializeObject(_mess));
    }
    public void SendCancelRequestFriendMessage(string playfabId)
    {
        ChatMessage _mess = new ChatMessage()
        {
            type = ChatMessageType.CancelRequestFriend,
            senderId = PlayerData.GetId(),
            senderDisplayName = PlayerData.GetNickName(),
            message = ""
        };
        this.chatClient.SendPrivateMessage(playfabId, JsonConvert.SerializeObject(_mess));
    }
    public void SendAcceptInvitaionFriendMessage(string playfabId)
    {
        ChatMessage _mess = new ChatMessage()
        {
            type = ChatMessageType.AcceptRequestFriend,
            senderId = PlayerData.GetId(),
            senderDisplayName = PlayerData.GetNickName(),
            message = ""
        };
        this.chatClient.SendPrivateMessage(playfabId, JsonConvert.SerializeObject(_mess));
    }
    public void SendRefuseInvitationFriendMessage(string playfabId)
    {
        ChatMessage _mess = new ChatMessage()
        {
            type = ChatMessageType.RefuserequestFriend,
            senderId = PlayerData.GetId(),
            senderDisplayName = PlayerData.GetNickName(),
            message = ""
        };
        this.chatClient.SendPrivateMessage(playfabId, JsonConvert.SerializeObject(_mess));
    }
    public void SendRemoveFriendMessage(string playfabId)
    {
        ChatMessage _mess = new ChatMessage()
        {
            type = ChatMessageType.RemoveFriend,
            senderId = PlayerData.GetId(),
            senderDisplayName = PlayerData.GetNickName(),
            message = ""
        };
        this.chatClient.SendPrivateMessage(playfabId, JsonConvert.SerializeObject(_mess));
    }
    #endregion

}

public enum ChatMessageType
{
    RequestFriend,
    CancelRequestFriend,
    AcceptRequestFriend,
    RefuserequestFriend,
    RemoveFriend
}

public class ChatMessage
{
    public ChatMessageType type;
    public string senderId;
    public string senderDisplayName;
    public string message;
}
