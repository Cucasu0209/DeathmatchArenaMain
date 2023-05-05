using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class LobbyController : MonoBehaviour
{
    #region Singleton
    protected LobbyController() { }

    private static LobbyController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static LobbyController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<LobbyController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static LobbyController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<LobbyController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    #endregion

    #region Unity
    private void OnEnable()
    {
        NetworkController_PUN.ActionOnJoinedRoom += OnJoinedRoom;
        NetworkController_PUN.ActionOnLeftRoom += OnLeftRoom;
        NetworkController_PUN.ActionOnCreateRoomFailed += OnCreateRoomFailed;
        NetworkController_PUN.ActionOnJoinRandomFailed += OnJoinRandomFailed;
        NetworkController_PUN.ActionOnJoinRoomFailed += OnJoinRoomFailed;
    }
    private void OnDisable()
    {
        NetworkController_PUN.ActionOnJoinedRoom -= OnJoinedRoom;
        NetworkController_PUN.ActionOnLeftRoom -= OnLeftRoom;
        NetworkController_PUN.ActionOnCreateRoomFailed -= OnCreateRoomFailed;
        NetworkController_PUN.ActionOnJoinRandomFailed -= OnJoinRandomFailed;
        NetworkController_PUN.ActionOnJoinRoomFailed -= OnJoinRoomFailed;
    }
    #endregion

    #region Public Actions
    public void JoinRoom(string roomName)
    {
        NetworkController_PUN.Instance.JoinRoom(roomName);
        PopupController.ShowLoadingPopup();
    }
    public void LeaveRoom()
    {
        NetworkController_PUN.Instance.LeaveRoom();
        PopupController.ShowLoadingPopup();
    }
    public void CreateRoom(string roomName)
    {
        NetworkController_PUN.Instance.CreateRoom(roomName);
        PopupController.ShowLoadingPopup();
    }
    public void JoinRandomRoom()
    {
        NetworkController_PUN.Instance.JoinRandomRoom();
        PopupController.ShowLoadingPopup();
    }
    public string GetRoomName()
    {
        return NetworkController_PUN.Instance.GetRoomName();
    }
    #endregion

    #region Private Actions
    private void OnJoinedRoom()
    {
        LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.Room);
        PopupController.HideLoadingPopup();
    }
    private void OnLeftRoom()
    {
        LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.Lobby);
        PopupController.HideLoadingPopup();
    }
    private void OnCreateRoomFailed()
    {
        PopupController.HideLoadingPopup();
    }
    private void OnJoinRandomFailed()
    {
        PopupController.ShowYesNoPopup("NO ROOM AVAILABLE.\n Would you like to create a new one?", () =>
        {
            PopupController.ShowCreateRoomFormPopup();
        }, null);
        PopupController.HideLoadingPopup();
    }
    private void OnJoinRoomFailed()
    {
        PopupController.HideLoadingPopup();
    }
    #endregion
}
