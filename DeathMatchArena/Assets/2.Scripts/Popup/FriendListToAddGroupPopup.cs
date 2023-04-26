using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Popup.Container.FriendListToAddGroup;

public class FriendListToAddGroupPopup : BasePopup
{
    public static FriendListToAddGroupPopup Instance;
    public TextMeshProUGUI GroupName;
    public FriendListToAddGroupAdapter listFriendAdapter;

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
        GroupName.SetText(group.GroupName);
        StartCoroutine(ShowListOSA(group));
    }
    private IEnumerator ShowListOSA(GroupChatInfomation group)
    {
        List<MyListItemModel> items = new List<MyListItemModel>();
        foreach (var friend in FriendController.Instance.GetTempFriend())
        {
            string _friend = friend;
            bool isMember = false;
            bool isInvited = false;
            foreach (var member in group.members)
            {
                if (member.PlayfabId == _friend)
                {
                    isMember = true;
                }
            }
            if (group.GroupInvitations != null)
            {
                foreach (var invitation in group.GroupInvitations)
                {
                    if (invitation.MasterInvitedEntity.Id == _friend)
                    {
                        isInvited = true;
                    }
                }
            }


            items.Add(new MyListItemModel()
            {
                displayName = OtherPlayersController.Instance.GetTempAllPlayers()[_friend].DisplayName,
                id = _friend,
                isInvited = isInvited,
                isMember = isMember,
                OnClick = () => { ChatController.Instance.InviteMembersToGroupChat(group, _friend); }
            });
        }
        yield return new WaitForSeconds(0.1f);
        listFriendAdapter.SetItems(items);
    }
    private void HideList()
    {
        listFriendAdapter.SetItems(new List<MyListItemModel>());
    }
    public void ClickClose()
    {
        PopupController.HideFriendListToAddGroupPopup();
    }
}
