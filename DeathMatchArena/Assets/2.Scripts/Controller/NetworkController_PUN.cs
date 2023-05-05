using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Photon.Pun;
using System;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkController_PUN : MonoBehaviourPunCallbacks
{
    #region Singleton
    protected NetworkController_PUN() { }

    private static NetworkController_PUN f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static NetworkController_PUN Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<NetworkController_PUN>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static NetworkController_PUN AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<NetworkController_PUN>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    public static readonly int MAX_PLAYER_IN_ROOM = 4;
    public static readonly string PLAYER_READY_STATE = "PLAYER_READY_STATE";
    public static readonly string PLAYER_NAME = "PLAYER_NAME";
    public static readonly string ROOM_SLOT = "ROOM_SLOT";

    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    private Action OnConnecctCompleted;
    public static event Action ActionOnConnectedToMaster;
    public static event Action ActionOnRoomListUpdate;
    public static event Action ActionOnJoinedLobby;
    public static event Action ActionOnLeftLobby;
    public static event Action ActionOnCreateRoomFailed;
    public static event Action ActionOnJoinRoomFailed;
    public static event Action ActionOnJoinRandomFailed;
    public static event Action ActionOnJoinedRoom;
    public static event Action ActionOnLeftRoom;
    public static event Action ActionOnPlayerEnteredRoom;
    public static event Action ActionOnPlayerLeftRoom;
    public static event Action ActionOnMasterClientSwitched;
    public static event Action ActionOnPlayerPropertiesUpdate;

    public static event Action ActionOnPlayerListChanged;

    public string MasterClientId;
    #endregion

    #region Unity
    public override void OnEnable()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        base.OnEnable();
    }
    #endregion

    #region PUN CALLBACKS
    public override void OnConnectedToMaster()
    {

        Debug.Log($"[{this.name}]:PUN Connected.");
        JoinLobby();
        OnConnecctCompleted?.Invoke();
        ActionOnConnectedToMaster?.Invoke();
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log($"[{this.name}]:PUN Room list update.");
        foreach (RoomInfo info in roomList)
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
            }
            // Add new room info to cache
            else
            {
                cachedRoomList.Add(info.Name, info);
            }
        }

        ActionOnRoomListUpdate?.Invoke();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log($"[{this.name}]:PUN Joined lobby.");
        ActionOnJoinedLobby?.Invoke();
    }
    public override void OnLeftLobby()
    {
        Debug.Log($"[{this.name}]:PUN Left lobby.");
        ActionOnLeftLobby?.Invoke();
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"[{this.name}]:PUN Create room fail.");
        ActionOnCreateRoomFailed?.Invoke();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"[{this.name}]:PUN Join room fail.");
        ActionOnJoinRoomFailed?.Invoke();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"[{this.name}]:PUN Join random room fail.");
        ActionOnJoinRandomFailed?.Invoke();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"[{this.name}]:PUN Joined room.");
        cachedRoomList.Clear();

        Hashtable props = new Hashtable
        {
            {PLAYER_READY_STATE, false},
            {PLAYER_NAME, PlayerData.GetNickName()},

        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        ActionOnJoinedRoom?.Invoke();
        ActionOnPlayerListChanged?.Invoke();
    }
    public override void OnLeftRoom()
    {
        Debug.Log($"[{this.name}]:PUN Left room.");
        JoinLobby();
        ActionOnLeftRoom?.Invoke();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[{this.name}]:PUN {newPlayer.NickName} Enter room.");
        ActionOnPlayerEnteredRoom?.Invoke();
        ActionOnPlayerListChanged?.Invoke();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[{this.name}]:PUN {otherPlayer.NickName} Left room.");
        ActionOnPlayerLeftRoom?.Invoke();
        ActionOnPlayerListChanged?.Invoke();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"[{this.name}]:PUN {newMasterClient.NickName} is new master client.");
        MasterClientId = newMasterClient.NickName;
        ActionOnPlayerLeftRoom?.Invoke();
        ActionOnPlayerListChanged?.Invoke();
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log($"[{this.name}]:PUN {targetPlayer.NickName} changed his properties.");
        ActionOnPlayerPropertiesUpdate?.Invoke();
        ActionOnPlayerListChanged?.Invoke();
    }
    #endregion

    #region Public Action
    public void ConnectPUN(Action OnComplete)
    {
        if (PhotonNetwork.IsConnected == false)
        {
            PhotonNetwork.LocalPlayer.NickName = PlayerData.GetId();
            PhotonNetwork.ConnectUsingSettings();
            OnConnecctCompleted = OnComplete;
        }

    }
    public void JoinLobby()
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }
    public string GetRoomName()
    {
        if (PhotonNetwork.InRoom)
        {
            return PhotonNetwork.CurrentRoom.Name;
        }
        return "";
    }
    public void CreateRoom(string roomName)
    {
        RoomOptions options = new RoomOptions { MaxPlayers = (byte)MAX_PLAYER_IN_ROOM, PlayerTtl = 0 };
        PhotonNetwork.CreateRoom(roomName, options, null);
    }
    public void JoinRoom(string roomName)
    {
        JoinLobby();
        PhotonNetwork.JoinRoom(roomName);
    }
    public void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Player Properties
    public Dictionary<Player, int> GetPlayersSlot()
    {
        Dictionary<Player, int> PlayerInSlot = new Dictionary<Player, int>();
        if (PhotonNetwork.InRoom == false) return new Dictionary<Player, int>();
        foreach (var player in PhotonNetwork.CurrentRoom.Players.Values)
        {
            object slot;
            Hashtable data = player.CustomProperties;
            if (data.TryGetValue(ROOM_SLOT, out slot))
            {
                if (slot is int)
                {
                    int slotIndex = (int)slot;
                    if (slotIndex >= 0 && slotIndex <= 3)
                    {
                        PlayerInSlot[player] = slotIndex;
                    }
                    else
                    {
                        PlayerInSlot[player] = -1;
                    }
                }
                else
                {
                    PlayerInSlot[player] = -1;
                }
            }
            else
            {
                PlayerInSlot[player] = -1;
            }
        }
        return PlayerInSlot;
    }
    public bool isSlotEmpty(int slotIndex)
    {
        Dictionary<Player, int> players = GetPlayersSlot();

        foreach (var index in players.Values)
        {
            if (index == slotIndex) return false;
        }
        return true;
    }
    public void SetSlot(int slotIndex)
    {
        if (isSlotEmpty(slotIndex) == false) return;
        Hashtable props = new Hashtable { { ROOM_SLOT, slotIndex } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public object GetPropertiesValue(Player player, string propName)
    {
        object value;
        Hashtable data = player.CustomProperties;
        if (data.TryGetValue(propName, out value))
        {
            return value;
        }
        return null;

    }
    public bool AmIMasterClient()
    {
        return PhotonNetwork.LocalPlayer.IsMasterClient;
    }
    public void SetReady(bool isReady)
    {
        Hashtable props = new Hashtable { { PLAYER_READY_STATE, isReady } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    #endregion
}
