using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using PlayFab.ClientModels;
using PlayFab;
using PlayFab.Json;

public class LoadingController : MonoBehaviour
{
    #region Singleton
    protected LoadingController() { }

    private static LoadingController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static LoadingController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<LoadingController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static LoadingController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<LoadingController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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

    private List<Action<Action>> ListAction = new List<Action<Action>>();
    public static event Action OnIncreaseProgress;
    public bool isloading = false;
    public int numberOfDoneTask;
    public int maxTask;
    public void RegisterEventPrepare(Action<Action> _event)
    {
        if (isloading)
        {
            Debug.Log($"[{this.name}]: Can not register because loading process is runing");
            return;
        }
        ListAction.Add(_event);
    }

    public void StartLoading(Action OnLoaingComplete)
    {
        LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.Loading);
        isloading = true;
        numberOfDoneTask = 0;
        maxTask = ListAction.Count;
        foreach (var _event in ListAction)
        {
            _event?.Invoke(() => { IncreaseProgress(OnLoaingComplete); });
        }
    }
    private void IncreaseProgress(Action OnLoaingComplete)
    {
        numberOfDoneTask++;
        OnIncreaseProgress?.Invoke();
        if (numberOfDoneTask == maxTask)
        {
            ListAction.Clear();
            isloading = false;
            OnLoaingComplete?.Invoke();
        }
    }

    private void Start()
    {

        RegisterEventPrepare((Action) =>
        {
            AuthenticationController.Instance.LoginDefault((a) => { Action?.Invoke(); });
        });
        RegisterEventPrepare((Action) =>
        {
            AuthenticationController.Instance.LoginDefault((a) =>
            {
                UpdateLeaderboard(() => { Action?.Invoke(); });

            });
        });
        StartLoading(() => { LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.MainMenu); });
    }

    public void UpdateLeaderboard(Action oncomplete)
    {

        // Set up the request to call the updateLeaderboard Cloud Script function
        var request = new ExecuteCloudScriptRequest
        {
            FunctionName = "updateLeaderboard",
        };

        // Call the ExecuteCloudScript API to execute the updateLeaderboard function
        PlayFabClientAPI.ExecuteCloudScript(request, (a) => { OnUpdateLeaderboardSuccess(a); oncomplete?.Invoke(); }, OnUpdateLeaderboardError);
    }

    private void OnUpdateLeaderboardSuccess(ExecuteCloudScriptResult result)
    {
        Debug.Log(result.Error.Error+" "+result.Error.StackTrace);
        Debug.Log(result.FunctionResult.ToString());
        Debug.Log("Leaderboard updated successfully!");
    }

    private void OnUpdateLeaderboardError(PlayFabError error)
    {
        Debug.LogError("Error updating leaderboard: " + error.GenerateErrorReport());
    }
}
