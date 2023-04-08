using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmLogoutPopup : BasePopup
{
    public static ConfirmLogoutPopup Instance;
    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }
    public void ClickYes()
    {
        AuthenticationController.Instance.Logout();
        LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.Login);
    }
    public void ClickNo()
    {
        PopupController.HideConfirmLogoutPopup();
    }
}
