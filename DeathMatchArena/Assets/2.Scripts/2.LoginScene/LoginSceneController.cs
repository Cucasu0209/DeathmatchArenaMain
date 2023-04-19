using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Containers;

public class LoginSceneController : MonoBehaviour
{
    public static LoginSceneController Instance;
    public UIContainer LoginContainer;
    public UIContainer RegisterContainer;
    public UIContainer StartNameContainer;

    private void Awake()
    {
        Instance = this;
    }
    public void ShowLoginContainer()
    {
        LoginContainer.Show();
        RegisterContainer.Hide();
        StartNameContainer.Hide();
    }
    public void ShowRegisterContainer()
    {
        LoginContainer.Hide();
        RegisterContainer.Show();
        StartNameContainer.Hide();
    }
    public void ShowStartNameContainer()
    {
        LoginContainer.Hide();
        RegisterContainer.Hide();
        StartNameContainer.Show();
    }
}
