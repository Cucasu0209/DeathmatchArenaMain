using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneSmoothController : MonoBehaviour
{
    #region Singleton
    protected LoadSceneSmoothController() { }

    private static LoadSceneSmoothController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static LoadSceneSmoothController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<LoadSceneSmoothController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static LoadSceneSmoothController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<LoadSceneSmoothController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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

    #region Link
    private static readonly string LoadSceneSmoothEntityLink = "Other/LoadSceneSmoothEntity";

    #endregion

    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            LoadSceneSmoothEntity.Instance?.Hide(null);
        };
    }
    public void LoadScene(SceneEnum.Type scene)
    {
        if (LoadSceneSmoothEntity.Instance == null)
        {
            GameObject myEffect = Resources.Load<GameObject>(LoadSceneSmoothEntityLink);
            if (myEffect != null)
            {

                GameObject newEffect = GameObject.Instantiate(myEffect, null);
                newEffect.GetComponent<LoadSceneSmoothEntity>()?.Show(() =>
                {
                    SceneManager.LoadScene(SceneEnum.GetSceneString(scene));
                });
            }
        }
        else
        {
            LoadSceneSmoothEntity.Instance.Show(() =>
            {
                SceneManager.LoadScene(SceneEnum.GetSceneString(scene));
            });
        }

    }

}
