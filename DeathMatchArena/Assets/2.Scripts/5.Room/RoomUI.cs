using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class RoomUI : MonoBehaviour
{
    public static RoomUI Instance;
    public bool AmIReady = false;
    public TextMeshProUGUI RoomName;
    public TextMeshProUGUI ReadyState;
    public RoomSlotGroup SlotGroup;
    public Transform PlayButton;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        RoomController.Instance.InitInstance();
        PlayButton.localScale = Vector3.zero;
        ReadyState.SetText("Ready!");
        AmIReady = false;
        DisplayRoomSlots();
        RoomName.SetText("Room: " + LobbyController.Instance.GetRoomName());
    }
    private void OnEnable()
    {
        RoomController.ActionOnPlayerListChanged += DisplayRoomSlots;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += CheckIfPlayGame;
    }
    private void OnDisable()
    {
        RoomController.ActionOnPlayerListChanged -= DisplayRoomSlots;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= CheckIfPlayGame;
    }
    public void LeaveRoom()
    {
        PopupController.ShowYesNoPopup("Are you sure to leave room?",
            () => LobbyController.Instance.LeaveRoom()
            , null);

    }
    public void DisplayRoomSlots()
    {
        SlotGroup.DisplayRoomSlots();
        CheckAndActivePlayButton();
    }
    public void SwitchReady()
    {
        AmIReady = !AmIReady;
        if (AmIReady) ReadyState.SetText("Cancel Ready");
        else ReadyState.SetText("Ready!");
        NetworkController_PUN.Instance.UpdateMyProperty(PlayerPropertiesType.isReady, AmIReady);
    }
    public void CheckAndActivePlayButton()
    {
        if (NetworkController_PUN.Instance.AmIMasterClient())
        {
            if (RoomController.Instance.IsEveryOneReady())
            {
                PlayButton.DOScale(1, 0.4f);
                return;
            }

        }

        PlayButton.DOScale(0, 0.4f);

    }
    private void CheckIfPlayGame()
    {

        if (RoomController.Instance.CheckCanPlayGame())
        {
            NetworkController_PUN.Instance.ResetGamePlayProperties();
            NetworkController_PUN.Instance.LockCurrentRoom();
            NetworkController_PUN.Instance.UpdateMyProperty<bool>(PlayerPropertiesType.isReady, false);
            NetworkController_PUN.Instance.UpdateMyProperty<bool>(PlayerPropertiesType.isLoaded, false);
            LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.MainGame);
        }
    }
    public void PlayGameClick()
    {
        NetworkController_PUN.Instance.UpdateMyProperty<bool>(PlayerPropertiesType.isMasterPlayGame, true);
        NetworkController_PUN.Instance.UpdateMyProperty<bool>(PlayerPropertiesType.isMasterPlayGame, false);
    }
}
