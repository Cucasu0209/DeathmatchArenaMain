using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class RankItemUI : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Id;
    public TextMeshProUGUI elo;
    public Button Button;


    private void Start()
    {
        Name.SetText("???????????");
        Id.SetText("id: ???????????");
        elo.SetText("????");
    }

    public void Show(PlayerPlayfabInformation player, Action OnClick)
    {
        if (player == null)
        {
            Name.SetText("??????????");
            Id.SetText("Id: ??????????");

            elo.SetText("????");
        }
        else
        {
            Name.SetText(player.DisplayName);
            Id.SetText("Id: " + player.PlayFabId);
            elo.SetText(player.StatValue.ToString());

            Button.onClick.RemoveAllListeners();
            Button.onClick.AddListener(() => OnClick?.Invoke());
        }
    }
}
