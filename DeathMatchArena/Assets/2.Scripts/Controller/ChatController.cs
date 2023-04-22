using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using Newtonsoft.Json;
public class ChatController : MonoBehaviour
{
    #region Singleton
    protected ChatController() { }

    private static ChatController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static ChatController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<ChatController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static ChatController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<ChatController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    public List<string> SubscribedChannel;

    public ChatPartner ChatPartnerForcus;
    public static Action OnChatPartnerForcusChange;
    public static Action<string, ChatPartnerMessageInfomation> OnFriendChatMessageCome;
    #endregion

    #region Unity
    private void OnEnable()
    {
        NetworkController_Chat.OnChatMessageCome += ChatMessageCome;
    }
    private void OnDisable()
    {
        NetworkController_Chat.OnChatMessageCome -= ChatMessageCome;
    }
    #endregion

    #region Actions
    public void ChatWith(string name, ChatPartnerType type)
    {

    }
    public string GetIdFocus()
    {
        return ChatPartnerForcus.Id;
    }
    public void EnterChat(string message, ChatPartner partner = null)
    {
        if (partner == null) partner = ChatPartnerForcus;
        if (partner == null) return;
        if (partner.Type == ChatPartnerType.Friend)
        {
            string NewMmgObj = JsonConvert.SerializeObject(new ChatPartnerMessageInfomation()
            {
                MyId = PlayerData.GetId(),
                PartnerId = partner.Id,
                MyName = PlayerData.GetNickName(),
                message = message,
                time = DateTime.Now,
            });
            NetworkController_Chat.Instance.SendChatFriendMessage(partner.Id, NewMmgObj);
        }
    }
    public void ChatMessageCome(ChatMessage_Photon message)
    {
        if (message.type == ChatMessageType_Photon.ChatFriend)
        {
            ChatPartnerMessageInfomation newMes = JsonConvert.DeserializeObject<ChatPartnerMessageInfomation>(
                message.message);
            string partnerId = PlayerData.GetId() == message.senderId ? newMes.PartnerId : newMes.MyId;

            SaveChat(new ChatPartner()
            {
                Id = partnerId,
                Type = ChatPartnerType.Friend
            }, newMes);
            OnFriendChatMessageCome?.Invoke(partnerId,newMes);
        }
    }
    public void SaveChat(ChatPartner partner, ChatPartnerMessageInfomation msg)
    {

    }
    #endregion
}
public enum ChatPartnerType
{
    Friend,
    Channel
}
public class ChatPartner
{
    public string Id;
    public ChatPartnerType Type;
}
public class ChatPartnerMessageInfomation
{
    public string MyId;
    public string PartnerId;
    public string MyName;
    public string message;
    public DateTime time;
}
