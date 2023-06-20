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
    public GamePlayResult currentGameResult = new GamePlayResult();
    public const string defaultEmptyName = "???";
    #endregion

    #region Unity
    private void OnEnable()
    {

        NetworkController_PUN.ActionOnPlayerListChanged += GetPlayers;
        NetworkController_PUN.ActionOnLeftRoom += ResetSlot;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateGameResult;
        GetPlayers();
    }
    private void OnDisable()
    {
        NetworkController_PUN.ActionOnPlayerListChanged -= GetPlayers;
        NetworkController_PUN.ActionOnLeftRoom -= ResetSlot;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateGameResult;
    }
    private void Update()
    {
        //Debug.Log($"[{this.name}]: Slot 0: {PlayerInSlot[0] != null}," +
        //    $"1: {PlayerInSlot[1] != null}, " +
        //    $"2: {PlayerInSlot[2] != null}, " +
        //    $"3: {PlayerInSlot[3] != null}");
    }
    #endregion

    #region Public Actions
    public bool GetIsMaster(Player player)
    {
        return player.IsMasterClient;
    }
    public bool IsEveryOneReady()
    {
        if (PlayerInSlot[1] == null && PlayerInSlot[0] == null) return false;
        if (PlayerInSlot[2] == null && PlayerInSlot[3] == null) return false;
        foreach (Player p in PlayerInSlot.Values)
        {
            if (p != null)
            {
                if (NetworkController_PUN.Instance.GetPlayerProperties(p).isReady == false) return false;
            }
        }
        return true;
    }

    public int GetTeam(Player player)
    {
        int result = NetworkController_PUN.Instance.GetPlayerProperties(player).slotInRoom;
        int team = result <= 1 ? 1 : 2;
        return team;
    }
    public GamePlayResultEnum GetGameResult()
    {
        return currentGameResult.gameResult;
    }
    public string GetNamePlayer(int PlayerIndex)
    {
        if (PlayerInSlot.ContainsKey(PlayerIndex)) return defaultEmptyName;

        return PlayerInSlot[PlayerIndex] != null ?
                NetworkController_PUN.Instance.GetPlayerProperties(RoomController.Instance.PlayerInSlot[PlayerIndex]).playerName : defaultEmptyName;
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
    private void UpdateGameResult()
    {

        currentGameResult = new GamePlayResult();

        currentGameResult.gameResult = CheckResult();
        currentGameResult.player1Reward = new PlayerReward()
        {
            owner = PlayerInSlot[0],
            CoinReward = 0,
            EloReward = 0
        };

        currentGameResult.player2Reward = new PlayerReward()
        {
            owner = PlayerInSlot[1],
            CoinReward = 0,
            EloReward = 0
        };

        currentGameResult.player3Reward = new PlayerReward()
        {
            owner = PlayerInSlot[2],
            CoinReward = 0,
            EloReward = 0
        };

        currentGameResult.player4Reward = new PlayerReward()
        {
            owner = PlayerInSlot[3],
            CoinReward = 0,
            EloReward = 0
        };
    }

    private GamePlayResultEnum CheckResult()
    {

        if (RoomController.Instance.PlayerInSlot[0] == null && RoomController.Instance.PlayerInSlot[1] == null) return GamePlayResultEnum.NotCompleteYet;
        if (RoomController.Instance.PlayerInSlot[2] == null && RoomController.Instance.PlayerInSlot[3] == null) return GamePlayResultEnum.NotCompleteYet;

        bool[] isALive = new bool[4] { false, false, false, false };
        for (int i = 0; i < RoomController.Instance.PlayerInSlot.Count; i++)
        {
            if (RoomController.Instance.PlayerInSlot[i] != null)
            {
                isALive[i] = NetworkController_PUN.Instance.GetPlayerProperties(RoomController.Instance.PlayerInSlot[i]).playerHealth > 0;
            }
        }
        if ((isALive[0] || isALive[1] || isALive[2] || isALive[3]) == false)
        {
            return GamePlayResultEnum.Draw;
        }
        else if ((isALive[0] || isALive[1]) == false)
        {
            return GamePlayResultEnum.Team2Win;
        }
        else if ((isALive[2] || isALive[3]) == false)
        {
            return GamePlayResultEnum.Team1Win;
        }
        return GamePlayResultEnum.NotCompleteYet;

    }
    #endregion
}

public class GamePlayResult
{
    public GamePlayResultEnum gameResult;
    public PlayerReward player1Reward;
    public PlayerReward player2Reward;
    public PlayerReward player3Reward;
    public PlayerReward player4Reward;

    public GamePlayResult()
    {
        gameResult = GamePlayResultEnum.Draw;
        player1Reward = new PlayerReward();
        player2Reward = new PlayerReward();
        player3Reward = new PlayerReward();
        player4Reward = new PlayerReward();
    }
}
public class PlayerReward
{
    public Player owner;
    public float EloReward;
    public float CoinReward;

    public PlayerReward()
    {
        EloReward = 0;
        CoinReward = 0;
    }
}