using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PopupController
{
    #region Link
    private static readonly string PopupCanvasLink = "Popup/PopupCanvas";
    private static readonly string LoadingPopupLink = "Popup/LoadingPopup";
    private static readonly string LoginAfterRegisterPopupLink = "Popup/LoginAfterRegisterPopup";
    private static readonly string RenamePopupLink = "Popup/RenamePopup";
    private static readonly string ConfirmLogoutPopupLink = "Popup/ConfirmLogoutPopup";
    private static readonly string YesNoPopupLink = "Popup/YesNoPopup";
    private static readonly string NewGroupChatPopupLink = "Popup/NewGroupChatPopup";
    private static readonly string FriendListToAddGroupPopupLink = "Popup/FriendListToAddGroupPopup";
    
    #endregion

    #region Action
    public static PopupCanvas GetPopupCanvas()
    {
        if (PopupCanvas.Instance != null)
        {
            return PopupCanvas.Instance;
        }

        GameObject popupCanvas = Resources.Load<GameObject>(PopupCanvasLink);
        if (popupCanvas != null)
        {
            return GameObject.Instantiate(popupCanvas, null).GetComponent<PopupCanvas>();
        }
        else
        {
            return null;
        }

    }
    public static void ShowLoadingPopup()
    {
        if (LoadingPopup.Instance != null)
        {
            LoadingPopup.Instance.Show();
            LoadingPopup.Instance.transform.SetAsLastSibling();
        }
        else
        {

            GameObject myPopup = Resources.Load<GameObject>(LoadingPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, GetPopupCanvas().transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
                NewPopup.GetComponent<BasePopup>()?.transform.SetAsLastSibling();
            }
        }

    }
    public static void HideLoadingPopup()
    {
        if (LoadingPopup.Instance != null)
        {
            LoadingPopup.Instance.Hide();
        }

    }
    public static void ShowLoginAfterRegisterPopupPopup(string username, string password)
    {
        if (LoginAfterRegisterPopup.Instance != null)
        {
            LoginAfterRegisterPopup.Instance.Show();
            LoginAfterRegisterPopup.Instance.transform.SetAsLastSibling();
            LoginAfterRegisterPopup.Instance.SetInformation(username, password);
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(LoginAfterRegisterPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, GetPopupCanvas().transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
                NewPopup.GetComponent<BasePopup>()?.transform.SetAsLastSibling();
                NewPopup.GetComponent<LoginAfterRegisterPopup>()?.SetInformation(username, password);
            }
        }

    }
    public static void HideLoginAfterRegisterPopupPopup()
    {
        if (LoginAfterRegisterPopup.Instance != null)
        {
            LoginAfterRegisterPopup.Instance.Hide();
        }

    }
    public static void ShowRenamePopup()
    {
        if (RenamePopup.Instance != null)
        {
            RenamePopup.Instance.Show();
            RenamePopup.Instance.transform.SetAsLastSibling();
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(RenamePopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, GetPopupCanvas().transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
                NewPopup.GetComponent<BasePopup>()?.transform.SetAsLastSibling();
            }
        }

    }
    public static void HideRenamePopup()
    {
        if (RenamePopup.Instance != null)
        {
            RenamePopup.Instance.Hide();
        }

    }
    public static void ShowConfirmLogoutPopup()
    {
        if (ConfirmLogoutPopup.Instance != null)
        {
            ConfirmLogoutPopup.Instance.Show();
            ConfirmLogoutPopup.Instance.transform.SetAsLastSibling();
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(ConfirmLogoutPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, GetPopupCanvas().transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
                NewPopup.GetComponent<BasePopup>()?.transform.SetAsLastSibling();

            }
        }

    }
    public static void HideConfirmLogoutPopup()
    {
        if (ConfirmLogoutPopup.Instance != null)
        {
            ConfirmLogoutPopup.Instance.Hide();
        }

    }
    public static void ShowYesNoPopup(string notifyMessage,Action Yes, Action No)
    {
        if (YesNoPopup.Instance != null)
        {
            YesNoPopup.Instance.Show();
            YesNoPopup.Instance.transform.SetAsLastSibling();
            YesNoPopup.Instance.SetInformation(notifyMessage, Yes, No);
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(YesNoPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, GetPopupCanvas().transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
                NewPopup.GetComponent<BasePopup>()?.transform.SetAsLastSibling();
                NewPopup.GetComponent<YesNoPopup>()?.SetInformation(notifyMessage, Yes, No);

            }
        }

    }
    public static void HideYesNoPopup()
    {
        if (YesNoPopup.Instance != null)
        {
            YesNoPopup.Instance.Hide();
        }

    }
    public static void ShowNewGroupChatPopup()
    {
        if (NewGroupChatPopup.Instance != null)
        {
            NewGroupChatPopup.Instance.Show();
            NewGroupChatPopup.Instance.transform.SetAsLastSibling();
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(NewGroupChatPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, GetPopupCanvas().transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
                NewPopup.GetComponent<BasePopup>()?.transform.SetAsLastSibling();
            }
        }

    }
    public static void HideNewGroupChatPopup()
    {
        if (NewGroupChatPopup.Instance != null)
        {
            NewGroupChatPopup.Instance.Hide();
        }

    }
    public static void ShowFriendListToAddGroupPopup(GroupChatInfomation group)
    {
        if (FriendListToAddGroupPopup.Instance != null)
        {
            FriendListToAddGroupPopup.Instance.Show();
            FriendListToAddGroupPopup.Instance.transform.SetAsLastSibling();
            FriendListToAddGroupPopup.Instance.ShowList(group);
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(FriendListToAddGroupPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, GetPopupCanvas().transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
                NewPopup.GetComponent<BasePopup>()?.transform.SetAsLastSibling();
                NewPopup.GetComponent<FriendListToAddGroupPopup>()?.ShowList(group);
            }
        }

    }
    public static void HideFriendListToAddGroupPopup()
    {
        if (FriendListToAddGroupPopup.Instance != null)
        {
            FriendListToAddGroupPopup.Instance.Hide();
        }

    }
    #endregion
}
