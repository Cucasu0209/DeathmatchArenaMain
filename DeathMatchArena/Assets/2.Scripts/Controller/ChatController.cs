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
    public static Action<string, ChatPartnerMessageInfomation> OnChannelChatMessageCome;
    public Dictionary<string, List<ChatPartnerMessageInfomation>> _tempAllFriendChatMessage = new Dictionary<string, List<ChatPartnerMessageInfomation>>();
    public Dictionary<string, List<ChatPartnerMessageInfomation>> _tempAllGroupChatMessage = new Dictionary<string, List<ChatPartnerMessageInfomation>>();
    public Dictionary<string, GroupChatInfomation> _tempSubscribedGroupChat = new Dictionary<string, GroupChatInfomation>();
    public Dictionary<string, GroupOpportunityForm> _tempMyGroupInvitation = new Dictionary<string, GroupOpportunityForm>();
    #endregion

    #region Unity
    private void OnEnable()
    {
        NetworkController_Chat.OnPrivateChatMessageCome += ChatPrivateMessageCome;
        NetworkController_Chat.OnPublicChatMessageCome += ChatPublicMessageCome;
    }
    private void OnDisable()
    {
        NetworkController_Chat.OnPrivateChatMessageCome -= ChatPrivateMessageCome;
        NetworkController_Chat.OnPublicChatMessageCome -= ChatPublicMessageCome;
    }
    #endregion

    #region Actions
    public Dictionary<string, GroupChatInfomation> getTempChannelSub()
    {
        return _tempSubscribedGroupChat;
    }
    public Dictionary<string, GroupOpportunityForm> getTempInvitations()
    {
        return _tempMyGroupInvitation;
    }
    public void ChatWith(string name, ChatPartnerType type)
    {

    }
    public string GetIdFocus()
    {
        return ChatPartnerForcus?.Id;
    }
    public GroupChatInfomation getChannelForcus()
    {
        if (ChatPartnerForcus.Type == ChatPartnerType.Channel)
        {
            if (_tempSubscribedGroupChat.ContainsKey(ChatPartnerForcus.Id))
            {
                return _tempSubscribedGroupChat[ChatPartnerForcus.Id];
            }
            return null;
        }
        return null;
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
        if (partner.Type == ChatPartnerType.Channel)
        {
            string NewMmgObj = JsonConvert.SerializeObject(new ChatPartnerMessageInfomation()
            {
                SenderId = PlayerData.GetId(),
                ReceiverId = partner.Id,
                SenderName = PlayerData.GetNickName(),
                Message = message,
                Time = DateTime.Now,
            });
            NetworkController_Chat.Instance.SendChatGroupMessage(partner.Id, NewMmgObj);
        }
    }
    public void ChatPrivateMessageCome(ChatMessage_Photon message)
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
    public void ChatPublicMessageCome(string channel, ChatMessage_Photon message)
    {
        if (message.type == ChatMessageType_Photon.ChatChannel)
        {
            ChatPartnerMessageInfomation newMes = JsonConvert.DeserializeObject<ChatPartnerMessageInfomation>(
                message.message);

            SaveChat(new ChatPartner()
            {
                Id = channel,
                Type = ChatPartnerType.Channel
            }, newMes);
            OnChannelChatMessageCome?.Invoke(channel, newMes);
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
        else if (partner.Type == ChatPartnerType.Channel)
        {
            if (_tempAllGroupChatMessage.ContainsKey(partner.Id) == false)
            {
                _tempAllGroupChatMessage.Add(partner.Id, new List<ChatPartnerMessageInfomation>());
            }
            if (_tempAllGroupChatMessage[partner.Id] == null)
            {
                _tempAllGroupChatMessage[partner.Id] = new List<ChatPartnerMessageInfomation>();
            }

            _tempAllGroupChatMessage[partner.Id].Add(msg);


            Debug.Log("set friend" + _tempAllGroupChatMessage[partner.Id].Count);
            if (_tempSubscribedGroupChat.ContainsKey(partner.Id))
            {
                PlayfabController.Instance.SetAllGroupChatMessage(_tempSubscribedGroupChat[partner.Id], _tempAllGroupChatMessage[partner.Id]);

            }
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
            Dictionary<string, GroupChatInfomation> _temp = new Dictionary<string, GroupChatInfomation>();
            if (groupList.Count == 0)
            {
                Oncomplete?.Invoke();
            }
            foreach (var group in groupList)
            {
                NetworkController_Chat.Instance.SubscribeChat(group.Key);
                _temp.Add(group.Key, group.Value);
                PlayfabController.Instance.GetGroupChatMembers(group.Value, (members) =>
                {
                    _temp[group.Key].members = members;
                    if (isAdmin(PlayerData.GetId(), group.Value))
                    {
                        PlayfabController.Instance.GetGroupRequests(group.Value, (requests) =>
                        {

                            _temp[group.Key].GroupInvitations = requests;
                            GroupLoaded++;
                            if (GroupLoaded == groupList.Count * 2)
                            {
                                _tempSubscribedGroupChat = _temp;
                                Oncomplete?.Invoke();
                            }
                        });
                        return;
                    }

                    GroupLoaded++;
                    if (GroupLoaded == groupList.Count * 2)
                    {
                        _tempSubscribedGroupChat = _temp;
                        Oncomplete?.Invoke();
                    }

                });

                PlayfabController.Instance.GetGroupChatMessage(group.Value, (ListMessage) =>
                {
                    if (_tempAllGroupChatMessage.ContainsKey(group.Key))
                    {
                        _tempAllGroupChatMessage[group.Key] = ListMessage;
                    }
                    else _tempAllGroupChatMessage.Add(group.Key, ListMessage);
                    GroupLoaded++;
                    if (GroupLoaded == groupList.Count * 2)
                    {
                        _tempSubscribedGroupChat = _temp;
                        Oncomplete?.Invoke();
                    }
                });
            }
        });
    }
    public void GetMyMembershipOpportunities(Action OnComplete)
    {
        PlayfabController.Instance.GetMembershipOpportunities((listInvitations) =>
        {
            _tempMyGroupInvitation = new Dictionary<string, GroupOpportunityForm>();
            foreach (var invitation in listInvitations)
            {
                if (_tempMyGroupInvitation.ContainsKey(invitation.group.GroupEntity.Id) == false)
                {
                    _tempMyGroupInvitation.Add(invitation.group.GroupEntity.Id, invitation);
                }
                else
                {
                    _tempMyGroupInvitation[invitation.group.GroupEntity.Id] = invitation;
                }
            }
            OnComplete?.Invoke();
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
    public void InviteMembersToGroupChat(GroupChatInfomation group, string PlayFabId)
    {
        PlayfabController.Instance.InviteMemberToGroupChat(group, PlayFabId, () =>
        {
            PopupController.ShowLoadingPopup();
            GetAllGroupChat(() =>
            {
                PopupController.HideLoadingPopup();
            });
        });
    }
    public void AcceptGroupInvitation(Action OnComplete, ChatPartner group = null)
    {
        if (group == null) group = ChatPartnerForcus;
        if (ChatPartnerForcus == null) return;
        if (_tempMyGroupInvitation.ContainsKey(group.Id))
        {
            PlayfabController.Instance.AcceptMembershipOpportunities(_tempMyGroupInvitation[group.Id].group, (issuccess) =>
            {
                PopupController.ShowLoadingPopup();
                GetAllGroupChat(() =>
                {
                    GetMyMembershipOpportunities(() =>
                    {
                        PopupController.HideLoadingPopup();
                        OnComplete?.Invoke();
                    });
                });
            });

        }
    }
    public void RemoveGroupInvitation(Action OnComplete, ChatPartner group = null)
    {
        if (group == null) group = ChatPartnerForcus;
        if (ChatPartnerForcus == null) return;
        if (_tempMyGroupInvitation.ContainsKey(group.Id))
        {
            PlayfabController.Instance.RefuseMembershipOpportunities(_tempMyGroupInvitation[group.Id].group, () =>
            {
                PopupController.ShowLoadingPopup();
                GetMyMembershipOpportunities(() =>
                {
                    PopupController.HideLoadingPopup();
                    OnComplete?.Invoke();
                });
            });

        }
    }
    public void RemoveMember(string titleID, Action<GroupChatInfomation> OnComplete, ChatPartner group = null)
    {
        if (group == null) group = ChatPartnerForcus;
        if (ChatPartnerForcus == null) return;
        if (_tempSubscribedGroupChat.ContainsKey(group.Id))
        {
            if (isAdmin(PlayerData.GetId(), _tempSubscribedGroupChat[group.Id]))
            {
                PlayFab.GroupsModels.EntityKey _player_ = new PlayFab.GroupsModels.EntityKey();
                foreach (var member in _tempSubscribedGroupChat[group.Id].members)
                {
                    if (titleID == member.titleId)
                    {
                        _player_ = new PlayFab.GroupsModels.EntityKey()
                        {
                            Id = titleID,
                            Type = "title_player_account"
                        };
                    }
                }
                if (string.IsNullOrEmpty(_player_.Id) == false)
                {
                    PlayfabController.Instance.RemoveMember(_player_, _tempSubscribedGroupChat[group.Id], () =>
                    {
                        PopupController.ShowLoadingPopup();
                        GetAllGroupChat(() =>
                        {
                            PopupController.HideLoadingPopup();

                            OnComplete?.Invoke(_tempSubscribedGroupChat[group.Id]);
                        });
                    });
                }

            }
        }
    }
    public void DeleteGroup(Action OnComplete, ChatPartner group = null)
    {
        if (group == null) group = ChatPartnerForcus;
        if (ChatPartnerForcus == null) return;
        if (_tempSubscribedGroupChat.ContainsKey(group.Id))
        {
            if (isAdmin(PlayerData.GetId(), _tempSubscribedGroupChat[group.Id]))
            {
                PlayfabController.Instance.DeleteGroup(_tempSubscribedGroupChat[group.Id], () =>
                {
                    PopupController.ShowLoadingPopup();
                    GetAllGroupChat(() =>
                    {
                        PopupController.HideLoadingPopup();
                        OnComplete?.Invoke();
                    });
                });
            }
        }
    }
    public bool isAdmin(string id, GroupChatInfomation group)
    {
        foreach (var member in group.members)
            if ((member.PlayfabId == id || member.titleId == id) && member.playerRoleId == PlayfabController.Instance.GroupRoleAdmin)
                return true;
        return false;
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
    public string titleId;
    public string PlayfabId;
    public string DisplayName;
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
    public string InvitedEntityName;
    public string InvitedByEntityName;
}