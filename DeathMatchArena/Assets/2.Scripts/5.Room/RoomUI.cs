using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RoomUI : MonoBehaviour
{
    public TextMeshProUGUI RoomName;

    private void Start()
    {
        RoomName.SetText("Room: " + LobbyController.Instance.GetRoomName());
    }
    public void LeaveRoom()
    {
        PopupController.ShowYesNoPopup("Are you sure to leave room?",
            () => LobbyController.Instance.LeaveRoom()
            , null);

    }
}
