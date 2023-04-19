using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using PlayFab;
using Newtonsoft.Json;

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
    #endregion

    #region General Variables
    private int NUMBER_PLAYER_GET_EACH_TURN = 30;
    #endregion

    #region Playfab Call API
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
    public string Position;
    public int StatValue;

    public string getInf()
    {
        return DisplayName + ":" + PlayFabId;
    }
}