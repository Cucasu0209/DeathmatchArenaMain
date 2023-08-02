using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
public class GamePlayController : MonoBehaviour
{
    public static GamePlayController instance;
    private int timeasd = 300;
    #region Variables UI
    bool isendgame = false;
    public Slider[] PlayersHealthUI;
    public Slider[] PlayersPhysicalUI;
    public TextMeshProUGUI[] PlayersNameUI;
    public TextMeshProUGUI TimeCollapsed;

    [Header("Start Game")]
    public Image StartDarkBG;
    public TextMeshProUGUI cd3;
    public TextMeshProUGUI cd2;
    public TextMeshProUGUI cd1;


    public TextMeshProUGUI Death;


    public Transform spawnPos1;
    public Transform spawnPos2;
    public Transform spawnPos3;
    public Transform spawnPos4;

    #endregion

    #region Unity
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {

        if (AuthenticationController.Instance.IsLogin() == false)
        {
            NetworkController_PUN.ActionOnJoinedRoom += () =>
            {
                DOVirtual.DelayedCall(3, () =>
                {
                    int slotIndex = NetworkController_PUN.Instance.GetPlayerProperties(PhotonNetwork.LocalPlayer).slotInRoom;
                    Vector3 spawnPos = slotIndex == 0 ? spawnPos1.position :
                                        slotIndex == 1 ? spawnPos2.position :
                                        slotIndex == 2 ? spawnPos3.position :
                                        slotIndex == 3 ? spawnPos4.position : Vector3.zero;
                    GameObject newChar = PhotonNetwork.Instantiate("Character", spawnPos, Quaternion.identity);
                    Physics2D.IgnoreLayerCollision(newChar.layer, newChar.layer);
                    Hashtable props = new Hashtable
                    {
                        {PlayerProperties.PLAYER_LOADED_LEVEL, true},
                        {PlayerProperties.PLAYER_NAME, PlayerData.GetNickName()},
                        {PlayerProperties.PLAYER_HEALTH, PlayerProperties.MAX_HEALTH},
                        {PlayerProperties.PLAYER_PHYSICAL, PlayerProperties.MAX_PHYSICAL},
                        {PlayerProperties.PLAYER_WEAPON, PlayerData.GetCurrentWeaponIndex()},
                        {PlayerProperties.PLAYER_HAT, PlayerData.GetCurrentHatIndex()},
                        {PlayerProperties.PLAYER_SHOE, PlayerData.GetCurrentShoeIndex()},
                    };
                    PhotonNetwork.LocalPlayer.SetCustomProperties(props);
                });


            };
            NetworkController_PUN.ActionOnJoinRandomFailed += () =>
            {
                NetworkController_PUN.Instance.CreateRoom("DefaultRoom");
            };
            NetworkController_PUN.ActionOnJoinedLobby += () =>
            {
                NetworkController_PUN.Instance.JoinRandomRoom();
            };
            PopupController.ShowLoadingPopup();
            AuthenticationController.Instance.LoginDefault((result) =>
            {
                PopupController.HideLoadingPopup();
                NetworkController_PUN.Instance.ConnectPUN(() =>
                {
                    NetworkController_PUN.Instance.JoinLobby();

                });
            });
        }
        else
        {
            int slotIndex = NetworkController_PUN.Instance.GetPlayerProperties(PhotonNetwork.LocalPlayer).slotInRoom;
            Vector3 spawnPos = slotIndex == 0 ? spawnPos1.position :
                                slotIndex == 1 ? spawnPos2.position :
                                slotIndex == 2 ? spawnPos3.position :
                                slotIndex == 3 ? spawnPos4.position : Vector3.zero;
            GameObject newChar = PhotonNetwork.Instantiate("Character", spawnPos, Quaternion.identity);
            Physics2D.IgnoreLayerCollision(newChar.layer, newChar.layer);
            Physics2D.IgnoreLayerCollision(8, 8);
            Hashtable props = new Hashtable
            {
                {PlayerProperties.PLAYER_LOADED_LEVEL, true},
                {PlayerProperties.PLAYER_NAME, PlayerData.GetNickName()},
                {PlayerProperties.PLAYER_HEALTH, PlayerProperties.MAX_HEALTH},
                {PlayerProperties.PLAYER_PHYSICAL, PlayerProperties.MAX_PHYSICAL},
                {PlayerProperties.PLAYER_WEAPON, PlayerData.GetCurrentWeaponIndex()},
                {PlayerProperties.PLAYER_HAT, PlayerData.GetCurrentHatIndex()},
                {PlayerProperties.PLAYER_SHOE, PlayerData.GetCurrentShoeIndex()},
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        StartCoroutine(StartGame());
    }

    private void OnEnable()
    {
        //UpdateUI();
        NetworkController_PUN.ActionOnPlayerListChanged += UpdateUI;
        NetworkController_PUN.ActionOnPlayerLeftRoom += UpdateUI;
        NetworkController_PUN.ActionOnPlayerEnteredRoom += UpdateUI;
        NetworkController_PUN.ActionOnJoinedRoom += UpdateUI;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += UpdateUI;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate += Checkgameover;


    }

    private void OnDisable()
    {
        NetworkController_PUN.ActionOnPlayerListChanged -= UpdateUI;
        NetworkController_PUN.ActionOnPlayerLeftRoom -= UpdateUI;
        NetworkController_PUN.ActionOnPlayerEnteredRoom -= UpdateUI;
        NetworkController_PUN.ActionOnJoinedRoom -= UpdateUI;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateUI;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= Checkgameover;
    }

    public void ShowDeath()
    {
        Death.DOFade(1, 0.5f);
    }
    #endregion

    #region Action
    private void UpdateUI()
    {
        for (int i = 0; i < RoomController.Instance.PlayerInSlot.Count; i++)
        {
            if (RoomController.Instance.PlayerInSlot[i] != null)
            {
                if (PlayersHealthUI[i] != null)
                {
                    PlayersHealthUI[i].gameObject.SetActive(true);
                    PlayersHealthUI[i].DOKill();
                    PlayersHealthUI[i].DOValue(NetworkController_PUN.Instance.GetPlayerProperties(RoomController.Instance.PlayerInSlot[i]).playerHealth * 1f / PlayerProperties.MAX_HEALTH, 0.5f);
                }

                if (PlayersPhysicalUI[i] != null)
                {
                    PlayersPhysicalUI[i].gameObject.SetActive(true);
                    PlayersPhysicalUI[i].DOKill();
                    PlayersPhysicalUI[i].DOValue(NetworkController_PUN.Instance.GetPlayerProperties(RoomController.Instance.PlayerInSlot[i]).playerPhysical * 1f / PlayerProperties.MAX_PHYSICAL, 0.5f);
                }

                if (PlayersNameUI[i] != null)
                {
                    PlayersNameUI[i].gameObject.SetActive(true);
                    PlayersNameUI[i].SetText(NetworkController_PUN.Instance.GetPlayerProperties(RoomController.Instance.PlayerInSlot[i]).playerName);
                }

            }
            else
            {
                if (PlayersHealthUI.Length >= i + 1 && PlayersHealthUI[i] != null)
                {
                    PlayersHealthUI[i].gameObject.SetActive(false);

                }

                if (PlayersPhysicalUI.Length >= i + 1 && PlayersPhysicalUI[i] != null)
                {
                    PlayersPhysicalUI[i].gameObject.SetActive(false);

                }

                if (PlayersNameUI.Length >= i + 1 && PlayersNameUI[i] != null)
                {
                    PlayersNameUI[i].gameObject.SetActive(false);
                }
            }
        }
    }
    private void Checkgameover()
    {
        if (isendgame) return;

        if (RoomController.Instance.PlayerInSlot[0] == null && RoomController.Instance.PlayerInSlot[1] == null) return;
        if (RoomController.Instance.PlayerInSlot[2] == null && RoomController.Instance.PlayerInSlot[3] == null) return;

        float[] health = new float[4] { 0, 0, 0, 0 };
        for (int i = 0; i < RoomController.Instance.PlayerInSlot.Count; i++)
        {
            if (RoomController.Instance.PlayerInSlot[i] != null)
            {
                health[i] = NetworkController_PUN.Instance.GetPlayerProperties(RoomController.Instance.PlayerInSlot[i]).playerHealth;
            }
        }

        if (health[0] + health[1] <= 0 || health[2] + health[3] <= 0 || timeasd <= 0)
        {
            isendgame = true;
            //if (health[0] + health[1] <= 0)
            //{
            //    Debug.LogError("team 2 win");
            //}
            //else if (health[2] + health[3] <= 0)
            //{
            //    Debug.LogError("team 1 win");
            //}
            //else if (health[0] + health[1] > health[2] + health[3])
            //{
            //    Debug.LogError("team 1 win");
            //}
            //else if (health[0] + health[1] < health[2] + health[3])
            //{
            //    Debug.LogError("team 1 win");
            //}
            //else if (health[0] + health[1] == health[2] + health[3])
            //{
            //    Debug.LogError("draw");
            //}
            //else
            //{
            //    Debug.LogError("draw");
            //}
            StartCoroutine(IDelayLoadGame());
        }

    }

    IEnumerator IDelayLoadGame()
    {
        yield return new WaitForSeconds(3);
        LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.EndGame);
    }

    public IEnumerator StartGame()
    {
        yield return new WaitForSeconds(2f);
        cd3.gameObject.SetActive(true);
        cd3.transform.localScale = Vector3.zero;
        cd3.transform.DOScale(Vector3.one, 1.5f).OnComplete(() =>
        {
            cd3.DOFade(1, 0.5f);
            cd3.DOFade(0, 0.9f);
        });
        yield return new WaitForSeconds(2f);
        cd2.gameObject.SetActive(true);
        cd2.transform.localScale = Vector3.zero;
        cd2.transform.DOScale(Vector3.one, 1.5f).OnComplete(() =>
        {
            cd2.DOFade(1, 0.5f);
            cd2.DOFade(0, 0.9f);
        });
        yield return new WaitForSeconds(2f);
        cd1.gameObject.SetActive(true);
        cd1.transform.localScale = Vector3.zero;
        cd1.transform.DOScale(Vector3.one, 1.5f).OnComplete(() =>
        {
            cd1.DOFade(1, 0.5f);
            cd1.DOFade(0, 0.9f);
        });
        yield return new WaitForSeconds(2f);

        StartDarkBG.DOFade(0, 0.2f);

        while (timeasd >= 0)
        {
            TimeCollapsed.SetText(timeasd.ToString());
            yield return new WaitForSeconds(1);
            timeasd--;
            RoomController.Instance.timeasd = timeasd;
        }
        Checkgameover();
    }


    #endregion
}
