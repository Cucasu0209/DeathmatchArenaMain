using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupController
{
    private static readonly string LoadingPopupLink = "Popup/LoadingPopup";
    public static void ShowLoadingPopup(Canvas holder)
    {
        if (LoadingPopup.Instance != null)
        {
            LoadingPopup.Instance.Show();
        }
        else
        {
            GameObject myPopup = Resources.Load<GameObject>(LoadingPopupLink);
            if (myPopup != null)
            {
                GameObject NewPopup = GameObject.Instantiate(myPopup, holder.transform);
                NewPopup.GetComponent<BasePopup>()?.Show();
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
}
