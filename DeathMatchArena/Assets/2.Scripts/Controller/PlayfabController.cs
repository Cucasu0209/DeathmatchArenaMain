using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using PlayFab;
using Newtonsoft.Json;
using PlayFab.DataModels;

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


    private readonly string PlayfabDataName_RequestAddFriend = "RequestAddFriend";
    private readonly string PlayfabDataName_InvitationFriend = "InvitationFriend";

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
                    Debug.Log(player.getInf());
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
    public void GetSubscribedGroupChat(Action OnComplete)
    {
        var data = new Dictionary<string, object>()
        {
            {"fsdfsdfsdf", 100},
            {"Mana", 10000}
        };
        var dataList = new List<SetObject>()
        {
            new SetObject()
            {
                ObjectName = "PlayerData",
                DataObject = data
            },
    // A free-tier customer may store up to 3 objects on each entity
        };


        PlayFabGroupsAPI.ListMembership(new PlayFab.GroupsModels.ListMembershipRequest()
        {
        },
        (result) =>
        {
            string debu = "";
            foreach (var group in result.Groups)
            {
                debu += group.GroupName + ",";
                PlayFabGroupsAPI.ListGroupMembers(new PlayFab.GroupsModels.ListGroupMembersRequest()
                {
                    Group = group.Group,
                },
                (result) =>
                {
                    string debua = "";
                    foreach (var member in result.Members)
                    {
                        //debua += $"({member.Members[0].Key.Id},{member.RoleId},{member.RoleName})";
                        foreach (var membe in member.Members)
                        {
                            debua += $"({membe.Key.Id},{member.RoleId},{member.RoleName})";

                        }
                    }
                    Debug.Log($"[{this.name}]:Get Groups member success group:{group.GroupName}, {debua}");
                },
                (error) =>
                {
                    Debug.Log($"[{this.name}]:Get Groups member fail { error.ErrorMessage}");
                });

                PlayFabDataAPI.SetObjects(new SetObjectsRequest()
                {
                    Entity = new EntityKey() { Id = group.Group.Id, Type = group.Group.Type }, // Saved from GetEntityToken, or a specified key created from a titlePlayerId, CharacterId, etc
                    Objects = dataList,
                    
                },
                (result) =>
                {

                    Debug.Log($"[{this.name}]:update Groups data success {group.GroupName}");
                },
                (error) =>
                {
                    Debug.Log($"[{this.name}]:update Groups data member fail {group.GroupName} { error.ErrorMessage}");
                });

            }
            Debug.Log($"[{this.name}]:Get Groups success {debu}, {result.Groups.Count}");
            OnComplete?.Invoke();
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Remove friend fail { error.ErrorMessage}");
            OnComplete?.Invoke();
        });
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

    public string getInf()
    {
        return DisplayName + ":" + PlayFabId;
    }
}