using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class ChatUI : MonoBehaviour
{
    public RectTransform ChatPanel;

    public void ToggleChatPanel()
    {
        float distanceToZero = Vector3.Distance(ChatPanel.localScale, Vector3.zero);
        float distanceToOne = Vector3.Distance(ChatPanel.localScale, Vector3.one);

        if (distanceToOne > distanceToZero)
        {
            ChatPanel.DOKill();
            ChatPanel.DOScale(1, 0.2f);
        }
        else
        {
            ChatPanel.DOKill();
            ChatPanel.DOScale(0, 0.2f);
        }
    }
}
