using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatChannelUIItem : MonoBehaviour
{
    private string channelID;
    public TextMeshProUGUI Name;
    public Button btn;
    public Image BG;

    private void OnEnable()
    {
        BG.color = new Color(BG.color.r, BG.color.g, BG.color.b, 0);
        ChatController.OnChatPartnerForcusChange += OnPlayerFocusChange;
    }
    private void OnDisable()
    {
        ChatController.OnChatPartnerForcusChange -= OnPlayerFocusChange;
    }

    private void OnPlayerFocusChange()
    {
        if (channelID == null) return;
        if (channelID ==   ChatController.Instance.GetIdFocus())
        {
            BG.DOKill();
            BG.DOFade(1, 0.2f);
            btn.transform.DOKill();
            btn.transform.DOScale(Vector3.one * 1.1f, 0.2f);
        }
        else
        {
            BG.DOKill();
            BG.DOFade(0, 0.2f);
            btn.transform.DOKill();
            btn.transform.DOScale(Vector3.one, 0.2f);
        }
    }


    public void SetInformation(string channelId, string channelName)
    {
        channelID = channelId;
        Name?.SetText(channelName);
        btn?.onClick.AddListener(() => ChatUI.Instance.ChangePartnerChatWith(new ChatPartner() { Id = channelID, Type = ChatPartnerType.Channel }));
    }
    public void ClearInformation()
    {
        btn?.onClick.RemoveAllListeners();

    }
}
