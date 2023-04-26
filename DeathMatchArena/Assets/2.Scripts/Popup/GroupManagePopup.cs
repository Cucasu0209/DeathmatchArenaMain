using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Popup.Container.GroupManager;

public class GroupManagePopup : BasePopup
{
    public static GroupManagePopup Instance;
    GroupChatInfomation inf;
    public TextMeshProUGUI GroupName;
    public TextMeshProUGUI GroupMemberCount;
    public GroupManageListAdapter listMemberAdapter;
    public GameObject DeleteBtn;

    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }

    private void OnEnable()
    {
        GroupName.SetText("");
    }
    private void OnDisable()
    {
        HideList();
    }
    public void ShowList(GroupChatInfomation group)
    {
        inf = group;
        DeleteBtn.SetActive(false);
        if (ChatController.Instance.isAdmin(PlayerData.GetId(), group))
            DeleteBtn.SetActive(true);

        GroupName.SetText(group.GroupName);
        GroupMemberCount.SetText(group.members.Count + " members");
        StartCoroutine(ShowListOSA(group));
    }
    private IEnumerator ShowListOSA(GroupChatInfomation group)
    {
        List<MyListItemModel> items = new List<MyListItemModel>();
        bool AmIAdmin = ChatController.Instance.isAdmin(PlayerData.GetId(), group);

        foreach (var member in group.members)
        {
            items.Add(new MyListItemModel()
            {
                OnClick = () =>
                {
                    string notify = $"Are you sure to kick member {member.DisplayName} out of the group?";
                    PopupController.ShowYesNoPopup(notify,
                    () =>
                    {
                        ChatController.Instance.RemoveMember(member.titleId, (newGroup) =>
                        {
                            ShowList(newGroup);
                            ChatUI.Instance?.RefreshInf();
                        });
                    },
                    () =>
                    {

                    });
                },
                amIAdmin = AmIAdmin,
                displayName = member.DisplayName,
                id = member.PlayfabId,
                isPlayermember = member.playerRoleId == PlayfabController.Instance.GroupRoleMember,
            });

        }
        yield return new WaitForSeconds(0.1f);
        listMemberAdapter.SetItems(items);
    }
    private void HideList()
    {
        listMemberAdapter.SetItems(new List<MyListItemModel>());
    }
    public void DeleteGroupClick()
    {
        string notify = $"Are you sure to remove group {inf.GroupName}?";
        PopupController.ShowYesNoPopup(notify,
        () =>
        {
            ChatController.Instance.DeleteGroup( () =>
            {
                PopupController.HideGroupManagePopup();
                ChatUI.Instance?.RefreshInf();
                ChatUI.Instance?.UpdateList();
            });
        },
        () =>
        {

        });
       
    }
    public void ClickClose()
    {
        PopupController.HideGroupManagePopup();
    }
}
