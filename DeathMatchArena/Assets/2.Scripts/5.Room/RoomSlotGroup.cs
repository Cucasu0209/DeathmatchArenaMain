using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

public class RoomSlotGroup : MonoBehaviour
{

    public RoomSlotUI Slot1_Team1;
    public RoomSlotUI Slot2_Team1;
    public RoomSlotUI Slot1_Team2;
    public RoomSlotUI Slot2_Team2;

    Dictionary<int, RoomSlotUI> slots = new Dictionary<int, RoomSlotUI>();
    private void Start()
    {
        Slot1_Team1?.SetHolder(this, 0);
        Slot1_Team2?.SetHolder(this, 2);
        Slot2_Team1?.SetHolder(this, 1);
        Slot2_Team2?.SetHolder(this, 3);
        slots.Add(0, Slot1_Team1);
        slots.Add(1, Slot2_Team1);
        slots.Add(2, Slot1_Team2);
        slots.Add(3, Slot2_Team2);
    }

    public void DisplayRoomSlots()
    {
        for (int i = 0; i <= 3; i++)
        {
            if (RoomController.Instance.PlayerInSlot[i] != null)
            {
                Player p = RoomController.Instance.PlayerInSlot[i];

                slots[i].SetPlayer(RoomController.Instance.GetId(p),
                   RoomController.Instance.GetName(p),
                   NetworkController_PUN.Instance.AmIMasterClient(),
                   RoomController.Instance.GetIsReady(p));
            }
            else
            {
                slots[i].SetEmpty();
            }
        }
    }

    public void SwitchToSlot(int slotId)
    {
        if (RoomUI.Instance.AmIReady == false)
            NetworkController_PUN.Instance.SetSlot(slotId);
    }

}
