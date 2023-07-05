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
        //PhotonNetwork.AutomaticallySyncScene = true;
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
            {PlayerProperties.PLAYER_READY_STATE, false},
            {PlayerProperties.PLAYER_NAME, PlayerData.GetNickName()},
            {PlayerProperties.PLAYER_HEALTH,PlayerProperties. MAX_HEALTH},
            {PlayerProperties.PLAYER_PHYSICAL, PlayerProperties.MAX_PHYSICAL},
            {PlayerProperties.PLAYER_WEAPON, PlayerData.GetCurrentWeaponIndex()},
            {PlayerProperties.PLAYER_HAT, PlayerData.GetCurrentHatIndex()},
            {PlayerProperties.PLAYER_SHOE, PlayerData.GetCurrentShoeIndex()},

        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        ActionOnPlayerListChanged?.Invoke();
        ActionOnJoinedRoom?.Invoke();

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
        ActionOnPlayerListChanged?.Invoke();
        ActionOnPlayerEnteredRoom?.Invoke();

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"[{this.name}]:PUN {otherPlayer.NickName} Left room.");
        ActionOnPlayerListChanged?.Invoke();
        ActionOnPlayerLeftRoom?.Invoke();

    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        Debug.Log($"[{this.name}]:PUN {newMasterClient.NickName} is new master client.");
        ActionOnPlayerListChanged?.Invoke();
        MasterClientId = newMasterClient.NickName;
        ActionOnPlayerLeftRoom?.Invoke();

    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        Debug.Log($"[{this.name}]:PUN {targetPlayer.NickName} changed his properties.");
        ActionOnPlayerListChanged?.Invoke();
        ActionOnPlayerPropertiesUpdate?.Invoke();

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
        RoomOptions options = new RoomOptions { MaxPlayers = (byte)PlayerProperties.MAX_PLAYER_IN_ROOM, PlayerTtl = 0 };
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

    public void LockCurrentRoom()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient == false || PhotonNetwork.CurrentRoom == null) return;

        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }

    public void UnLockCurrentRoom()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient == false || PhotonNetwork.CurrentRoom == null) return;

        PhotonNetwork.CurrentRoom.IsVisible = true;
        PhotonNetwork.CurrentRoom.IsOpen = true;
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
            if (data.TryGetValue(PlayerProperties.ROOM_SLOT, out slot))
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
        UpdateMyProperty(PlayerPropertiesType.slotIndex, slotIndex);
    }
    public T GetPropertiesValue<T>(Player player, string propName, T defaultResult)
    {
        object value;
        Hashtable data = player.CustomProperties;
        if (data.TryGetValue(propName, out value))
        {
            if (value is T)
                return (T)value;
        }
        return defaultResult;

    }
    public bool AmIMasterClient()
    {
        return PhotonNetwork.LocalPlayer.IsMasterClient;
    }
    public bool CheckIfMasterPlayGame()
    {
        return GetPropertiesValue<bool>(PhotonNetwork.MasterClient, PlayerProperties.IS_MASTER_PLAY_GAME, false);
    }
    public string GetId(Player player)
    {
        return player.NickName;
    }
    public PlayerProperties GetPlayerProperties(Player player)
    {
        if (player == null) return new PlayerProperties();
        return new PlayerProperties()
        {
            isReady = GetPropertiesValue<bool>(player, PlayerProperties.PLAYER_READY_STATE, false),
            isLoadedLevel = GetPropertiesValue<bool>(player, PlayerProperties.PLAYER_LOADED_LEVEL, false),
            playerId = player.NickName,
            playerName = GetPropertiesValue<string>(player, PlayerProperties.PLAYER_NAME, "Bot"),
            isMasterPlaygame = GetPropertiesValue<bool>(player, PlayerProperties.IS_MASTER_PLAY_GAME, false),
            slotInRoom = GetPropertiesValue<int>(player, PlayerProperties.ROOM_SLOT, 0),
            playerHealth = GetPropertiesValue<int>(player, PlayerProperties.PLAYER_HEALTH, PlayerProperties.MAX_HEALTH),
            playerPhysical = GetPropertiesValue<int>(player, PlayerProperties.PLAYER_PHYSICAL, PlayerProperties.MAX_PHYSICAL),
            weaponIndex = GetPropertiesValue<int>(player, PlayerProperties.PLAYER_WEAPON, 0),
            hatIndex = GetPropertiesValue<int>(player, PlayerProperties.PLAYER_HAT, 0),
            shoeIndex = GetPropertiesValue<int>(player, PlayerProperties.PLAYER_SHOE, 0),

        };
    }
    public void UpdateMyProperty<T>(PlayerPropertiesType type, T value)
    {
        string propName = "";
        if (type == PlayerPropertiesType.isMasterPlayGame && PhotonNetwork.LocalPlayer.IsMasterClient == false) return;
        switch (type)
        {
            case PlayerPropertiesType.isReady: propName = PlayerProperties.PLAYER_READY_STATE; break;
            case PlayerPropertiesType.isLoaded: propName = PlayerProperties.PLAYER_LOADED_LEVEL; break;
            case PlayerPropertiesType.name: propName = PlayerProperties.PLAYER_NAME; break;
            case PlayerPropertiesType.isMasterPlayGame: propName = PlayerProperties.IS_MASTER_PLAY_GAME; break;
            case PlayerPropertiesType.slotIndex: propName = PlayerProperties.ROOM_SLOT; break;
            case PlayerPropertiesType.health: propName = PlayerProperties.PLAYER_HEALTH; break;
            case PlayerPropertiesType.physical: propName = PlayerProperties.PLAYER_PHYSICAL; break;
            case PlayerPropertiesType.weapon: propName = PlayerProperties.PLAYER_WEAPON; break;
            case PlayerPropertiesType.hat: propName = PlayerProperties.PLAYER_HAT; break;
            case PlayerPropertiesType.shoe: propName = PlayerProperties.PLAYER_SHOE; break;
        }

        Hashtable props = new Hashtable { { propName, value } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }
    public void ResetGamePlayProperties()
    {
        UpdateMyProperty<int>(PlayerPropertiesType.health, PlayerProperties.MAX_HEALTH);
        UpdateMyProperty<int>(PlayerPropertiesType.physical, PlayerProperties.MAX_PHYSICAL);
    }
    #endregion
}

