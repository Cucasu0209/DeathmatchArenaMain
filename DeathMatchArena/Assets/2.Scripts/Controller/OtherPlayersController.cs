using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;
using System.Linq;
public class OtherPlayersController : MonoBehaviour
{
    #region Singleton
    protected OtherPlayersController() { }

    private static OtherPlayersController f_instance;

    /// <summary> Returns a reference to the UIPopupManager in the scene. If one does not exist, it gets created. </summary>
    public static OtherPlayersController Instance
    {
        get
        {
            if (f_instance != null) return f_instance;
            //if (ApplicationIsQuitting) return null;
            f_instance = FindObjectOfType<OtherPlayersController>();
            if (f_instance == null) DontDestroyOnLoad(AddToScene().gameObject);
            return f_instance;
        }
    }
    private static OtherPlayersController AddToScene(bool selectGameObjectAfterCreation = false) { return AddToScene<OtherPlayersController>($"{MethodBase.GetCurrentMethod().DeclaringType}", true, selectGameObjectAfterCreation); }
    public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation = false) where T : MonoBehaviour
    {
        var component = FindObjectOfType<T>();
        if (component != null && isSingleton)
        {
            Debug.Log("Cannot add another " + typeof(T).Name + " to this Scene because you don't need more than one.");
#if UNITY_EDITOR
            UnityEditor.Selection.activeObject = component;
#endif
            return component;
        }

        component = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();

#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(component.gameObject, "Created " + gameObjectName);
        if (selectGameObjectAfterCreation) UnityEditor.Selection.activeObject = component.gameObject;
#endif
        return component;
    }
    private void Awake()
    {
        if (f_instance != null && f_instance != this)
        {
            Destroy(gameObject);
            return;
        }

        f_instance = this;
        DontDestroyOnLoad(gameObject);


    }
    public void InitInstance() { }
    #endregion

    #region Variables
    private Dictionary<string, PlayerPlayfabInformation> _tempOtherPlayer = new Dictionary<string, PlayerPlayfabInformation>();
    private List<string> _tempRequest = new List<string>();
    private List<string> _tempInvitation = new List<string>();

    public static Action OnPlayerFocusChange;
    public static Action<ChatMessage_Photon> OnTempListChange;
    public PlayerPlayfabInformation currentFocus;
    #endregion

    #region Unity
    private void OnEnable()
    {
        NetworkController_Chat.OnPrivateChatMessageCome += OnChatMessageCome;
    }
    private void OnDisable()
    {
        NetworkController_Chat.OnPrivateChatMessageCome += OnChatMessageCome;
    }
    float cd = 0;
    public void Update()
    {
        cd += Time.deltaTime;
        if (cd >= 2) { GetAllPlayers(0, null); cd = 0; }
    }
    #endregion

    #region Actions
    public PlayerPlayfabInformation GetInfFromId(string playfabid = null)
    {
        if (string.IsNullOrEmpty(playfabid)) playfabid = GetIdFocus();
        if (_tempOtherPlayer.ContainsKey(playfabid)) return _tempOtherPlayer[playfabid];
        return new PlayerPlayfabInformation() { PlayFabId = playfabid };
    }
    public string GetIdFocus()
    {
        if (currentFocus == null) return "";
        return currentFocus.PlayFabId;
    }
    public void OnChatMessageCome(ChatMessage_Photon message)
    {
        if (message.type == ChatMessageType_Photon.RequestFriend)
        {
            NewRequestCome(message.senderId);
            OnTempListChange?.Invoke(message);
        }
        else if (message.type == ChatMessageType_Photon.CancelRequestFriend)
        {
            NewCancelRequestCome(message.senderId);
            OnTempListChange?.Invoke(message);
        }
        else if (message.type == ChatMessageType_Photon.AcceptRequestFriend)
        {
            NewAcceptInvitationCome(message.senderId);
            OnTempListChange?.Invoke(message);
        }
        else if (message.type == ChatMessageType_Photon.RefuserequestFriend)
        {
            NewRefuseInvitationCome(message.senderId);
            OnTempListChange?.Invoke(message);
        }
        else if (message.type == ChatMessageType_Photon.RemoveFriend)
        {
            NewRemoveFriendCome(message.senderId);
            OnTempListChange?.Invoke(message);
        }
    }
    #endregion

    #region GetType
    public bool IsMe(string playfabId)
    {
        return playfabId == PlayerData.GetId();
    }
    public bool IsPlayerInInvitationList(string playfabId)
    {
        return _tempInvitation.Contains(playfabId);
    }
    public bool IsPlayerInFriendList(string playfabId)
    {
        return FriendController.Instance.IsFriend(playfabId);
    }
    public bool IsPlayerInRequestList(string playfabId)
    {
        return _tempRequest.Contains(playfabId);
    }
    #endregion

    #region All Player
    public Dictionary<string, PlayerPlayfabInformation> GetTempAllPlayers()
    {
        return _tempOtherPlayer;
    }
    public void GetAllPlayers(int index, Action OnComplete)
    {
        PlayfabController.Instance.GetAllPlayersPlayfab(index, (players) =>
        {
            foreach (var player in players)
            {
                if (_tempOtherPlayer.ContainsKey(player.PlayFabId)) _tempOtherPlayer[player.PlayFabId] = player;
                else _tempOtherPlayer.Add(player.PlayFabId, player);
            }
            OnComplete?.Invoke();
        });
    }
    public void AddFriend(string playfabid = null)
    {
        if (string.IsNullOrEmpty(playfabid)) playfabid = GetIdFocus();

        //temp
        if (_tempRequest.Contains(playfabid) == false) _tempRequest.Add(playfabid);

        //server
        NetworkController_Chat.Instance.SendRequestFriendMessage(playfabid);
        PlayfabController.Instance.UpdateRequestAddfriend(_tempRequest, null);
    }
    public void AddTempPlayer(PlayerPlayfabInformation player)
    {
        if (_tempOtherPlayer.ContainsKey(player.PlayFabId)) _tempOtherPlayer[player.PlayFabId] = player;
        else _tempOtherPlayer.Add(player.PlayFabId, player);
    }


    #endregion

    #region Request
    public List<string> GetTempRequest()
    {
        return _tempRequest;
    }
    public void GetRequestAddfriend(Action OnComplete)
    {
        PlayfabController.Instance.GetRequestAddfriend((requests) =>
        {
            foreach (var request in requests)
            {
                if (_tempRequest.Contains(request) == false) _tempRequest.Add(request);
            }
            OnComplete?.Invoke();
        });
    }
    public void CancelRequestFriend(string playfabid = null)
    {
        if (string.IsNullOrEmpty(playfabid)) playfabid = GetIdFocus();

        //temp
        if (_tempRequest.Contains(playfabid) == true) _tempRequest.Remove(playfabid);

        //server
        NetworkController_Chat.Instance.SendCancelRequestFriendMessage(playfabid);
        PlayfabController.Instance.UpdateRequestAddfriend(_tempRequest, null);
    }
    public void NewRequestCome(string playfabid)
    {
        //temp
        if (_tempInvitation.Contains(playfabid) == false) _tempInvitation.Add(playfabid);

        //server
        PlayfabController.Instance.UpdateInvitationfriend(_tempInvitation, null);
    }
    public void NewCancelRequestCome(string playfabid)
    {
        //temp
        if (_tempInvitation.Contains(playfabid) == true) _tempInvitation.Remove(playfabid);

        //server
        PlayfabController.Instance.UpdateInvitationfriend(_tempInvitation, null);
    }
    #endregion

    #region Invitaion
    public List<string> GetTempInvitaion()
    {
        return _tempInvitation;
    }
    public void GetInvitationfriend(Action OnComplete)
    {
        PlayfabController.Instance.GetInvitationfriend((invitations) =>
        {
            foreach (var invitation in invitations)
            {
                if (_tempInvitation.Contains(invitation) == false) _tempInvitation.Add(invitation);
            }
            OnComplete?.Invoke();
        });
    }
    public void AcceptInvitationFriend(string playfabid = null)
    {
        if (string.IsNullOrEmpty(playfabid)) playfabid = GetIdFocus();

        //temp
        if (_tempInvitation.Contains(playfabid) == true) _tempInvitation.Remove(playfabid);

        //server
        NetworkController_Chat.Instance.SendAcceptInvitaionFriendMessage(playfabid);
        PlayfabController.Instance.UpdateInvitationfriend(_tempInvitation, null);
        FriendController.Instance.SaveNewFriend(playfabid);
    }
    public void RefuseInvitationFriend(string playfabid = null)
    {
        if (string.IsNullOrEmpty(playfabid)) playfabid = GetIdFocus();

        //temp
        if (_tempInvitation.Contains(playfabid) == true) _tempInvitation.Remove(playfabid);

        //server
        NetworkController_Chat.Instance.SendRefuseInvitationFriendMessage(playfabid);
        PlayfabController.Instance.UpdateInvitationfriend(_tempInvitation, null);
    }
    #endregion

    #region  Friend
    public void RemoveFriend(string playfabid = null)
    {
        if (string.IsNullOrEmpty(playfabid)) playfabid = GetIdFocus();

        NetworkController_Chat.Instance.SendRemoveFriendMessage(playfabid);
        FriendController.Instance.SaveRemoveFriend(playfabid);
    }
    public void NewAcceptInvitationCome(string playfabid)
    {
        if (_tempRequest.Contains(playfabid) == true)
        {
            FriendController.Instance.SaveNewFriend(playfabid);
            _tempRequest.Remove(playfabid);
            PlayfabController.Instance.UpdateRequestAddfriend(_tempRequest, null);
        }
    }
    public void NewRefuseInvitationCome(string playfabid)
    {
        if (_tempRequest.Contains(playfabid) == true)
        {
            _tempRequest.Remove(playfabid);
            PlayfabController.Instance.UpdateRequestAddfriend(_tempRequest, null);
        }
    }

    public void NewRemoveFriendCome(string playfabid)
    {
        FriendController.Instance.SaveRemoveFriend(playfabid);
    }
    #endregion
}
