using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PlayerRewardUI : MonoBehaviour
{
    public TextMeshProUGUI Name;
    public TextMeshProUGUI EloReward;
    public TextMeshProUGUI CoinReward;

    private void Start()
    {
        EloReward.transform.parent.localScale = Vector3.zero;
        CoinReward.transform.parent.localScale = Vector3.zero;
    }

    public void SetInfor(PlayerReward reward)
    {
        Name.SetText(NetworkController_PUN.Instance.GetPlayerProperties(reward.owner).playerName);
        if (reward.CoinReward < 0)
        {
            CoinReward.color = Color.red;
            CoinReward.SetText(reward.CoinReward.ToString());
        }
        else
        {
            CoinReward.color = Color.green;
            CoinReward.SetText("+ " + reward.CoinReward);
        }

        if (reward.EloReward < 0)
        {
            EloReward.color = Color.red;
            EloReward.SetText(reward.EloReward.ToString());
        }
        else
        {
            EloReward.color = Color.green;
            EloReward.SetText("+ " + reward.EloReward);
        }

        EloReward.transform.parent.DOScale(1, 0.3f).SetDelay(1);
        CoinReward.transform.parent.DOScale(1, 0.3f).SetDelay(1);
    }
}
