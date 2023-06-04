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
    #region Variables
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
                {NetworkController_PUN.PLAYER_LOADED_LEVEL, true},
                {NetworkController_PUN.PLAYER_NAME, PlayerData.GetNickName()},
                {NetworkController_PUN.PLAYER_HEALTH, NetworkController_PUN.MAX_HEALTH},
                {NetworkController_PUN.PLAYER_PHYSICAL, NetworkController_PUN.MAX_PHYSICAL}
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
            Hashtable props = new Hashtable
            {
                {NetworkController_PUN.PLAYER_LOADED_LEVEL, true},
                {NetworkController_PUN.PLAYER_NAME, PlayerData.GetNickName()},
                {NetworkController_PUN.PLAYER_HEALTH, NetworkController_PUN.MAX_HEALTH},
                {NetworkController_PUN.PLAYER_PHYSICAL, NetworkController_PUN.MAX_PHYSICAL}
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


    }

    private void OnDisable()
    {
        NetworkController_PUN.ActionOnPlayerListChanged -= UpdateUI;
        NetworkController_PUN.ActionOnPlayerLeftRoom -= UpdateUI;
        NetworkController_PUN.ActionOnPlayerEnteredRoom -= UpdateUI;
        NetworkController_PUN.ActionOnJoinedRoom -= UpdateUI;
        NetworkController_PUN.ActionOnPlayerPropertiesUpdate -= UpdateUI;
    }


    #endregion

    #region Action
    private void UpdateUI()
    {
        Debug.Log("phus update");
        for (int i = 0; i < RoomController.Instance.PlayerInSlot.Count; i++)
        {
            if (RoomController.Instance.PlayerInSlot[i] != null)
            {
                Debug.Log("i phus " + i);
                if (PlayersHealthUI[i] != null)
                {
                    PlayersHealthUI[i].gameObject.SetActive(true);
                    PlayersHealthUI[i].DOKill();
                    PlayersHealthUI[i].DOValue(RoomController.Instance.GetHealth(RoomController.Instance.PlayerInSlot[i]) * 1f / NetworkController_PUN.MAX_HEALTH, 0.5f);
                }

                if (PlayersPhysicalUI[i] != null)
                {
                    PlayersPhysicalUI[i].gameObject.SetActive(true);
                    PlayersPhysicalUI[i].DOKill();
                    PlayersPhysicalUI[i].DOValue(RoomController.Instance.GetPhysical(RoomController.Instance.PlayerInSlot[i]) * 1f / NetworkController_PUN.MAX_PHYSICAL, 0.5f);
                }

                if (PlayersNameUI[i] != null)
                {
                    PlayersNameUI[i].gameObject.SetActive(true);
                    PlayersNameUI[i].SetText(RoomController.Instance.GetName(RoomController.Instance.PlayerInSlot[i]));
                }

            }
            else
            {
                Debug.Log("i phus fail" + i);
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
    #endregion
}
