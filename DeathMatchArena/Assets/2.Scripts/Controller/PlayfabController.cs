using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using PlayFab;
using Newtonsoft.Json;
using PlayFab.DataModels;
using PlayFab.Internal;
using System.Text;

public class PlayfabController : MonoBehaviour
{
    #region Singleton
    protected PlayfabController() { }

    private static PlayfabController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static PlayfabController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<PlayfabController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static PlayfabController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<PlayfabController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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

    #region Variables Playfab
    private readonly string PlayfabFunctionCloud_Attendance = "attendance";
    private readonly string PlayfabFunctionCloud_GetAllPlayers = "getAllPlayers";
    private readonly string PlayfabFunctionCloud_GetEloLeaderboard = "getEloLeaderboard";

    private readonly string PlayfabFunctionCloud_SetWeaponIndex = "setWeaponIndex";
    private readonly string PlayfabFunctionCloud_SetHatIndex = "setHatIndex";
    private readonly string PlayfabFunctionCloud_SetShoeIndex = "setShoeIndex";

    private readonly string PlayfabFunctionCloud_GetWeaponIndex = "getWeaponIndex";
    private readonly string PlayfabFunctionCloud_GetHatIndex = "getHatIndex";
    private readonly string PlayfabFunctionCloud_GetShoeIndex = "getShoeIndex";

    private readonly string PlayfabFunctionCloud_AddCurrency = "addCurrency";
    private readonly string PlayfabFunctionCloud_GetCurrency = "getCurrency";

    private readonly string PlayfabFunctionCloud_AddElo = "addElo";
    private readonly string PlayfabFunctionCloud_GetElo = "getElo";

    private readonly string PlayfabDataName_RequestAddFriend = "RequestAddFriend";
    private readonly string PlayfabDataName_InvitationFriend = "InvitationFriend";
    private readonly string PlayfabDataName_FriendChatMessage = "FriendChatMessage";
    private readonly string PlayfabDataName_GroupChatMessage = "GroupChatMessage";

    private readonly string PlayfabDataName_WeanponOwned = "WeanponOwned";
    private readonly string PlayfabDataName_HatOwned = "HatOwned";
    private readonly string PlayfabDataName_ShoeOwned = "ShoeOwned";


    public string GroupRoleMember = "members";
    public string GroupRoleAdmin = "admins";
    //Files
    private readonly Dictionary<string, string> _entityFileJson = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _tempUpdates = new Dictionary<string, string>();
    public string ActiveUploadFileName;
    public string NewFileName;
    public int GlobalFileLock = 0;
    #endregion

    #region General Variables

    #endregion

