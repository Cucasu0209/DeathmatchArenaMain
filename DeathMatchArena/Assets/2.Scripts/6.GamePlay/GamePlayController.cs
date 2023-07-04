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
    #region Variables UI
    public Slider[] PlayersHealthUI;
    public Slider[] PlayersPhysicalUI;
    public TextMeshProUGUI[] PlayersNameUI;
    #endregion

    #region Unity
    void Start()
    {

        if (AuthenticationController.Instance.IsLogin() == false)
        {
            NetworkController_PUN.ActionOnJoinedRoom += () =>
            {
                GameObject newChar = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);
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
            GameObject newChar = PhotonNetwork.Instantiate("Character", Vector3.zero, Quaternion.identity);
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

        if (RoomController.Instance.PlayerInSlot[0] == null && RoomController.Instance.PlayerInSlot[1] == null) return;
        if (RoomController.Instance.PlayerInSlot[2] == null && RoomController.Instance.PlayerInSlot[3] == null) return;

        bool[] isALive = new bool[4] { false, false, false, false };
        for (int i = 0; i < RoomController.Instance.PlayerInSlot.Count; i++)
        {
            if (RoomController.Instance.PlayerInSlot[i] != null)
            {
                isALive[i] = NetworkController_PUN.Instance.GetPlayerProperties(RoomController.Instance.PlayerInSlot[i]).playerHealth > 0;
            }
        }
        if ((isALive[0] || isALive[1] || isALive[2] || isALive[3]) == false)
        {
            Debug.LogError("Draw");
            LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.EndGame);
        }
        else if ((isALive[0] || isALive[1]) == false)
        {
            Debug.LogError("team 2 win");
            LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.EndGame);
        }
        else if ((isALive[2] || isALive[3]) == false)
        {
            Debug.LogError("team 1 win");
            LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.EndGame);
        }

    }
    #endregion
}
