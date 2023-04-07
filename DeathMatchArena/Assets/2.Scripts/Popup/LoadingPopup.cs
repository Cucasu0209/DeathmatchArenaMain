using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class LoadingPopup : BasePopup
{
    public static LoadingPopup Instance;
    public TextMeshProUGUI LoadingText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        Container.rectTransform.localScale = Vector3.zero;
        Overlay.color = new Color(Overlay.color.r, Overlay.color.g, Overlay.color.b, 0);
    }
    public override void Show()
    {
        gameObject.SetActive(true);
        base.Show();
        Container.rectTransform.DOScale(Vector3.one, 0.2f);
        Overlay.DOFade(0.3f, 0.2f);
        StartCoroutine(IELoading());
    }
    public override void Hide()
    {
        base.Hide();
        Container.rectTransform.DOKill();
        Overlay.DOKill();
        Container.rectTransform.DOScale(Vector3.zero, 0.2f);
        Overlay.DOFade(0, 0.2f).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
        StopAllCoroutines();
    }
    IEnumerator IELoading()
    {
        int i = 0;
        while (true)
        {
            i++;
            if (i % 4 == 0) LoadingText.SetText("Loading .");
            if (i % 4 == 1) LoadingText.SetText("Loading ..");
            if (i % 4 == 2) LoadingText.SetText("Loading ...");
            if (i % 4 == 3) LoadingText.SetText("Loading");
            yield return new WaitForSeconds(0.3f);
        }
    }
}
