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
    const float MaxTimeLoading = 1;

    public void RegisterEventPrepare(Action<Action> _event)
    {
        if (isloading)
        {
            Debug.Log($"[{this.name}]: Can not register because loading process is runing");
            return;
        }
        ListAction.Add(_event);
    }
    IEnumerator CountDownTime(Action OnLoadingComplete)
    {
        yield return new WaitForSeconds(MaxTimeLoading);
        if (isloading)
            EndLoad(OnLoadingComplete);
    }

    public void StartLoading(Action OnLoadingComplete)
    {
        PopupController.ShowLoadingPopup();
        isloading = true;
        numberOfDoneTask = 0;
        maxTask = ListAction.Count;

        StartCoroutine(CountDownTime(OnLoadingComplete));
        foreach (var _event in ListAction)
        {
            _event?.Invoke(() => { IncreaseProgress(OnLoadingComplete); });
        }
    }
    private void IncreaseProgress(Action OnLoadingComplete)
    {
        numberOfDoneTask++;
        OnIncreaseProgress?.Invoke();
        if (numberOfDoneTask == maxTask && isloading)
        {
            EndLoad(OnLoadingComplete);
        }
    }

    private void EndLoad(Action OnLoadingComplete)
    {
        ListAction.Clear();
        isloading = false;
        PopupController.HideLoadingPopup();
        OnLoadingComplete?.Invoke();
    }

    private void Start()
    {

    }

}
