using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class RootController : MonoBehaviour
{
    public Image Logo;
    public TextMeshProUGUI Studio;
    private void Awake()
    {
        Logo.color = new Color(Logo.color.r, Logo.color.g, Logo.color.b, 0);
        Studio.color = new Color(Studio.color.r, Studio.color.g, Studio.color.b, 0);

        Logo.DOFade(1, 3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Logo.DOFade(0, 1).SetDelay(3).OnComplete(() =>
            {
                LoadSceneSmoothController.Instance.LoadScene(SceneEnum.Type.Login);
            });
        });


        Studio.DOFade(1, 3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            Studio.DOFade(0, 1).SetDelay(3);
        });

    }
}
