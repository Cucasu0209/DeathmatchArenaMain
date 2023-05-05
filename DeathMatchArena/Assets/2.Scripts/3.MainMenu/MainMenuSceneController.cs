using Doozy.Runtime.UIManager.Containers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MainMenuSceneController : MonoBehaviour
{
    public enum ContainerType
    {
        PlayGame,
        Setting,
        Friend,
        Shop,
        Rank,
    }
    public static MainMenuSceneController Instance;
    public UIContainer PlayGameContainer;
    public UIContainer SettingContainer;
    public UIContainer FriendContainer;
    public UIContainer RankContainer;
    public UIContainer ShopContainer;

    public ContainerType CurrentContainer = ContainerType.PlayGame;

    public TextMeshProUGUI FriendNotiCount;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

        if (AuthenticationController.Instance.IsLogin() == false)
        {
            PopupController.ShowLoadingPopup();
            AuthenticationController.Instance.LoginDefault((result) =>
            {
                PopupController.HideLoadingPopup();
            });
        }

    }
    private void Update()
    {
        if (OtherPlayersController.Instance.GetTempInvitaion().Count > 0)
        {
            FriendNotiCount.transform.parent.gameObject.SetActive(true);
            FriendNotiCount.SetText(OtherPlayersController.Instance.GetTempInvitaion().Count.ToString());
        }
        else FriendNotiCount.transform.parent.gameObject.SetActive(false);
    }
    public void ShowContainer(ContainerType type)
    {
        if (type == CurrentContainer) return;
        CurrentContainer = type;
        PlayGameContainer.Hide();
        SettingContainer.Hide();
        FriendContainer.Hide();
        RankContainer.Hide();
        ShopContainer.Hide();


        if (type == ContainerType.PlayGame) PlayGameContainer.Show();
        if (type == ContainerType.Setting) SettingContainer.Show();
        if (type == ContainerType.Friend) FriendContainer.Show();
        if (type == ContainerType.Rank) RankContainer.Show();
        if (type == ContainerType.Shop) ShopContainer.Show();
    }
    public void Quit()
    {
        PopupController.ShowConfirmLogoutPopup();
    }
    #region ButtonCallbacks
    public void GoToLobby()
    {
        LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.Lobby);
    }
    #endregion
}
