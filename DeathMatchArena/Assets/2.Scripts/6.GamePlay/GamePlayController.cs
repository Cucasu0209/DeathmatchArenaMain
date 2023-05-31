using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GamePlayController : MonoBehaviour
{
    // Start is called before the first frame update
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
                {NetworkController_PUN.PLAYER_NAME, PlayerData.GetNickName()}
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
                {NetworkController_PUN.PLAYER_NAME, PlayerData.GetNickName()}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

    
    }

    // Update is called once per frame
    void Update()
    {

    }
}
