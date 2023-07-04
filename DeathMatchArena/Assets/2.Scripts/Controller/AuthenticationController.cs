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
    private string entityId = "";
    private string entityType = "";
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
            entityId = result.EntityToken.Entity.Id;
            entityType = result.EntityToken.Entity.Type;



            LocalClientData.SaveUsername(username);
            LocalClientData.SavePassword(password);
            PlayerData.SetId(MyPlayfabId);
            LoadDataAfterLogin(() => OnComplete?.Invoke(LoginResultType.Success));


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
            PlayerData.SetNickName(newDisplayName);
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
                PlayerData.SetNickName(MyDisplayName);
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
    private void LogoutPlayfab()
    {
        PlayFabClientAPI.ForgetAllCredentials();
    }
    private void LoginDefaultPlayfab(Action<LoginResultType> OnComplete)
    {
        Login(LocalClientData.LoadUsername(), LocalClientData.LoadPassword(), OnComplete);
    }
    private void LoadDataAfterLogin(Action OnComplete)
    {
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.ActionImediatelyAfterLogin(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetEquipPlayfab(ItemType.Weapon, PlayerData.GetId(), (a, b, c) => oncomplete?.Invoke()));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetEquipPlayfab(ItemType.Hat, PlayerData.GetId(), (a, b, c) => oncomplete?.Invoke()));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetEquipPlayfab(ItemType.Shoe, PlayerData.GetId(), (a, b, c) => oncomplete?.Invoke()));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetItemOwned(ItemType.Weapon, (a, b) => oncomplete?.Invoke()));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetItemOwned(ItemType.Hat, (a, b) => oncomplete?.Invoke()));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetItemOwned(ItemType.Shoe, (a, b) => oncomplete?.Invoke()));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetCurrencyPlayfab((a) => oncomplete?.Invoke()));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => OtherPlayersController.Instance.GetAllPlayers(0, oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => OtherPlayersController.Instance.GetAllPlayers(30, oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => OtherPlayersController.Instance.GetRequestAddfriend(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => OtherPlayersController.Instance.GetInvitationfriend(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => FriendController.Instance.GetFriends(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => NetworkController_Chat.Instance.Connect(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => NetworkController_PUN.Instance.ConnectPUN(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => ChatController.Instance.GetAllFriendChatMessage(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => ChatController.Instance.GetAllGroupChat(oncomplete));
        LoadingController.Instance.RegisterEventPrepare((oncomplete) => ChatController.Instance.GetMyMembershipOpportunities(oncomplete));

        //LoadingController.Instance.RegisterEventPrepare((oncomplete) => PlayfabController.Instance.GetSubscribedGroupChat(oncomplete));

        LoadingController.Instance.StartLoading(OnComplete);
    }
    #endregion

    #region Action
    public PlayFab.DataModels.EntityKey GetEntityKey()
    {
        Debug.Log("My Type " + entityType);
        return new PlayFab.DataModels.EntityKey() { Id = entityId, Type = entityType };
    }
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
        if (newDisplayName.Length < 6)
        {
            Oncomplete?.Invoke(NickNameChangeResult.NickNameTooShort);
            return;
        }
        if (newDisplayName.Length > 12)
        {
            Oncomplete?.Invoke(NickNameChangeResult.NickNameTooLong);
            return;
        }
        ChangeDisplayNamePlayfab(newDisplayName, Oncomplete);
    }
    public void GetNickName(Action<GetNickNameResult, string> OnComplete)
    {
        GetDisplayNamePlayfab(OnComplete);
    }
    public void Logout()
    {
        LogoutPlayfab();
    }
    public void LoginDefault(Action<LoginResultType> OnComplete)
    {
        if (IsClientLoggedIn())
        {
            OnComplete?.Invoke(LoginResultType.Success);
            return;
        }
        LoginDefaultPlayfab((result) =>
        {
            GetNickName((_result, _name) =>
            {
                OnComplete?.Invoke(result);
            });
        });
    }
    public bool IsLogin()
    {
        return IsClientLoggedIn();
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
    NickNameTooLong,
    NotLoginYet,
}

public enum GetNickNameResult
{
    Success,
    NotExist,
    NotLoginYet,
}
