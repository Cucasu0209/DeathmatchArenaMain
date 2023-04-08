using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PopupController
{
    #region Link
    private static readonly string LoadingPopupLink = "Popup/LoadingPopup";
    private static readonly string LoginAfterRegisterPopupLink = "Popup/LoginAfterRegisterPopup";
    #endregion

    #region Action
    public static void ShowLoadingPopup(Canvas holder)
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
                GameObject NewPopup = GameObject.Instantiate(myPopup, holder.transform);
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
    public static void ShowLoginAfterRegisterPopupPopup(string username, string password, Canvas holder)
    {
        if (LoginAfterRegisterPopup.Instance != null)
        {
            LoginAfterRegisterPopup.Instance.Show();
            LoginAfterRegisterPopup.Instance.transform.SetAsLastSibling();
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(LoginAfterRegisterPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, holder.transform);
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
    #endregion
}
