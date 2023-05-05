using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Photon.Realtime;
using System;
public class RoomController : MonoBehaviour
{
    #region Singleton
    protected RoomController() { }

    private static RoomController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static RoomController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<RoomController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static RoomController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<RoomController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    public Dictionary<int, Player> PlayerInSlot = new Dictionary<int, Player>()
    {
        {0, null},// Slow1  -  Team1
        {1, null},// Slow2  -  Team1
        {2, null},// Slow1  -  Team2
        {3, null},// Slow2  -  Team2
    };
    public static event Action ActionOnPlayerListChanged;
    #endregion

    #region Unity
    private void OnEnable()
    {

        NetworkController_PUN.ActionOnPlayerListChanged += GetPlayers;
        NetworkController_PUN.ActionOnLeftRoom += ResetSlot;
        GetPlayers();
    }
    private void OnDisable()
    {
        NetworkController_PUN.ActionOnPlayerListChanged -= GetPlayers;
        NetworkController_PUN.ActionOnLeftRoom -= ResetSlot;
    }
    #endregion

    #region Public Actions
    public string GetName(Player player)
    {
        object result = NetworkController_PUN.Instance.GetPropertiesValue(player, NetworkController_PUN.PLAYER_NAME);
        if (result is string) return (string)result;
        return "";
    }
    public string GetId(Player player)
    {
        return player.NickName;
    }
    public bool GetIsMaster(Player player)
    {
        return player.IsMasterClient;
    }
    public bool GetIsReady(Player player)
    {
        object result = NetworkController_PUN.Instance.GetPropertiesValue(player, NetworkController_PUN.PLAYER_READY_STATE);
        if (result is bool) return (bool)result;
        return false;
    }
    public bool IsEveryOneReady()
    {
        if (PlayerInSlot[1] == null && PlayerInSlot[0] == null) return false;
        if (PlayerInSlot[2] == null && PlayerInSlot[3] == null) return false;
        foreach(Player p in PlayerInSlot.Values)
        {
            if (p != null)
            {
                if (GetIsReady(p) == false) return false;
            }
        }
        return true;
    }

    #endregion

    #region Private Actions
    private void GetPlayers()
    {
        ClearSlot();
        Dictionary<Player, int> players = NetworkController_PUN.Instance.GetPlayersSlot();
        foreach (var player in players)
        {
            if (player.Value >= 0 && player.Value <= 3)
            {
                PlayerInSlot[player.Value] = player.Key;
            }
        }
        foreach (var player in players)
        {
            if (player.Key.IsLocal && player.Value == -1)
            {
                NetworkController_PUN.Instance.SetSlot(GetSlotEmpty());
            }
        }
        ActionOnPlayerListChanged?.Invoke();
    }
    private int GetSlotEmpty()
    {
        if (PlayerInSlot[0] == null) { return 0; }
        Debug.LogError($"slot 0 occupy by {PlayerInSlot[0].NickName}");
        if (PlayerInSlot[2] == null) { return 2; }
        Debug.LogError($"slot 2 occupy by {PlayerInSlot[2].NickName}");
        if (PlayerInSlot[1] == null) { return 1; }
        Debug.LogError($"slot 1 occupy by {PlayerInSlot[1].NickName}");
        if (PlayerInSlot[3] == null) { return 3; }
        Debug.LogError($"slot 3 occupy by {PlayerInSlot[3].NickName}");
        return -1;
    }
    private void ResetSlot()
    {
        NetworkController_PUN.Instance.SetSlot(-1);
    }
    private void ClearSlot()
    {
        PlayerInSlot[0] = null;
        PlayerInSlot[1] = null;
        PlayerInSlot[2] = null;
        PlayerInSlot[3] = null;
    }

    #endregion
}
