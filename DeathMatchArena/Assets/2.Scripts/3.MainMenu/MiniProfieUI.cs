using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniProfieUI : MonoBehaviour
{
    public TextMeshProUGUI NickName;
    public TextMeshProUGUI Id;

    private void OnEnable()
    {
        UpdateId();
        UpdateNickname();
        PlayerData.OnNickNameChange += UpdateNickname;
        PlayerData.OnIdChange += UpdateId;
    }
    private void OnDisable()
    {
        PlayerData.OnNickNameChange -= UpdateNickname;
        PlayerData.OnIdChange -= UpdateId;
    }

    private void UpdateNickname()
    {
        NickName.SetText(PlayerData.GetNickName());
    }
    private void UpdateId()
    {
        Id.SetText("ID: " + PlayerData.GetId());
    }
    public void Rename()
    {
        PopupController.ShowRenamePopup();
    }
    public void OpenBag()
    {
        PopupController.ShowBagPopup();
    }
}
