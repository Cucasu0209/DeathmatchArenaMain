using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using PlayFab;
using UnityEngine.SceneManagement;

public class StartNameContainerUI : MonoBehaviour
{
    public TextMeshProUGUI Notification;
    public TextFieldComponentUI NickName;

    private Dictionary<NickNameChangeResult, string> NotificationLog = new Dictionary<NickNameChangeResult, string>()
    {
        {NickNameChangeResult.Success, "Completed"},
        {NickNameChangeResult.NickNameExisted, "Nickname has already existed or invalid. Try again ! !"},
        {NickNameChangeResult.NickNameTooShort, "Your nickname is too short \" more than 5 \" !"},
        {NickNameChangeResult.NickNameTooLong, "Your nickname is too long \" less than 13 \" !"},
        {NickNameChangeResult.NotLoginYet, "You haven't logged in yet !"},
    };
    private void OnEnable()
    {
        Notification.SetText("");
        NickName.ClearString();
    }

    public void StartGame()
    {
        Notification.SetText("");
        string nickname = NickName.GetText();
        PopupController.ShowLoadingPopup();
        AuthenticationController.Instance.ChangeNickName(nickname, ActionChangeNameResult);
    }
    public void ActionChangeNameResult(NickNameChangeResult result)
    {
        Notification.SetText(NotificationLog[result]);
        PopupController.HideLoadingPopup();

        if (result == NickNameChangeResult.Success)
        {
            LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.MainMenu);
        }




    }
}
