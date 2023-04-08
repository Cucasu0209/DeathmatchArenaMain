using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LoadSceneSmoothEntity : MonoBehaviour
{
    public static LoadSceneSmoothEntity Instance;

    public Image Panel;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        Panel.color = new Color(Panel.color.r, Panel.color.g, Panel.color.b, 0);
    }
    public void Show(Action OnComplete)
    {
        gameObject.SetActive(true);
        Panel.DOFade(1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            OnComplete?.Invoke();
        });
    }
    public void Hide(Action OnComplete)
    {
        Panel.DOFade(0, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            OnComplete?.Invoke();
            gameObject.SetActive(false);
        });
    }
}
