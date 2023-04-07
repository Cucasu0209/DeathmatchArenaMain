using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class LoginSceneController : MonoBehaviour
{
    public static LoginSceneController Instance;
    public Canvas MainCanvas;

    private void Awake()
    {
        Instance = this;
    }
}
