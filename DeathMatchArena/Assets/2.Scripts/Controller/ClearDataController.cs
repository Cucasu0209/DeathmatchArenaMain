using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearDataController : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(AuthenticationController.Instance?.gameObject);
        Destroy(ChatController.Instance?.gameObject);
        Destroy(FriendController.Instance?.gameObject);
        Destroy(NetworkController_Chat.Instance?.gameObject);
        Destroy(NetworkController_PUN.Instance?.gameObject);
        Destroy(OtherPlayersController.Instance?.gameObject);
        Destroy(PlayfabController.Instance?.gameObject);
    }
}
