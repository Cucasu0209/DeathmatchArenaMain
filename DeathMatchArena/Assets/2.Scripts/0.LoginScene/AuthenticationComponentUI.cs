using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class AuthenticationComponentUI : MonoBehaviour
{
    public TMP_InputField Input;
    public Image Arrow;

    private Vector2 startSizeInput;
    private RectTransform inputTranform;

    private void Start()
    {
        inputTranform = Input.GetComponent<RectTransform>();
        startSizeInput = inputTranform.sizeDelta;
        Arrow.DOFade(0, 0);


        Input.onSelect.AddListener(OnInputSelect);
        Input.onDeselect.AddListener(OnInputDeselect);
    }

    public void OnInputSelect(string content)
    {
        inputTranform.DOSizeDelta(startSizeInput * 1.05f, 0.3f);
        Arrow.DOFade(1, 0.2f);
    }

    public void OnInputDeselect(string content)
    {
        inputTranform.DOSizeDelta(startSizeInput, 0.3f);
        Arrow.DOFade(0, 0.2f);
    }
    public string GetText()
    {
        return Input.text;
    }
    public void SetString(string input)
    {
        Input.SetTextWithoutNotify(input);
    }
    public void ClearString()
    {
        Input.SetTextWithoutNotify("");
    }
}
