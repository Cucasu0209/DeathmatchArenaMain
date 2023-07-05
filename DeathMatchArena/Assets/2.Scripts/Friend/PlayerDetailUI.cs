using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerDetailUI : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Id;
    public PlayerReviewUI review;

    private string currentid = "";

    public void DisplayInfomation(PlayerPlayfabInformation player)
    {
        if (player == null)
        {
            Name.SetText("??????????");
            Id.SetText("Id: ??????????");

            review.HideAll();
        }
        else
        {
            Name.SetText(player.DisplayName);
            Id.SetText("Id: " + player.PlayFabId);

            currentid = player.PlayFabId;

            review.currentIdShowned = currentid;
            review.Show();
        }


    }
}
