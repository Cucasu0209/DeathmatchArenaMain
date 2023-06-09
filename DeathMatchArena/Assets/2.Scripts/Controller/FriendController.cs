using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class FriendController : MonoBehaviour
{
    #region Singleton
    protected FriendController() { }

    private static FriendController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static FriendController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<FriendController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static FriendController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<FriendController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    private List<string> _tempFriend = new List<string>();
    #endregion

    #region Actions
    public bool IsFriend(string PlayfabId)
    {
        return _tempFriend.Contains(PlayfabId);
    }
    public void GetFriends(Action OnComplete)
    {
        PlayfabController.Instance.GetAllFriendPlayfab((friends) =>
        {
            foreach (var friend in friends)
            {
                OtherPlayersController.Instance.AddTempPlayer(friend);
                if (_tempFriend.Contains(friend.PlayFabId) == false) _tempFriend.Add(friend.PlayFabId);
            }
            OnComplete?.Invoke();
        });
    }
    public List<string> GetTempFriend()
    {
        return _tempFriend;
    }
    public void SaveNewFriend(string PlayfabId)
    {
        if (_tempFriend.Contains(PlayfabId) == false) _tempFriend.Add(PlayfabId);
        PlayfabController.Instance.AddNewFriendPlayfab(PlayfabId, null);
    }

    public void SaveRemoveFriend(string PlayfabId)
    {
        if (_tempFriend.Contains(PlayfabId) == true) _tempFriend.Remove(PlayfabId);
        PlayfabController.Instance.RemoveFriendPlayfab(PlayfabId, null);
    }
    #endregion
}
