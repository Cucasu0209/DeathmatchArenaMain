using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class PlayerReviewController : MonoBehaviour
{
    #region Singleton
    protected PlayerReviewController() { }

    private static PlayerReviewController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static PlayerReviewController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<PlayerReviewController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static PlayerReviewController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<PlayerReviewController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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

    #region Public Method
    public PlayerReviewEntity GetPlayerInformation(string id)
    {
        if (id == PlayerData.GetId())
        {
            return new PlayerReviewEntity()
            {
                WeaponIndex = PlayerData.GetCurrentWeaponIndex(),
                HatIndex = PlayerData.GetCurrentHatIndex(),
                ShoeIndex = PlayerData.GetCurrentShoeIndex(),
            };
        }

        return new PlayerReviewEntity()
        {
            HatIndex = 3,
            ShoeIndex = 4,
            WeaponIndex = 2,
        };
    }

    #endregion
}


public class PlayerReviewEntity
{
    public int WeaponIndex;
    public int HatIndex;
    public int ShoeIndex;
}