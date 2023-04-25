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
            Debug.Log("trunge " + FriendController.Instance.GetTempFriend().Count);
            string _friend = friend;
            bool isadded = false;
            foreach (var member in group.members)
            {
                if (member.playfabID == _friend)
                {
                    isadded = true;
                }
            }
            items.Add(new MyListItemModel()
            {
                displayName = OtherPlayersController.Instance.GetTempAllPlayers()[_friend].DisplayName,
                id = _friend,
                isAdded = isadded,
                OnClick = () => { ChatController.Instance.AddMembersToGroupChat(group, _friend);  }
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