    #region Playfab Call API Yourself
    private void AttendancePlayfab(Action OnComplete)
    {
        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = PlayfabFunctionCloud_Attendance,
        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                Debug.Log($"[{this.name}]:Attendance {result.FunctionResult}");
            }
            else
            {
                Debug.Log($"[{this.name}]:Attendance fail");
            }
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Attendance fail");
            OnComplete?.Invoke();
        });
    }
    public void GetAllPlayersPlayfab(int index, Action<List<PlayerPlayfabInformation>> OnComplete)
    {
        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = PlayfabFunctionCloud_GetAllPlayers,
            FunctionParameter = new { startPos = index }
        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                List<PlayerPlayfabInformation> Players = JsonConvert.DeserializeObject<List<PlayerPlayfabInformation>>(result.FunctionResult.ToString());
                Debug.Log($"[{this.name}]:Get All Players {Players.Count}");
                foreach (var player in Players)
                {
                    Debug.Log($"[{this.name}]: player inf" + player.getInf());
                }
                OnComplete?.Invoke(Players);
            }
            else
            {
                Debug.Log($"[{this.name}]:Get All Players Fail");
            }

        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get All Players Fail");
        });
    }
    public void GetEloLeaderboardPlayfab(Action<List<PlayerPlayfabInformation>> OnComplete)
    {
        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = PlayfabFunctionCloud_GetEloLeaderboard,
        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                List<PlayerPlayfabInformation> Players = JsonConvert.DeserializeObject<List<PlayerPlayfabInformation>>(result.FunctionResult.ToString());
                Debug.Log($"[{this.name}]:GetEloLeaderboardPlayfab {Players.Count} {result.FunctionResult}");
                foreach (var player in Players)
                {
                    Debug.Log($"[{this.name}]: GetEloLeaderboardPlayfab player inf" + player.getInf());
                }
                PlayerData.SetEloLeaderboard(Players);
                OnComplete?.Invoke(Players);
            }
            else
            {
                Debug.Log($"[{this.name}]:GetEloLeaderboardPlayfab Fail");
            }

        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get All Players Fail");
        });
    }
    public void SetEquipPlayfab(ItemType type, int index, Action OnComplete)
    {
        string functionname = "";
        switch (type)
        {
            case ItemType.Weapon:
                functionname = PlayfabFunctionCloud_SetWeaponIndex;
                PlayerData.SetCurrentWeaponIndex(index);
                break;
            case ItemType.Hat:
                functionname = PlayfabFunctionCloud_SetHatIndex;
                PlayerData.SetCurrentHatIndex(index);
                break;
            case ItemType.Shoe:
                functionname = PlayfabFunctionCloud_SetShoeIndex;
                PlayerData.SetCurrentShoeIndex(index);
                break;
        }

        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = functionname,
            FunctionParameter = new { id = index }
        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                Debug.Log($"[{this.name}]:SetEquipPlayfab {functionname} {result.FunctionResult}");
            }
            else
            {
                Debug.Log($"[{this.name}]:SetEquipPlayfab {functionname} fail");
            }
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:SetEquipPlayfab {functionname} fail");
            OnComplete?.Invoke();
        });

    }
    public void GetEquipPlayfab(ItemType type, string playfabId, Action<ItemType, string, int> OnComplete)
    {
        string functionname = "";
        switch (type)
        {
            case ItemType.Weapon:
                functionname = PlayfabFunctionCloud_GetWeaponIndex;
                break;
            case ItemType.Hat:
                functionname = PlayfabFunctionCloud_GetHatIndex;
                break;
            case ItemType.Shoe:
                functionname = PlayfabFunctionCloud_GetShoeIndex;
                break;
        }

        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = functionname,
            FunctionParameter = new { playfabId = playfabId }
        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                int index = int.Parse(result.FunctionResult.ToString());

                Debug.Log($"[{this.name}]:GetEquipPlayfab {functionname} {result.FunctionResult}");
                if (PlayerData.GetId() == playfabId)
                {
                    switch (type)
                    {
                        case ItemType.Weapon:
                            PlayerData.SetCurrentWeaponIndex(index);
                            break;
                        case ItemType.Hat:
                            PlayerData.SetCurrentHatIndex(index);
                            break;
                        case ItemType.Shoe:
                            PlayerData.SetCurrentShoeIndex(index);
                            break;
                    }
                }

                OnComplete?.Invoke(type, playfabId, index);
            }
            else
            {
                Debug.Log($"[{this.name}]:GetEquipPlayfab {functionname} fail");
                OnComplete?.Invoke(ItemType.Hat, "", -1);
            }
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:GetEquipPlayfab {functionname} fail");
            OnComplete?.Invoke(ItemType.Hat, "", -1);
        });
    }
    public void GetItemOwned(ItemType type, Action<ItemType, List<int>> OnComplete)
    {
        string keyName = "";
        switch (type)
        {
            case ItemType.Weapon:
                keyName = PlayfabDataName_WeanponOwned;
                break;
            case ItemType.Hat:
                keyName = PlayfabDataName_HatOwned;
                break;
            case ItemType.Shoe:
                keyName = PlayfabDataName_ShoeOwned;
                break;
        }
        PlayFabClientAPI.GetUserData(new PlayFab.ClientModels.GetUserDataRequest()
        {
            Keys = new List<string>() { keyName },
        },
        (result) =>
        {
            List<int> listReqest = new List<int>();
            if (result.Data.ContainsKey(keyName))
            {
                listReqest = JsonConvert.DeserializeObject<List<int>>(result.Data[keyName].Value);
                Debug.Log($"[{this.name}]:Get Item Owned  {listReqest.Count}");
            }
            else
            {
                Debug.Log($"[{this.name}]:Get Item Owned  - No Request");

            }
            switch (type)
            {
                case ItemType.Weapon:
                    PlayerData.SetWeaponOwned(listReqest);
                    break;
                case ItemType.Hat:
                    PlayerData.SetHatOwned(listReqest);
                    break;
                case ItemType.Shoe:
                    PlayerData.SetShoeOwned(listReqest);
                    break;
            }
            OnComplete?.Invoke(type, listReqest);
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get Item Owned fail {error.ErrorMessage}");
            OnComplete?.Invoke(type, new List<int>());
        });
    }
    public void AdditemOwned(ItemType type, int index, Action OnComplete)
    {
        string keyName = "";
        List<int> list = new List<int>();
        switch (type)
        {
            case ItemType.Weapon:
                keyName = PlayfabDataName_WeanponOwned;
                PlayerData.AddWeaponOwned(index);
                list = PlayerData.GetWeaponOwned();
                break;
            case ItemType.Hat:
                keyName = PlayfabDataName_HatOwned;
                PlayerData.AddHatOwned(index);
                list = PlayerData.GetHatOwned();
                break;
            case ItemType.Shoe:
                keyName = PlayfabDataName_ShoeOwned;
                PlayerData.AddShoeOwned(index);
                list = PlayerData.GetShoeOwned();
                break;
        }



        PlayFabClientAPI.UpdateUserData(new PlayFab.ClientModels.UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
                {
                    {keyName, JsonConvert.SerializeObject(list)},
                }
        },
        (result) =>
        {
            GetItemOwned(type, null);
            Debug.Log($"[{this.name}]:AdditemOwned success");
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:AdditemOwned success { error.ErrorMessage}");
            OnComplete?.Invoke();
        });



    }
    public void GetCurrencyPlayfab(Action<int> OnComplete)
    {
        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = PlayfabFunctionCloud_GetCurrency,
            FunctionParameter = new { playfabId = PlayerData.GetId() }
        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                Debug.Log($"[{this.name}]:GetCurrencyPlayfab {result.FunctionResult}");
                int s = int.Parse(result.FunctionResult.ToString());
                PlayerData.SetCurrency(s);
                OnComplete?.Invoke(int.Parse(result.FunctionResult.ToString()));
            }
            else
            {
                Debug.Log($"[{this.name}]:GetCurrencyPlayfab  fail");
                OnComplete?.Invoke(0);
            }
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:GetCurrencyPlayfab fail");
            OnComplete?.Invoke(0);
        });
    }
    public void AddCurrencyPlayfab(int pl, Action OnComplete)
    {
        PlayerData.AddCurrency(pl);
        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = PlayfabFunctionCloud_AddCurrency,
            FunctionParameter = new { playfabId = PlayerData.GetId(), currencyAdd = pl }

        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                Debug.Log($"[{this.name}]:AddCurrencyPlayfab {result.FunctionResult}");
            }
            else
            {
                Debug.Log($"[{this.name}]:AddCurrencyPlayfab  fail {result.Error.StackTrace}");

            }
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:AddCurrencyPlayfab fail");
            OnComplete?.Invoke();
        });
    }
    public void GetEloPlayfab(Action<int> OnComplete)
    {
        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = PlayfabFunctionCloud_GetElo,
            FunctionParameter = new { playfabId = PlayerData.GetId() }
        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                Debug.Log($"[{this.name}]:GetEloPlayfab {result.FunctionResult}");
                int s = int.Parse(result.FunctionResult.ToString());
                PlayerData.SetElo(s);
                OnComplete?.Invoke(int.Parse(result.FunctionResult.ToString()));
            }
            else
            {
                Debug.Log($"[{this.name}]:GetEloPlayfab  fail");
                OnComplete?.Invoke(0);
            }
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:GetEloPlayfab fail");
            OnComplete?.Invoke(0);
        });
    }
    public void AddEloPlayfab(int pl, Action OnComplete)
    {
        PlayerData.AddElo(pl);
        PlayFabClientAPI.ExecuteCloudScript(new PlayFab.ClientModels.ExecuteCloudScriptRequest()
        {
            FunctionName = PlayfabFunctionCloud_AddElo,
            FunctionParameter = new { playfabId = PlayerData.GetId(), eloAdd = pl }

        },
        (result) =>
        {
            if (result.FunctionResult != null)
            {
                Debug.Log($"[{this.name}]:AddEloPlayfab {result.FunctionResult}");
            }
            else
            {
                Debug.Log($"[{this.name}]:AddEloPlayfab  fail {result.Error.StackTrace}");

            }
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:AddEloPlayfab fail");
            OnComplete?.Invoke();
        });
    }
    public void GetRequestAddfriend(Action<List<string>> OnComplete)
    {
        PlayFabClientAPI.GetUserData(new PlayFab.ClientModels.GetUserDataRequest()
        {
            Keys = new List<string>() { PlayfabDataName_RequestAddFriend },
        },
        (result) =>
        {
            if (result.Data.ContainsKey(PlayfabDataName_RequestAddFriend))
            {
                List<string> listReqest = JsonConvert.DeserializeObject<List<string>>(result.Data[PlayfabDataName_RequestAddFriend].Value);
                Debug.Log($"[{this.name}]:Get Request Addfriend - Request Count {listReqest.Count}");
                OnComplete?.Invoke(listReqest);
            }
            else
            {
                Debug.Log($"[{this.name}]:Get Request Addfriend - No Request");
                OnComplete?.Invoke(new List<string>());
            }
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get Request Addfriend fail {error.ErrorMessage}");
            OnComplete?.Invoke(new List<string>());
        });
    }
    public void UpdateRequestAddfriend(List<string> newList, Action Oncomplete)
    {
        PlayFabClientAPI.UpdateUserData(new PlayFab.ClientModels.UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {PlayfabDataName_RequestAddFriend, JsonConvert.SerializeObject(newList)},
            }
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Update Request Addfriend success");
            Oncomplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Update Request Addfriend success { error.ErrorMessage}");
            Oncomplete?.Invoke();
        });
    }
    public void GetInvitationfriend(Action<List<string>> OnComplete)
    {
        PlayFabClientAPI.GetUserData(new PlayFab.ClientModels.GetUserDataRequest()
        {
            Keys = new List<string>() { PlayfabDataName_InvitationFriend },
        },
        (result) =>
        {
            if (result.Data.ContainsKey(PlayfabDataName_InvitationFriend))
            {
                List<string> listReqest = JsonConvert.DeserializeObject<List<string>>(result.Data[PlayfabDataName_InvitationFriend].Value);
                Debug.Log($"[{this.name}]:Get Invitation friend - Request Count {listReqest.Count}");
                OnComplete?.Invoke(listReqest);
            }
            else
            {
                Debug.Log($"[{this.name}]:Get Invitation friend - No Request");
                OnComplete?.Invoke(new List<string>());
            }
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get Invitation friend fail {error.ErrorMessage}");
            OnComplete?.Invoke(new List<string>());
        });
    }
    public void UpdateInvitationfriend(List<string> newList, Action Oncomplete)
    {
        PlayFabClientAPI.UpdateUserData(new PlayFab.ClientModels.UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {PlayfabDataName_InvitationFriend, JsonConvert.SerializeObject(newList)},
            }
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Update Invitation friend success");
            Oncomplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Update Invitation friend success { error.ErrorMessage}");
            Oncomplete?.Invoke();
        });
    }
    public void GetAllFriendPlayfab(Action<List<PlayerPlayfabInformation>> OnComplete)
    {

        PlayFabClientAPI.GetFriendsList(new PlayFab.ClientModels.GetFriendsListRequest()
        {

        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Get all friend success");
            List<PlayerPlayfabInformation> friends = new List<PlayerPlayfabInformation>();
            foreach (var friend in result.Friends)
            {
                friends.Add(new PlayerPlayfabInformation()
                {
                    PlayFabId = friend.FriendPlayFabId,
                    DisplayName = friend.TitleDisplayName
                });
                Debug.Log("trung friend :" + friend.TitleDisplayName + " " + friend.FriendPlayFabId);
            }
            OnComplete?.Invoke(friends);
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get all friend fail { error.ErrorMessage}");
            OnComplete?.Invoke(new List<PlayerPlayfabInformation>());
        });
    }
    public void AddNewFriendPlayfab(string playfabid, Action OnComplete)
    {
        PlayFabClientAPI.AddFriend(new PlayFab.ClientModels.AddFriendRequest()
        {
            FriendPlayFabId = playfabid,
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Add friend success");
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Add friend fail { error.ErrorMessage}");
            OnComplete?.Invoke();
        });
    }
    public void RemoveFriendPlayfab(string playfabid, Action OnComplete)
    {
        PlayFabClientAPI.RemoveFriend(new PlayFab.ClientModels.RemoveFriendRequest
        {
            FriendPlayFabId = playfabid
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Remove friend success");
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Remove friend fail { error.ErrorMessage}");
            OnComplete?.Invoke();
        });

    }
    #endregion

    #region Playfab Call Group API
    public void GetSubscribedGroupChat(Action<Dictionary<string, GroupChatInfomation>> OnComplete)
    {
        PlayFabGroupsAPI.ListMembership(new PlayFab.GroupsModels.ListMembershipRequest()
        {

        },
        (result) =>
        {
            string _debugcontent_ = "";
            Dictionary<string, GroupChatInfomation> ListGroup = new Dictionary<string, GroupChatInfomation>();
            foreach (var group in result.Groups)
            {
                _debugcontent_ += group.GroupName + ",";
                ListGroup.Add(group.Group.Id, new GroupChatInfomation()
                {
                    GroupEntity = group.Group,
                    GroupName = group.GroupName,
                    members = new List<GroupChatPlayerRole>()
                });
            }
            Debug.Log($"[{this.name}]:Get Groups success {_debugcontent_}: {result.Groups.Count}");
            OnComplete?.Invoke(ListGroup);
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Remove friend fail { error.ErrorMessage}");
            OnComplete?.Invoke(new Dictionary<string, GroupChatInfomation>());
        });
    }
    public void GetGroupChatMembers(GroupChatInfomation GroupInf, Action<List<GroupChatPlayerRole>> OnComplete)
    {
        PlayFabGroupsAPI.ListGroupMembers(new PlayFab.GroupsModels.ListGroupMembersRequest()
        {
            Group = GroupInf.GroupEntity,
        },
        (result) =>
        {
            string debua = "";
            List<GroupChatPlayerRole> members = new List<GroupChatPlayerRole>();
            int numberOfLoaded = 0;
            int maxmember = 0;
            foreach (var member in result.Members)
            {
                maxmember += member.Members.Count;
            }
            if (maxmember == 0)
            {
                OnComplete?.Invoke(members);
            }
            foreach (var member in result.Members)
            {
                //debua += $"({member.Members[0].Key.Id},{member.RoleId},{member.RoleName})";
                foreach (var membe in member.Members)
                {
                    debua += $"({membe.Key.Id},{member.RoleId},{member.RoleName}, { membe.Lineage["master_player_account"].Id})";
                    GetDisplayNameFromPlayFabID(membe.Lineage["master_player_account"].Id, (displayname) =>
                    {
                        GroupChatPlayerRole newPlayerRole = new GroupChatPlayerRole()
                        {
                            DisplayName = displayname,
                            PlayfabId = membe.Lineage["master_player_account"].Id,
                            titleId = membe.Key.Id,
                            playerRoleId = member.RoleId,
                            playerRoleName = member.RoleName,
                        };
                        members.Add(newPlayerRole);
                        numberOfLoaded++;
                        Debug.Log("trungloz  " + GroupInf.GroupName + " " + numberOfLoaded + "/" + result.Members.Count);
                        if (maxmember == numberOfLoaded)
                        {
                            OnComplete?.Invoke(members);
                        }
                    });



                }
            }
            Debug.Log($"[{this.name}]:Get Groups member success group:{GroupInf.GroupName}- {GroupInf.GroupEntity.Id} - {debua}");

        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get Groups member fail { error.ErrorMessage}");
            OnComplete?.Invoke(new List<GroupChatPlayerRole>());
        });
    }
    public void CreateNewGroup(string GroupName, Action<GroupChatInfomation> OnComplete)
    {
        PlayFabGroupsAPI.CreateGroup(new PlayFab.GroupsModels.CreateGroupRequest()
        {
            GroupName = GroupName,
        },
        (result) =>
        {

            Debug.Log($"[{this.name}]:Create Groups  success");
            GetGroupChatMembers(new GroupChatInfomation()
            {
                GroupEntity = result.Group,
                GroupName = result.GroupName,
            }, (members) =>
            {
                OnComplete?.Invoke(new GroupChatInfomation()
                {
                    GroupEntity = result.Group,
                    GroupName = result.GroupName,
                    members = members
                });
            });
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Create Groups fail { error.ErrorMessage}");
            OnComplete?.Invoke(new GroupChatInfomation());
        });
    }
    public void InviteMemberToGroupChat(GroupChatInfomation group, string playfabId, Action OnComplete)
    {
        GetTittlePlayerAccountID(playfabId, (entity) =>
        {
            PlayFabGroupsAPI.InviteToGroup(new PlayFab.GroupsModels.InviteToGroupRequest()
            {
                Group = group.GroupEntity,
                Entity = new PlayFab.GroupsModels.EntityKey() { Id = entity.Id, Type = entity.Type }
            },
            (result) =>
            {
                Debug.Log($"[{this.name}]:Invite 1 memeber to Groups  success");
                OnComplete?.Invoke();
            },
            (error) =>
            {
                Debug.Log($"[{this.name}]:Invite 0 memeber to Groups fail { error.ErrorMessage}");
                OnComplete?.Invoke();
            });
        });


    }
    public void GetTittlePlayerAccountID(string playfabId, Action<PlayFab.ClientModels.EntityKey> OnComplete)
    {
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest()
        {
            PlayFabId = playfabId
        }, (result) =>
        {
            OnComplete?.Invoke(result.AccountInfo.TitleInfo.TitlePlayerAccount);
        }, (error) =>
        {
            OnComplete?.Invoke(new PlayFab.ClientModels.EntityKey());
        });
    }
    public void GetGroupRequests(GroupChatInfomation group, Action<List<GroupOpportunityForm>> OnComplete)
    {
        PlayFabGroupsAPI.ListGroupInvitations(new PlayFab.GroupsModels.ListGroupInvitationsRequest()
        {
            Group = group.GroupEntity
        },
        (result) =>
        {
            string _de_ = "";
            List<GroupOpportunityForm> ListOppotunities = new List<GroupOpportunityForm>();
            int loadedInvitations = 0;
            if (result.Invitations.Count == 0)
            {
                OnComplete?.Invoke(ListOppotunities);
            }
            foreach (var request in result.Invitations)
            {
                _de_ += $"{request.InvitedByEntity.Key.Id} invite {request.InvitedEntity.Key.Id} to group\n";
                GroupOpportunityForm newop = new GroupOpportunityForm()
                {
                    group = new GroupChatInfomation() { GroupEntity = request.Group },
                    TittleInvitedByEntity = request.InvitedByEntity.Key,
                    TittleInvitedEntity = request.InvitedEntity.Key,
                    MasterInvitedByEntity = request.InvitedByEntity.Lineage["master_player_account"],
                    MasterInvitedEntity = request.InvitedEntity.Lineage["master_player_account"],
                };
                GetDisplayNameFromPlayFabID(newop.MasterInvitedEntity.Id, (MasterInvitedEntityName) =>
                {
                    GetDisplayNameFromPlayFabID(newop.MasterInvitedByEntity.Id, (MasterInvitedByEntityName) =>
                    {
                        GetGroupNameFromId(newop.group, (GroupName) =>
                        {
                            loadedInvitations++;
                            newop.group.GroupName = GroupName;
                            newop.InvitedByEntityName = MasterInvitedByEntityName;
                            newop.InvitedEntityName = MasterInvitedEntityName;
                            ListOppotunities.Add(newop);
                            if (loadedInvitations == result.Invitations.Count)
                            {
                                OnComplete?.Invoke(ListOppotunities);
                            }
                        });
                    });
                });
            }
            Debug.Log($"[{this.name}]:get Group request {_de_} group {group.GroupName}");
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:get Group fail {error.ErrorMessage}");
            OnComplete?.Invoke(new List<GroupOpportunityForm>());
        });
    }
    public void GetMembershipOpportunities(Action<List<GroupOpportunityForm>> OnComplete)
    {
        PlayFabGroupsAPI.ListMembershipOpportunities(new PlayFab.GroupsModels.ListMembershipOpportunitiesRequest()
        {
            Entity = new PlayFab.GroupsModels.EntityKey()
            {
                Id = AuthenticationController.Instance.GetEntityKey().Id,
                Type = AuthenticationController.Instance.GetEntityKey().Type,
            }
        },
        (result) =>
        {
            string _de_ = "";
            List<GroupOpportunityForm> ListOppotunities = new List<GroupOpportunityForm>();
            int loadedInvitations = 0;
            if (result.Invitations.Count == 0)
            {
                OnComplete?.Invoke(ListOppotunities);
            }
            foreach (var request in result.Invitations)
            {
                string s = "";
                foreach (var a in request.InvitedByEntity.Lineage)
                {
                    s += a.Key + " ,(id " + a.Value.Id + " type " + a.Value.Type + ")";
                }
                _de_ += $" be invite by {s} invite {request.InvitedEntity.Key.Id} to group { request.Group.Id}\n";

                GroupOpportunityForm newop = new GroupOpportunityForm()
                {
                    group = new GroupChatInfomation() { GroupEntity = request.Group },
                    TittleInvitedByEntity = request.InvitedByEntity.Key,
                    TittleInvitedEntity = request.InvitedEntity.Key,
                    MasterInvitedByEntity = request.InvitedByEntity.Lineage["master_player_account"],
                    MasterInvitedEntity = request.InvitedEntity.Lineage["master_player_account"],
                };
                GetDisplayNameFromPlayFabID(newop.MasterInvitedEntity.Id, (MasterInvitedEntityName) =>
                {
                    GetDisplayNameFromPlayFabID(newop.MasterInvitedByEntity.Id, (MasterInvitedByEntityName) =>
                    {
                        GetGroupNameFromId(newop.group, (GroupName) =>
                        {
                            loadedInvitations++;
                            newop.group.GroupName = GroupName;
                            newop.InvitedByEntityName = MasterInvitedByEntityName;
                            newop.InvitedEntityName = MasterInvitedEntityName;
                            ListOppotunities.Add(newop);

                            if (loadedInvitations == result.Invitations.Count)
                            {
                                OnComplete?.Invoke(ListOppotunities);
                            }
                        });
                    });
                });
            }
            Debug.Log($"[{this.name}]:get Group Opportunities {_de_} ");
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:get Group Opportunities fail {error.ErrorMessage} ");
            OnComplete?.Invoke(new List<GroupOpportunityForm>());
        });

    }
    public void AcceptMembershipOpportunities(GroupChatInfomation group, Action<bool> OnComplete)
    {
        PlayFabGroupsAPI.AcceptGroupInvitation(new PlayFab.GroupsModels.AcceptGroupInvitationRequest()
        {
            Group = group.GroupEntity,
            Entity = new PlayFab.GroupsModels.EntityKey()
            {
                Id = AuthenticationController.Instance.GetEntityKey().Id,
                Type = AuthenticationController.Instance.GetEntityKey().Type,
            }
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]: Accept Group {group.GroupEntity.Id} success ");
            OnComplete?.Invoke(true);
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Accept Group {group.GroupEntity.Id} fail");
            OnComplete?.Invoke(false);
        });
    }
    public void RefuseMembershipOpportunities(GroupChatInfomation group, Action OnComplete)
    {
        PlayFabGroupsAPI.RemoveGroupInvitation(new PlayFab.GroupsModels.RemoveGroupInvitationRequest()
        {
            Group = group.GroupEntity,
            Entity = new PlayFab.GroupsModels.EntityKey()
            {
                Id = AuthenticationController.Instance.GetEntityKey().Id,
                Type = AuthenticationController.Instance.GetEntityKey().Type,
            }
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Remove Group {group.GroupEntity.Id} Invitation success ");
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Remove Group {group.GroupEntity.Id} Invitation fail");
            OnComplete?.Invoke();
        });
    }
    public void GetGroupNameFromId(GroupChatInfomation group, Action<string> OnComplete)
    {
        PlayFabGroupsAPI.GetGroup(new PlayFab.GroupsModels.GetGroupRequest()
        {
            Group = group.GroupEntity,
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]: Group name {group.GroupEntity.Id} success");
            OnComplete?.Invoke(result.GroupName);
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]: Group name {error.Error} fail");
            OnComplete?.Invoke("");
        });
    }
    public void GetDisplayNameFromPlayFabID(string id, Action<string> OnComplete)
    {
        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest()
        {
            PlayFabId = id
        },
        (result) =>
        {
            OnComplete?.Invoke(result.AccountInfo.TitleInfo.DisplayName);
        },
        (error) =>
        {
            OnComplete?.Invoke("");
        });
    }
    public void RemoveMember(PlayFab.GroupsModels.EntityKey member, GroupChatInfomation group, Action OnComplete)
    {
        PlayFabGroupsAPI.RemoveMembers(new PlayFab.GroupsModels.RemoveMembersRequest()
        {
            Group = group.GroupEntity,
            Members = new List<PlayFab.GroupsModels.EntityKey>()
            {
                member
            }
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]: remove memeber Group {group.GroupEntity.Id} success ");
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]: remove memeber Group {group.GroupEntity.Id} fail");
            OnComplete?.Invoke();
        });
    }
    public void DeleteGroup(GroupChatInfomation group, Action OnComplete)
    {
        PlayFabGroupsAPI.DeleteGroup(new PlayFab.GroupsModels.DeleteGroupRequest()
        {
            Group = group.GroupEntity
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]: remove Group {group.GroupEntity.Id} success ");
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]: remove Group {group.GroupEntity.Id} fail");
            OnComplete?.Invoke();
        });
    }
    #endregion

    #region Chat Store System
    public void GetAllFriendChatMessage(Action<Dictionary<string, List<ChatPartnerMessageInfomation>>> Oncomplete)
    {
        LoadFiles(AuthenticationController.Instance.GetEntityKey(),
                PlayfabDataName_FriendChatMessage,
                (payload) =>
                {
                    Dictionary<string, List<ChatPartnerMessageInfomation>> allChat;
                    try
                    {
                        allChat = JsonConvert.DeserializeObject<Dictionary<string, List<ChatPartnerMessageInfomation>>>(payload);
                    }
                    catch (Exception e)
                    {
                        allChat = new Dictionary<string, List<ChatPartnerMessageInfomation>>();
                    }
                    Oncomplete?.Invoke(allChat);
                });
    }
    public void SetAllFriendChatMessage(Dictionary<string, List<ChatPartnerMessageInfomation>> allFriendMsg)
    {
        string content = JsonConvert.SerializeObject(allFriendMsg);
        UploadFile(
            AuthenticationController.Instance.GetEntityKey(),
            PlayfabDataName_FriendChatMessage,
            content);
    }
    public void GetGroupChatMessage(GroupChatInfomation group, Action<List<ChatPartnerMessageInfomation>> Oncomplete)
    {
        LoadFiles(new EntityKey()
        {
            Id = group.GroupEntity.Id,
            Type = group.GroupEntity.Type,
        },
                PlayfabDataName_GroupChatMessage,
                (payload) =>
                {
                    List<ChatPartnerMessageInfomation> allChat;
                    try
                    {
                        allChat = JsonConvert.DeserializeObject<List<ChatPartnerMessageInfomation>>(payload);
                    }
                    catch (Exception e)
                    {
                        allChat = new List<ChatPartnerMessageInfomation>();
                    }
                    Oncomplete?.Invoke(allChat);
                });
    }
    public void SetAllGroupChatMessage(GroupChatInfomation group, List<ChatPartnerMessageInfomation> allFriendMsg)
    {
        string content = JsonConvert.SerializeObject(allFriendMsg);
        UploadFile(new EntityKey()
        {
            Id = group.GroupEntity.Id,
            Type = group.GroupEntity.Type,
        },
            PlayfabDataName_GroupChatMessage,
            content);
    }
    #endregion

    #region Store File
    int TryLoadIndex = 0;
    IEnumerator TryLoadFileAgain(EntityKey key, string fileName, Action<string> OnComplete)
    {
        TryLoadIndex++;
        if (TryLoadIndex <= 5)
        {
            yield return new WaitForSeconds(2);
            LoadFiles(key, fileName, OnComplete);
        }
        else
        {
            OnComplete?.Invoke("");
            TryLoadIndex = 0;
        }
    }
    int TryUpLoadIndex = 0;
    IEnumerator TryUploadFileAgain(EntityKey key, string fileName, string payloadStr = "")
    {
        TryUpLoadIndex++;
        if (TryUpLoadIndex <= 5)
        {
            yield return new WaitForSeconds(2);
            UploadFile(key, fileName, payloadStr);
        }
        else
        {
            TryUpLoadIndex = 0;
        }
    }

    /// <summary>
    /// GetFile here
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fileName"></param>
    /// <param name="OnComplete"></param>
    void LoadFiles(EntityKey key, string fileName, Action<string> OnComplete)
    {
        if (GlobalFileLock != 0)
        {
            StartCoroutine(TryLoadFileAgain(key, fileName, OnComplete));
            return;
        }
        //throw new Exception("This example overly restricts file operations for safety. Careful consideration must be made when doing multiple file operations in parallel to avoid conflict.");

        GlobalFileLock += 1;

        PlayFabDataAPI.GetFiles(new GetFilesRequest()
        {
            Entity = key,
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]: Loading " + result.Metadata.Count + "  files");
            int i = 0;
            _entityFileJson.Clear();
            foreach (var eachFilePair in result.Metadata)
            {

                _entityFileJson.Add(eachFilePair.Key, null);
                if (eachFilePair.Key == fileName)
                {
                    i++;
                    GetActualFileChatFriend(eachFilePair.Value, OnComplete);
                }

            }
            GlobalFileLock -= 1; // Finish GetFiles
            if (i == 0) OnComplete?.Invoke("");
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get Friend Chat Message fail { error.ErrorMessage}");
            GlobalFileLock -= 1;
            OnComplete?.Invoke("");
        });
    }
    void GetActualFileChatFriend(PlayFab.DataModels.GetFileMetadata fileData, Action<string> Oncomplete)
    {
        GlobalFileLock += 1; // Start Each SimpleGetCall
        PlayFabHttp.SimpleGetCall(fileData.DownloadUrl,
            result =>
            {
                _entityFileJson[fileData.FileName] = Encoding.UTF8.GetString(result);
                Debug.Log($"[{this.name}]:Get Friend Chat Message success");
                Oncomplete?.Invoke(_entityFileJson[fileData.FileName]);
                GlobalFileLock -= 1;
            }, // Finish Each SimpleGetCall
            error =>
            {
                Debug.Log($"[{this.name}]:Get Friend Chat Message fail ");
                Oncomplete?.Invoke("");
            }
        );
    }
    /// <summary>
    /// UploadFile Here
    /// </summary>
    /// <param name="key"></param>
    /// <param name="fileName"></param>
    /// <param name="payloadStr"></param>
    void UploadFile(EntityKey key, string fileName, string payloadStr = "")
    {
        if (GlobalFileLock != 0)
        {
            StartCoroutine(TryUploadFileAgain(key, fileName, payloadStr));
            return;
        }
        ActiveUploadFileName = fileName;
        GlobalFileLock += 1; // Start InitiateFileUploads
        var request = new PlayFab.DataModels.InitiateFileUploadsRequest
        {
            Entity = key,
            FileNames = new List<string> { ActiveUploadFileName },
        };
        PlayFabDataAPI.InitiateFileUploads(request, (res) => { OnInitFileUpload(key, res, payloadStr); }, (error) => OnInitFailed(key, error));
    }
    void OnInitFailed(EntityKey key, PlayFabError error)
    {
        Debug.Log($"[{this.name}]: File init fail {error.ErrorMessage}");
        if (error.Error == PlayFabErrorCode.EntityFileOperationPending)
        {
            // This is an error you should handle when calling InitiateFileUploads, but your resolution path may vary
            GlobalFileLock += 1; // Start AbortFileUploads
            var request = new PlayFab.DataModels.AbortFileUploadsRequest
            {
                Entity = key,
                FileNames = new List<string> { ActiveUploadFileName },
            };
            PlayFabDataAPI.AbortFileUploads(request, (result) => { GlobalFileLock -= 1; UploadFile(key, ActiveUploadFileName); }, OnSharedFailure); GlobalFileLock -= 1; // Finish AbortFileUploads
            GlobalFileLock -= 1; // Failed InitiateFileUploads
        }
        else
            OnSharedFailure(error);
    }
    void OnSharedFailure(PlayFabError error)
    {
        Debug.Log($"[{this.name}]: Share fail: ");
        GlobalFileLock -= 1;
    }
    void OnInitFileUpload(EntityKey key, PlayFab.DataModels.InitiateFileUploadsResponse response, string payloadstring)
    {
        Debug.Log($"[{this.name}]: File Initting ");
        string payloadStr = payloadstring;
        //if (!_entityFileJson.TryGetValue(ActiveUploadFileName, out payloadStr))
        //    payloadStr = "{}";
        var payload = Encoding.UTF8.GetBytes(payloadStr);

        GlobalFileLock += 1; // Start SimplePutCall
        PlayFabHttp.SimplePutCall(response.UploadDetails[0].UploadUrl,
            payload,
            (a) => FinalizeUpload(key, a),
            error => { Debug.Log(error); }
        );
        GlobalFileLock -= 1; // Finish InitiateFileUploads
    }
    void FinalizeUpload(EntityKey key, byte[] a)
    {
        Debug.Log($"[{this.name}]: File finalize updaload");
        GlobalFileLock += 1; // Start FinalizeFileUploads
        var request = new PlayFab.DataModels.FinalizeFileUploadsRequest
        {
            Entity = key,
            FileNames = new List<string> { ActiveUploadFileName },
        };
        PlayFabDataAPI.FinalizeFileUploads(request, OnUploadSuccess, OnSharedFailure);
        GlobalFileLock -= 1; // Finish SimplePutCall
    }
    void OnUploadSuccess(PlayFab.DataModels.FinalizeFileUploadsResponse result)
    {
        Debug.Log($"[{this.name}]: File upload success: ");
        GlobalFileLock -= 1; // Finish FinalizeFileUploads
    }
    #endregion

    #region Actions
    public void ActionImediatelyAfterLogin(Action onComplete)
    {
        AttendancePlayfab(onComplete);
    }
    #endregion
}

public class PlayerPlayfabInformation
{
    public string DisplayName;
    public string PlayFabId;
    public int status;
    public int StatValue;

    public string getInf()
    {
        return DisplayName + ":" + PlayFabId + ": elo - " + StatValue;
    }
    public static PlayerPlayfabInformation GetMyInfomation()
    {
        return new PlayerPlayfabInformation()
        {
            DisplayName = PlayerData.GetNickName(),
            PlayFabId = PlayerData.GetId(),
            status = 1,
            StatValue = PlayerData.GetElo(),
        };
    }
}