using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;


public class BasePopup : MonoBehaviour
{
    public Image Overlay;
    public Image Container;

    public virtual void Awake()
    {
        Container.rectTransform.localScale = Vector3.zero;
        Overlay.color = new Color(Overlay.color.r, Overlay.color.g, Overlay.color.b, 0);
    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
        Container.rectTransform.DOScale(Vector3.one, 0.2f);
        Overlay.DOFade(0.3f, 0.2f);
    }
    public virtual void Hide()
    {
        Container.rectTransform.DOKill();
        Overlay.DOKill();
        Container.rectTransform.DOScale(Vector3.zero, 0.2f);
        Overlay.DOFade(0, 0.2f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
