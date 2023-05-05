using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lobby.Container.RoomList;

public class LobbyUI : MonoBehaviour
{
    #region Variables
    public PlayerDetailUI MyProfie;
    public RoomListAdapter roomList;
    #endregion

    #region Unity
    private void Start()
    {
        if (AuthenticationController.Instance.IsLogin() == false)
        {
            PopupController.ShowLoadingPopup();
            AuthenticationController.Instance.LoginDefault((result) =>
            {
                PopupController.HideLoadingPopup();
            });
        }

        MyProfie.DisplayInfomation(PlayerPlayfabInformation.GetMyInfomation());
    }
    private void OnEnable()
    {
        NetworkController_PUN.ActionOnRoomListUpdate += UpdateRoomList;

        NetworkController_PUN.Instance.ConnectPUN(null);
        UpdateRoomList();
    }
    private void OnDisable()
    {
        NetworkController_PUN.ActionOnRoomListUpdate -= UpdateRoomList;
    }
    #endregion

    #region Button Callbacks
    public void OnBackToMenuClick()
    {
        LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.MainMenu);
    }
    #endregion

    #region Actions 
    public void UpdateRoomList()
    {
        StartCoroutine(IEUpdateRoomList());
    }
    private IEnumerator IEUpdateRoomList()
    {
        List<MyListItemModel> myListItems = new List<MyListItemModel>();
        foreach (var room in NetworkController_PUN.Instance.cachedRoomList.Values)
        {
            string roomname = room.Name;
            myListItems.Add(new MyListItemModel()
            {
                currentNumberPlayer = (byte)room.PlayerCount,
                MaxNumberPlayer = room.MaxPlayers,
                RoomName = room.Name,
                OnJoinClick = ()=>
                {
                    LobbyController.Instance.JoinRoom(roomname);
                },

            });
        }
        yield return new WaitForSeconds(0.05f);
        roomList.SetItems(myListItems);
    }
    public void OnCreateRoomClick()
    {
        PopupController.ShowCreateRoomFormPopup();
    }
    public void OnJoinRandomRoomClick()
    {
        LobbyController.Instance.JoinRandomRoom();
    }
    #endregion
}
