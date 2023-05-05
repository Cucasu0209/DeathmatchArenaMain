using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class LoginContainerUI : MonoBehaviour
{
    public TextMeshProUGUI Notification;
    public TextFieldComponentUI Username;
    public TextFieldComponentUI Password;

    private Dictionary<LoginResultType, string> NotificationLog = new Dictionary<LoginResultType, string>()
    {
        {LoginResultType.Success, "Login Success."},
        {LoginResultType.Invalid, "Username or password incorrect. Register Now !"},
        {LoginResultType.IncorrectFormatUsername, "Username incorrect format. Try again !"},
        {LoginResultType.IncorrectFormatPassword, "Password incorrect format. Try again !"},
    };
    private void Start()
    {
        Username.SetText(LocalClientData.LoadUsername());
        Password.SetText(LocalClientData.LoadPassword());
    }
    private void OnEnable()
    {
        Notification.SetText("");
    }
    private void OnDisable()
    {

    }
    public void Login()
    {
        Notification.SetText("");
        PopupController.ShowLoadingPopup();
        AuthenticationController.Instance.Login(Username.GetText(), Password.GetText(), ActionLoginResult);

    }

    public void ActionLoginResult(LoginResultType result)
    {
        Notification.SetText(NotificationLog[result]);
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
