using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using System;

public class LoginContainerUI : MonoBehaviour
{
    public TextMeshProUGUI Notification;
    public AuthenticationComponentUI Username;
    public AuthenticationComponentUI Password;

    private Dictionary<LoginResultType, string> NotificationLog = new Dictionary<LoginResultType, string>()
    {
        {LoginResultType.Success, "Login Success."},
        {LoginResultType.Invalid, "Username or password incorrect !"},
        {LoginResultType.IncorrectFormatUsername, "Username incorrect format !"},
        {LoginResultType.IncorrectFormatPassword, "Password incorrect format !"},
    };
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
        PopupController.ShowLoadingPopup(LoginSceneController.Instance.MainCanvas);
        AuthenticationController.Instance.Login(Username.GetText(), Password.GetText(), ActionLoginResult);

    }

    public void ActionLoginResult(LoginResultType result)
    {
        Notification.SetText(NotificationLog[result]);
        PopupController.HideLoadingPopup();
    }

}
