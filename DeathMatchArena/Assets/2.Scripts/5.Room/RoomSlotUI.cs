using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RoomSlotUI : MonoBehaviour
{
    public enum SlotState { Empty, NotEmpty };

    private SlotState currentState = SlotState.Empty;
    public int Slotid;
    private string currentId;
    public RoomSlotGroup holder;

    [Header("Empty")]
    public GameObject EmptyState;

    [Header("Not Empty")]
    public GameObject NotEmptyState;
    public TextMeshProUGUI NotReady;
    public Image Ready;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Id;
    public GameObject KickButton;

    private void Start()
    {
        StartCoroutine(IEWaitReady());
    }
    public bool IsEmpty()
    {
        return currentState == SlotState.Empty;
    }
    public void SetPlayer(string id, string name, bool AmIMaster, bool isReady)
    {
        currentState = SlotState.NotEmpty;
        EmptyState.SetActive(false);
        NotEmptyState.SetActive(true);
        NotReady.gameObject.SetActive(true);
        Ready.gameObject.SetActive(false);

        Name.SetText(name);
        Id.SetText("Id: " + id);
        currentId = id;

        KickButton.SetActive(AmIMaster);
        if (isReady) SetReady();
        else SetNotReady();
    }
    public void SetEmpty()
    {
        currentState = SlotState.NotEmpty;
        EmptyState.SetActive(true);
        NotEmptyState.SetActive(false);
    }
    public void SetReady()
    {
        NotReady.gameObject.SetActive(false);
        Ready.gameObject.SetActive(true);
    }
    public void SetNotReady()
    {
        NotReady.gameObject.SetActive(true);
        Ready.gameObject.SetActive(false);
    }
    public void OpenBag()
    {
        if (currentId == PlayerData.GetId() && currentState == SlotState.NotEmpty)
        {
            PopupController.ShowBagPopup();
        }
    }
    public void SwitchToSlot()
    {
        holder.SwitchToSlot(Slotid);
    }
    public void KickPlayer()
    {

    }
    IEnumerator IEWaitReady()
    {
        int i = 0;
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            if (i % 4 == 0) NotReady.SetText("");
            else if (i % 4 == 1) NotReady.SetText(".");
            else if (i % 4 == 2) NotReady.SetText("..");
            else if (i % 4 == 3) NotReady.SetText("...");
            i++;
        }
    }
    public void SetHolder(RoomSlotGroup _holder, int slotindex)
    {
        holder = _holder;
        Slotid = slotindex;
    }
}
