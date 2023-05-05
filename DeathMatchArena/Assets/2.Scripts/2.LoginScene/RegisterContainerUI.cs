using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Doozy;
using TMPro;
using Doozy.Runtime.UIManager.Containers;

public class RegisterContainerUI : MonoBehaviour
{
    public TextMeshProUGUI Notification;
    public TextFieldComponentUI Username;
    public TextFieldComponentUI Password;
    public TextFieldComponentUI Password2;

    private Dictionary<RegisterResultType, string> NotificationLog = new Dictionary<RegisterResultType, string>()
    {
        {RegisterResultType.Success, "Register Success."},
        {RegisterResultType.UsernameExisted, "Username has already existed. Try again !"},
        {RegisterResultType.IncorrectFormatUsername, "Username incorrect format. Try again !"},
        {RegisterResultType.IncorrectFormatPassword, "Password incorrect format. Try again !"},
        {RegisterResultType.TwoPasswordNotSame, "Two password are not the same. Try again !"},
        {RegisterResultType.IsnotLongEnough, "Username and password both have more than 8 words. Try again !"},
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
        PopupController.ShowLoadingPopup();
        AuthenticationController.Instance.Register(Username.GetText(), Password.GetText(), Password2.GetText(), ActionRegisterResult);
    }

    public void ActionRegisterResult(RegisterResultType result)
    {
        Notification.SetText(NotificationLog[result]);
        if (result == RegisterResultType.Success)
        {
            PopupController.ShowLoginAfterRegisterPopupPopup(Username.GetText(), Password.GetText());
        }
        PopupController.HideLoadingPopup();
    }
}
