using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCanvas : MonoBehaviour
{
    public static PopupCanvas Instance;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
