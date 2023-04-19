using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class AuthenticationComponentUI : MonoBehaviour
{
    [HideInInspector] public string allowedCharacters = "abcdefghijklmnopqrstuvwxyzQWERTYUIOPASDFGHJKLZXCVBNM._@:1234567890";
    public TMP_InputField Input;
    public Image Arrow;

    private Vector2 startSizeInput;
    private RectTransform inputTranform;

    private void Start()
    {
        inputTranform = Input.GetComponent<RectTransform>();
        startSizeInput = inputTranform.sizeDelta;
        Arrow.DOFade(0, 0);

        Input.onValueChanged.AddListener(OnInputFieldValueChanged);
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
    private void OnInputFieldValueChanged(string value)
    {
        string filteredValue = new string(value.Where(c => allowedCharacters.Contains(c)).ToArray());
        if (filteredValue != value)
        {
            Input.text = filteredValue;
        }
    }
    public string GetText()
    {
        return Input.text;
    }
    public void SetText(string input)
    {
        Input.SetTextWithoutNotify(input);
    }
    public void ClearString()
    {
        Input.SetTextWithoutNotify("");
    }
}
