using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerDetailUI : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Id;

    public void DisplayInfomation(PlayerPlayfabInformation player)
    {
        Name.SetText(player.DisplayName);
        Id.SetText("Id: " + player.PlayFabId);
    }
}
