using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class LoginAfterRegisterPopup : BasePopup
{
    public static LoginAfterRegisterPopup Instance;
    private string username, password;
    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }
    public void ClickYes()
    {

        PopupController.ShowLoadingPopup();
        AuthenticationController.Instance.Login(username, password, ActionLoginResult);
        PopupController.HideLoginAfterRegisterPopupPopup();
    }
    public void ClickNo()
    {
        PopupController.HideLoginAfterRegisterPopupPopup();
    }
    public void SetInformation(string _username, string _password)
    {
        this.username = _username;
        this.password = _password;
    }
    public void ActionLoginResult(LoginResultType result)
    {
        if (result == LoginResultType.Success)
        {
            AuthenticationController.Instance.GetNickName((result, nickname) =>
            {
                PopupController.HideLoadingPopup();
                if (result == GetNickNameResult.NotExist)
                {
                    LoginSceneController.Instance?.ShowStartNameContainer();
                }
                else
                {
                    LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.MainMenu);
                }
            });
        }
        else
        {
            PopupController.HideLoadingPopup();
        }

    }
}
