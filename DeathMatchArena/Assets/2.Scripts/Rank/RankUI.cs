using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankUI : MonoBehaviour
{
    public PlayerDetailUI detail;
    public RankItemUI[] top;
    public RankItemUI mine;
    public TextMeshProUGUI MyIndex;
    // Start is called before the first frame update

    private void OnEnable()
    {
        detail.DisplayInfomation(null);

        PlayfabController.Instance.GetEloLeaderboardPlayfab(null);
        //if (OtherPlayersController.Instance.currentFocus != null)
        //{
        //    detail.DisplayInfomation(OtherPlayersController.Instance.currentFocus);
        //}
        Show();

        PlayerData.OnEloChange += Show;
        PlayerData.OnEloLeaderboardChange += Show;
    }

    private void OnDisable()
    {
        PlayerData.OnEloChange -= Show;
        PlayerData.OnEloLeaderboardChange -= Show;
    }


    public void Show()
    {
        MyIndex.SetText("6+");
        for (int i = 0; i <= 4; i++)
        {
            if (PlayerData.GetEloLeaderboard(i) != null)
            {
                if (PlayerData.GetEloLeaderboard(i).PlayFabId == PlayerData.GetId())
                {
                    MyIndex.SetText((i + 1).ToString());
                }
            }
            int j = i;
            top[j].Show(PlayerData.GetEloLeaderboard(j), () =>
            {
                detail.DisplayInfomation(PlayerData.GetEloLeaderboard(j));
            });
        }
        mine.Show(PlayerPlayfabInformation.GetMyInfomation(), () =>
        {
            detail.DisplayInfomation(PlayerPlayfabInformation.GetMyInfomation());
        });
    }


}
