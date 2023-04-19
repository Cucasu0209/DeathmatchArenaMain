using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
public class OtherPlayersController : MonoBehaviour
{
    #region Singleton
    protected OtherPlayersController() { }

    private static OtherPlayersController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static OtherPlayersController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<OtherPlayersController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static OtherPlayersController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<OtherPlayersController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    private Dictionary<string, PlayerPlayfabInformation> _tempOtherPlayer= new Dictionary<string, PlayerPlayfabInformation>();
    #endregion


    #region Actions
    public void GetAllPlayers(int index,Action OnComplete)
    {
        PlayfabController.Instance.GetAllPlayersPlayfab(index, (players) =>
        {
            foreach(var player in players)
            {
                if (_tempOtherPlayer.ContainsKey(player.PlayFabId)) _tempOtherPlayer[player.PlayFabId] = player;
                else _tempOtherPlayer.Add(player.PlayFabId, player);
            }
            OnComplete?.Invoke();
        });
    }
    #endregion
}
