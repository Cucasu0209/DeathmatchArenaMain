using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
public class LoadingPopup : BasePopup
{
    public static LoadingPopup Instance;
    public TextMeshProUGUI LoadingText;
    public TextMeshProUGUI PercentText;

    public override void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        base.Awake();
    }
    private void OnEnable()
    {
        PercentText.SetText("");
        LoadingController.OnIncreaseProgress += Setpercent;
    }
    private void OnDisable()
    {
        LoadingController.OnIncreaseProgress -= Setpercent;
    }
    public override void Show()
    {
        base.Show();
        StartCoroutine(IELoading());
    }
    public override void Hide()
    {
        base.Hide();
        StopAllCoroutines();
    }
    public void Setpercent()
    {
        PercentText.SetText(Mathf.CeilToInt(LoadingController.Instance.numberOfDoneTask * 100f / LoadingController.Instance.maxTask).ToString());
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
