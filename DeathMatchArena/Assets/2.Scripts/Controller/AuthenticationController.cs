using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using PlayFab;
public class AuthenticationController : MonoBehaviour
{
    #region Singleton
    protected AuthenticationController() { }

    private static AuthenticationController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static AuthenticationController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<AuthenticationController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static AuthenticationController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<AuthenticationController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
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
    private string MyPlayfabId = "";
    private string MyDisplayName = "";
    private const string atMailDotCom = "@gmail.com";
    #endregion

    #region General Variables

    #endregion

    #region Playfab Authentication
    private void LoginPlayfab(string username, string password, Action<LoginResultType> OnComplete)
    {
        PlayFabClientAPI.LoginWithEmailAddress(new PlayFab.ClientModels.LoginWithEmailAddressRequest()
        {
            Email = username + atMailDotCom,
            Password = password,
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Login {result.PlayFabId}");
            MyPlayfabId = result.PlayFabId;
            LocalClientData.SaveUsername(username);
            LocalClientData.SavePassword(password);
            OnComplete?.Invoke(LoginResultType.Success);

        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Login Error {error.ErrorMessage}");
            OnComplete?.Invoke(LoginResultType.Invalid);

        });
    }
    private void RegisterPlayfabAccountPlayfab(string username, string password, Action<RegisterResultType> OnComplete)
    {
        PlayFabClientAPI.RegisterPlayFabUser(new PlayFab.ClientModels.RegisterPlayFabUserRequest()
        {
            Email = username + atMailDotCom,
            Password = password,
            Username = username,
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:Register {result.PlayFabId}");
            OnComplete?.Invoke(RegisterResultType.Success);

        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Register error {error.ErrorMessage}, {error.ErrorDetails}, {error.Error}");
            OnComplete?.Invoke(RegisterResultType.UsernameExisted);

        });
    }
    private void ChangeDisplayNamePlayfab(string newDisplayName, Action<NickNameChangeResult> OnComplete)
    {
        if (IsClientLoggedIn() == false)
        {
            Debug.Log($"[{this.name}]:ChangeName error {NickNameChangeResult.NotLoginYet}");
            OnComplete?.Invoke(NickNameChangeResult.NotLoginYet);

            return;
        }
        PlayFabClientAPI.UpdateUserTitleDisplayName(new PlayFab.ClientModels.UpdateUserTitleDisplayNameRequest()
        {
            DisplayName = newDisplayName
        },
        (result) =>
        {
            Debug.Log($"[{this.name}]:ChangeName {result.DisplayName}");
            OnComplete?.Invoke(NickNameChangeResult.Success);

        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Changname error {error.ErrorMessage}, {error.ErrorDetails}, {error.Error}");
            OnComplete?.Invoke(NickNameChangeResult.NickNameExisted);

        }); ;
    }
    private void GetDisplayNamePlayfab(Action<GetNickNameResult, string> OnComplete)
    {
        if (IsClientLoggedIn() == false)
        {
            Debug.Log($"[{this.name}]:Get NickName error {GetNickNameResult.NotLoginYet}");
            OnComplete?.Invoke(GetNickNameResult.NotLoginYet, "");

            return;
        }

        PlayFabClientAPI.GetAccountInfo(new PlayFab.ClientModels.GetAccountInfoRequest()
        {
            PlayFabId = MyPlayfabId,
        },
        (result) =>
        {
            MyDisplayName = result.AccountInfo.TitleInfo.DisplayName;
            if (string.IsNullOrEmpty(MyDisplayName) == false)
            {
                Debug.Log($"[{this.name}]:Get NickName {result.AccountInfo.TitleInfo.DisplayName}");
                OnComplete?.Invoke(GetNickNameResult.Success, MyDisplayName);

            }
            else
            {
                Debug.Log($"[{this.name}]:Get NickName error {result.AccountInfo.TitleInfo.DisplayName}");
                OnComplete?.Invoke(GetNickNameResult.NotExist, MyDisplayName);

            }
        },
        (error) =>
        {
            Debug.Log($"[{this.name}]:Get NickName error {error.ErrorMessage}");
            OnComplete?.Invoke(GetNickNameResult.NotExist, MyDisplayName);

        });
    }
    private bool IsClientLoggedIn()
    {
        return PlayFabClientAPI.IsClientLoggedIn() && string.IsNullOrEmpty(MyPlayfabId) == false;
    }
    #endregion

    #region Action
    public void Login(string username, string password, Action<LoginResultType> OnComplete)
    {
        if (IsAlphabetNumber(username) == false
            || IsStartWithAlphabet(username) == false)
        {
            OnComplete?.Invoke(LoginResultType.IncorrectFormatUsername);
            return;
        }
        if (IsAlphabetNumber(password) == false)
        {
            OnComplete?.Invoke(LoginResultType.IncorrectFormatPassword);
            return;
        }
        LoginPlayfab(username, password, OnComplete);

    }
    public void Register(string username, string password, string password2, Action<RegisterResultType> OnComplete)
    {
        if (IsAlphabetNumber(username) == false
            || IsStartWithAlphabet(username) == false
           )
        {
            OnComplete?.Invoke(RegisterResultType.IncorrectFormatUsername);
            return;
        }
        if (password != password2)
        {
            OnComplete?.Invoke(RegisterResultType.TwoPasswordNotSame);
            return;
        }
        if (IsAlphabetNumber(password) == false
           )
        {
            OnComplete?.Invoke(RegisterResultType.IncorrectFormatPassword);
            return;
        }
        if (IsEnoughLength(password) == false || IsEnoughLength(username) == false)
        {
            OnComplete?.Invoke(RegisterResultType.IsnotLongEnough);
            return;
        }
        RegisterPlayfabAccountPlayfab(username, password, OnComplete);
    }
    public void ChangeNickName(string newDisplayName, Action<NickNameChangeResult> Oncomplete)
    {
        if (newDisplayName.Length <= 6)
        {
            Oncomplete?.Invoke(NickNameChangeResult.NickNameTooShort);
            return;
        }
        ChangeDisplayNamePlayfab(newDisplayName, Oncomplete);
    }
    public void GetNickName(Action<GetNickNameResult, string> OnComplete)
    {
        GetDisplayNamePlayfab(OnComplete);
    }
    #endregion

    #region Validate
    private bool IsAlphabetNumber(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }



        for (int i = 1; i < input.Length; i++)
        {
            if (char.IsLetterOrDigit(input[i]) == false && input[i] != '@')
            {
                return false;
            }
        }

        return true;
    }
    private bool IsStartWithAlphabet(string input)
    {
        if (!char.IsLetter(input[0]))
        {
            return false;
        }
        return true;
    }
    private bool IsEnoughLength(string input)
    {
        return input.Length >= 8;
    }
    #endregion
}

public enum LoginResultType
{
    Success,
    Invalid,
    IncorrectFormatUsername,
    IncorrectFormatPassword,
}

public enum RegisterResultType
{
    Success,
    UsernameExisted,
    IncorrectFormatUsername,
    IncorrectFormatPassword,
    TwoPasswordNotSame,
    IsnotLongEnough
}

public enum NickNameChangeResult
{
    Success,
    NickNameExisted,
    NickNameTooShort,
    NotLoginYet,
}

public enum GetNickNameResult
{
    Success,
    NotExist,
    NotLoginYet,
}