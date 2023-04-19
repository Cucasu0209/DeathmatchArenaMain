using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RenamePopup : BasePopup
{
    public static RenamePopup Instance;
    public TextMeshProUGUI Notification;
    public AuthenticationComponentUI NickName;

    private Dictionary<NickNameChangeResult, string> NotificationLog = new Dictionary<NickNameChangeResult, string>()
    {
        {NickNameChangeResult.Success, "Completed"},
        {NickNameChangeResult.NickNameExisted, "Nickname has already existed or invalid. Try again ! !"},
        {NickNameChangeResult.NickNameTooShort, "Your nickname is too short \" more than 5 \" !"},
        {NickNameChangeResult.NickNameTooLong, "Your nickname is too long \" less than 13 \" !"},
        {NickNameChangeResult.NotLoginYet, "You haven't logged in yet !"},
    };

    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }

    private void OnEnable()
    {
        Notification.SetText("");
        NickName.SetText(PlayerData.GetNickName());
    }

    public void ClickOk()
    {
        Notification.SetText("");
        string nickname = NickName.GetText();
        if (MainMenuSceneController.Instance != null)
            PopupController.ShowLoadingPopup();
        AuthenticationController.Instance.ChangeNickName(nickname, ActionChangeNameResult);
    }
    public void ClickCancel()
    {
        PopupController.HideRenamePopup();
    }
    public void ActionChangeNameResult(NickNameChangeResult result)
    {
        Notification.SetText(NotificationLog[result]);
        PopupController.HideLoadingPopup();
        if (result == NickNameChangeResult.Success) ClickCancel();
    }
}
