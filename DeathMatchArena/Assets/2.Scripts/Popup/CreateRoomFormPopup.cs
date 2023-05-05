using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CreateRoomFormPopup : BasePopup
{
    public static CreateRoomFormPopup Instance;
    public TextFieldComponentUI RoomName;

    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }

    private void OnEnable()
    {
        RoomName.SetText("Room"+ UnityEngine.Random.Range(0,1000));
    }

    public void ClickOk()
    {
        string _RoomName = RoomName.GetText();
        LobbyController.Instance.CreateRoom(_RoomName);
    }
    public void ClickCancel()
    {
        PopupController.HideCreateRoomFormPopup();
    }
 
}
