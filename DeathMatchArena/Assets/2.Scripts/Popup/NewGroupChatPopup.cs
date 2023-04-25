using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewGroupChatPopup : BasePopup
{
    public static NewGroupChatPopup Instance;
    public TextMeshProUGUI Notification;
    public AuthenticationComponentUI GroupsName;

    private Dictionary<GroupCreateResultEnum, string> NotificationLog = new Dictionary<GroupCreateResultEnum, string>()
    {
        {GroupCreateResultEnum.Success, "Completed."},
        {GroupCreateResultEnum.Fail, "Invalid Group Name !!"},

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
        GroupsName.SetText("Groups" + Random.Range(1, 1000000));
    }

    public void ClickOk()
    {
        Notification.SetText("");
        string groupsName = GroupsName.GetText();
        if (MainMenuSceneController.Instance != null)
            PopupController.ShowLoadingPopup();

        ChatController.Instance.CreateNewGroupChat(groupsName, ActionChangeNameResult);
    }
    public void ClickCancel()
    {
        PopupController.HideNewGroupChatPopup();
    }
    public void ActionChangeNameResult(GroupCreateResultEnum result, GroupChatInfomation group)
    {
        Notification.SetText(NotificationLog[result]);
        PopupController.HideLoadingPopup();
        if (result == GroupCreateResultEnum.Success)
        {
            ClickCancel();
            PopupController.ShowFriendListToAddGroupPopup(group);
        }
    }
}
