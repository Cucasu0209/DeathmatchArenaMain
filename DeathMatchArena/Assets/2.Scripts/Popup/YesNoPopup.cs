using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class YesNoPopup : BasePopup
{
    public static YesNoPopup Instance;
    public TextMeshProUGUI Notification;
    public Action ClickYesAction;
    public Action ClickNoAction;
    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }
    private void OnDisable()
    {
        ClickYesAction = null;
        ClickNoAction = null;
    }
    public void SetInformation(string msg, Action yes, Action no)
    {
        Notification.SetText(msg);
        ClickYesAction = yes;
        ClickNoAction = no;
    }
    public void Clickyes()
    {
        ClickYesAction?.Invoke();
        PopupController.HideYesNoPopup();
    }
    public void ClickNo()
    {
        ClickNoAction?.Invoke();
        PopupController.HideYesNoPopup();
    }
}
