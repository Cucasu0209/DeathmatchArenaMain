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
    public Dictionary<string, List<ChatPartnerMessageInfomation>> _tempAllFriendChatMessage = new Dictionary<string, List<ChatPartnerMessageInfomation>>();
    public Dictionary<string, GroupChatInfomation> _tempSubscribedGroupChat = new Dictionary<string, GroupChatInfomation>();
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
    public Dictionary<string, GroupChatInfomation> getTempChannelSub()
    {
        return _tempSubscribedGroupChat;
    }
    public void ChatWith(string name, ChatPartnerType type)
    {

    }
    public string GetIdFocus()
    {
        return ChatPartnerForcus?.Id;
    }
    public void EnterChat(string message, ChatPartner partner = null)
    {
        if (partner == null) partner = ChatPartnerForcus;
        if (partner == null) return;
        if (partner.Type == ChatPartnerType.Friend)
        {
            string NewMmgObj = JsonConvert.SerializeObject(new ChatPartnerMessageInfomation()
            {
                SenderId = PlayerData.GetId(),
                ReceiverId = partner.Id,
                SenderName = PlayerData.GetNickName(),
                Message = message,
                Time = DateTime.Now,
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
            string partnerId = PlayerData.GetId() == message.senderId ? newMes.ReceiverId : newMes.SenderId;

            SaveChat(new ChatPartner()
            {
                Id = partnerId,
                Type = ChatPartnerType.Friend
            }, newMes);
            OnFriendChatMessageCome?.Invoke(partnerId, newMes);
        }
    }
    public void SaveChat(ChatPartner partner, ChatPartnerMessageInfomation msg)
    {
        if (partner.Type == ChatPartnerType.Friend)
        {
            if (_tempAllFriendChatMessage.ContainsKey(partner.Id) == false)
            {
                _tempAllFriendChatMessage.Add(partner.Id, new List<ChatPartnerMessageInfomation>());

            }
            _tempAllFriendChatMessage[partner.Id].Add(msg);
            Debug.Log("set friend" + _tempAllFriendChatMessage[partner.Id].Count);
            PlayfabController.Instance.SetAllFriendChatMessage(_tempAllFriendChatMessage);
        }

    }
    public void GetAllFriendChatMessage(Action Oncomplete)
    {
        PlayfabController.Instance.GetAllFriendChatMessage((chatmsg) =>
        {
            _tempAllFriendChatMessage = chatmsg;
            Oncomplete?.Invoke();
        });
    }
    public void GetAllGroupChat(Action Oncomplete)
    {
        PlayfabController.Instance.GetSubscribedGroupChat((groupList) =>
        {  
            int GroupLoaded = 0;
            if (groupList.Count == 0)
            {
                Oncomplete?.Invoke();
                return;
            }
            foreach (var group in groupList)
            {
                _tempSubscribedGroupChat.Add(group.Key, group.Value);
                PlayfabController.Instance.GetGroupChatMembers(group.Value, (members) =>
                {
                    GroupLoaded++;
                    _tempSubscribedGroupChat[group.Key].members = members;
                    if (GroupLoaded == groupList.Count)
                        Oncomplete?.Invoke();
                });
            }
        });
    }
    public void CreateNewGroupChat(string name, Action<GroupCreateResultEnum, GroupChatInfomation> Oncomplete)
    {
        if (string.IsNullOrEmpty(name.Trim()))
        {
            Oncomplete?.Invoke(GroupCreateResultEnum.Fail, new GroupChatInfomation());
            return;
        }
        PlayfabController.Instance.CreateNewGroup(name, (group) =>
        {
            if (group.GroupEntity == null)
            {
                Oncomplete?.Invoke(GroupCreateResultEnum.Fail, new GroupChatInfomation());
            }
            else
            {
                if (_tempSubscribedGroupChat.ContainsKey(group.GroupEntity.Id) == false)
                {
                    _tempSubscribedGroupChat.Add(group.GroupEntity.Id, group);
                }
                else
                {
                    _tempSubscribedGroupChat[group.GroupEntity.Id] = group;
                }
                Oncomplete?.Invoke(GroupCreateResultEnum.Success, group);
            }
        });
    }
    public void AddMembersToGroupChat(GroupChatInfomation group, string PlayFabId)
    {
        PlayfabController.Instance.InviteMemberToGroupChat(group, PlayFabId, (NewGroup) =>
        {
            if (_tempSubscribedGroupChat.ContainsKey(NewGroup.GroupEntity.Id) == false)
            {
                _tempSubscribedGroupChat.Add(NewGroup.GroupEntity.Id, group);
            }
            else
            {
                _tempSubscribedGroupChat[NewGroup.GroupEntity.Id] = group;
            }
        });
    }
    #endregion
}
public enum GroupCreateResultEnum
{
    Success,
    Fail
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

[Serializable]
public class ChatPartnerMessageInfomation
{
    public string SenderId;
    public string ReceiverId;
    public string SenderName;
    public string Message;
    public DateTime Time;
}

[Serializable]
public class GroupChatInfomation
{
    public PlayFab.GroupsModels.EntityKey GroupEntity;
    public string GroupName;
    public List<GroupChatPlayerRole> members;
    public List<GroupOpportunityForm> GroupInvitations;
}

[Serializable]
public class GroupChatPlayerRole
{
    public string playfabID;
    public string playerRoleId;
    public string playerRoleName;
}

[Serializable]
public class GroupOpportunityForm
{
    public PlayFab.GroupsModels.EntityKey TittleInvitedEntity;
    public PlayFab.GroupsModels.EntityKey TittleInvitedByEntity;
    public PlayFab.GroupsModels.EntityKey MasterInvitedEntity;
    public PlayFab.GroupsModels.EntityKey MasterInvitedByEntity;
    public GroupChatInfomation group;
}