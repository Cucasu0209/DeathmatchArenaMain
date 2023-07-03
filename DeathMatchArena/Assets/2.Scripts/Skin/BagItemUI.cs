using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagItemUI : MonoBehaviour
{
    public GameObject tick;
    public void SetOwned(bool isOwned)
    {
        if (isOwned)
        {
            GetComponent<Image>().color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.gray;
        }
    }
    public void SetEquipped(bool _isequip)
    {
        tick.SetActive(_isequip);
    }
}