public class PlayerProperties
{
    public static readonly int MAX_PLAYER_IN_ROOM = 4;
    public static readonly string PLAYER_READY_STATE = "PLAYER_READY_STATE";
    public static readonly string PLAYER_LOADED_LEVEL = "PLAYER_LOADED_LEVEL";
    public static readonly string PLAYER_NAME = "PLAYER_NAME";
    public static readonly string IS_MASTER_PLAY_GAME = "IS_MASTER_PLAY_GAME";
    public static readonly string ROOM_SLOT = "ROOM_SLOT";
    public static readonly string PLAYER_HEALTH = "PLAYER_HEALTH";
    public static readonly string PLAYER_PHYSICAL = "PLAYER_PHYSICAL";
    public static readonly string PLAYER_WEAPON = "PLAYER_WEAPON";
    public static readonly string PLAYER_HAT = "PLAYER_HAT";
    public static readonly string PLAYER_SHOE = "PLAYER_SHOE";
    public static readonly int MAX_HEALTH = 100;
    public static readonly int MAX_PHYSICAL = 100;

    public bool isReady;
    public bool isLoadedLevel;
    public string playerId;
    public string playerName;
    public bool isMasterPlaygame;
    public int slotInRoom;
    public int playerHealth;
    public int playerPhysical;
    public int weaponIndex;
    public int hatIndex;
    public int shoeIndex;
}

public enum PlayerPropertiesType
{
    isReady, isLoaded, name, isMasterPlayGame, slotIndex, health, physical, weapon, hat, shoe
}
