using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class LoadingUI : MonoBehaviour
{
    public TextMeshProUGUI Notification;
    public Image ProgressHandler;
    private float StartX, EndX;
    private void Awake()
    {
        StartX = -ProgressHandler.rectTransform.sizeDelta.x;
        EndX = 0;
        SetProgress(0);
    }
    private void OnEnable()
    {
        LoadingController.OnIncreaseProgress += UpdateProgress;
    }
    private void OnDisable()
    {
        LoadingController.OnIncreaseProgress -= UpdateProgress;
    }
    private void SetProgress(float value)
    {
        float newX = StartX + (EndX - StartX) * value / 100;
        float newY = ProgressHandler.rectTransform.anchoredPosition.y;
        Notification.SetText("Loading ... " + value);
        ProgressHandler.rectTransform.anchoredPosition = new Vector2(newX, newY);
    }
    private void DoProgress(float value)
    {
        ProgressHandler.rectTransform.DOKill();
        float newX = StartX + (EndX - StartX) * value / 100;
        float distance = Mathf.Abs(ProgressHandler.rectTransform.anchoredPosition.x - newX);
        float duration = distance * 0.1f;
        ProgressHandler.rectTransform.DOAnchorPosX(newX, duration).SetEase(Ease.Linear);

    }
    private void UpdateProgress()
    {
        Debug.Log(LoadingController.Instance.numberOfDoneTask + "/" + LoadingController.Instance.maxTask);
        SetProgress(LoadingController.Instance.numberOfDoneTask * 1f / LoadingController.Instance.maxTask * 100);
    }
}
