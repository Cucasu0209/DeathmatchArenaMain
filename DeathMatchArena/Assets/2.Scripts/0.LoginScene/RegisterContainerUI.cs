using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy;
using TMPro;
using Doozy.Runtime.UIManager.Containers;

public class RegisterContainerUI : MonoBehaviour
{
    public TextMeshProUGUI Notification;
    public AuthenticationComponentUI Username;
    public AuthenticationComponentUI Password;
    public AuthenticationComponentUI Password2;

    private Dictionary<RegisterResultType, string> NotificationLog = new Dictionary<RegisterResultType, string>()
    {
        {RegisterResultType.Success, "Register Success."},
        {RegisterResultType.UsernameExisted, "Username existed !"},
        {RegisterResultType.IncorrectFormatUsername, "Username incorrect format !"},
        {RegisterResultType.IncorrectFormatPassword, "Password incorrect format !"},
        {RegisterResultType.TwoPasswordNotSame, "Two password are not the same !"},
        {RegisterResultType.IsnotLongEnough, "Username and password both have more than 8 words !"},
    };
    private void OnEnable()
    {
        Notification.SetText("");
        Username.ClearString();
        Password.ClearString();
        Password2.ClearString();
    }
    private void OnDisable()
    {

    }
    public void Register()
    {
        Notification.SetText("");
        PopupController.ShowLoadingPopup(LoginSceneController.Instance.MainCanvas);
        AuthenticationController.Instance.Register(Username.GetText(), Password.GetText(),Password2.GetText(), ActionRegisterResult);
    }

    public void ActionRegisterResult(RegisterResultType result)
    {
        Notification.SetText(NotificationLog[result]);
        PopupController.HideLoadingPopup();
    }
}
