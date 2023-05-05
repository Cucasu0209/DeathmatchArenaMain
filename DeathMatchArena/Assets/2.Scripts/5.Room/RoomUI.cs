using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class RoomUI : MonoBehaviour
{
    public TextMeshProUGUI RoomName;
    public RoomSlotGroup SlotGroup;

    private void Start()
    {
        RoomController.Instance.InitInstance();
        DisplayRoomSlots();
        RoomName.SetText("Room: " + LobbyController.Instance.GetRoomName());
    }
    private void OnEnable()
    {
        RoomController.ActionOnPlayerListChanged += DisplayRoomSlots;
    }
    private void OnDisable()
    {
        RoomController.ActionOnPlayerListChanged -= DisplayRoomSlots;
    }
    public void LeaveRoom()
    {
        PopupController.ShowYesNoPopup("Are you sure to leave room?",
            () => LobbyController.Instance.LeaveRoom()
            , null);

    }
    public void DisplayRoomSlots()
    {
        Debug.Log("PUN display");
        SlotGroup.DisplayRoomSlots();
    }
}
