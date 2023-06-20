using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Doozy.Runtime.UIManager.Containers;

public class EndGameControllerUI : MonoBehaviour
{
    [Header("Team 1")]
    public UIView ViewTeam1;
    public TextMeshProUGUI NamePlayer1;
    public TextMeshProUGUI NamePlayer2;
    public TextMeshProUGUI WinTeam1;
    public TextMeshProUGUI LoseTeam1;
    public TextMeshProUGUI DrawTeam1;

    [Header("Team 2")]
    public UIView ViewTeam2;
    public TextMeshProUGUI NamePlayer3;
    public TextMeshProUGUI NamePlayer4;
    public TextMeshProUGUI WinTeam2;
    public TextMeshProUGUI LoseTeam2;
    public TextMeshProUGUI DrawTeam2;

    [Header("Reward View")]
    public UIView RewardView;
    public PlayerRewardUI rewardPlayer1;
    public PlayerRewardUI rewardPlayer2;
    public PlayerRewardUI rewardPlayer3;
    public PlayerRewardUI rewardPlayer4;


    private void Start()
    {
        SetupDefault();






        DOVirtual.DelayedCall(1, () =>
        {
            ShowFirstResult();
        });

    }

    private void SetupDefault()
    {
        //team1
        WinTeam1.transform.localScale = Vector3.zero;
        LoseTeam1.transform.localScale = Vector3.zero;
        DrawTeam1.transform.localScale = Vector3.zero;
        WinTeam1.gameObject.SetActive(true);
        LoseTeam1.gameObject.SetActive(true);
        DrawTeam1.gameObject.SetActive(true);

        //team2
        WinTeam2.transform.localScale = Vector3.zero;
        LoseTeam2.transform.localScale = Vector3.zero;
        DrawTeam2.transform.localScale = Vector3.zero;
        WinTeam2.gameObject.SetActive(true);
        LoseTeam2.gameObject.SetActive(true);
        DrawTeam2.gameObject.SetActive(true);
    }
    public void ShowFirstResult()
    {
        string name1 = RoomController.Instance.GetNamePlayer(0);
        string name2 = RoomController.Instance.GetNamePlayer(1);
        string name3 = RoomController.Instance.GetNamePlayer(2);
        string name4 = RoomController.Instance.GetNamePlayer(3);

        DisplayFirstResult(name1, name2, name3, name4, RoomController.Instance.GetGameResult());
    }

    private void DisplayFirstResult(string player1, string player2, string player3, string player4, GamePlayResultEnum result)
    {
        NamePlayer1.SetText(player1);
        NamePlayer2.SetText(player2);
        NamePlayer3.SetText(player3);
        NamePlayer4.SetText(player4);

        switch (result)
        {
            case GamePlayResultEnum.Team1Win:
                WinTeam1.transform.DOScale(1, 0.4f).SetDelay(0.6f);
                LoseTeam2.transform.DOScale(1, 0.4f).SetDelay(0.6f);
                break;
            case GamePlayResultEnum.Team2Win:
                WinTeam2.transform.DOScale(1, 0.4f).SetDelay(0.6f);
                LoseTeam1.transform.DOScale(1, 0.4f).SetDelay(0.6f);
                break;
            case GamePlayResultEnum.Draw:
                DrawTeam1.transform.DOScale(1, 0.4f).SetDelay(0.6f);
                DrawTeam2.transform.DOScale(1, 0.4f).SetDelay(0.6f);
                break;
        }

        ViewTeam1.Show();
        ViewTeam2.Show();
    }

    public void DisplaySecondResult()
    {
        ViewTeam1.Hide();
        ViewTeam2.Hide();
        RewardView.Show();

        rewardPlayer1.SetInfor(RoomController.Instance.currentGameResult.player1Reward);
        rewardPlayer2.SetInfor(RoomController.Instance.currentGameResult.player2Reward);
        rewardPlayer3.SetInfor(RoomController.Instance.currentGameResult.player3Reward);
        rewardPlayer4.SetInfor(RoomController.Instance.currentGameResult.player4Reward);
    }
}

public enum GamePlayResultEnum
{
    Team1Win, Team2Win, Draw, NotCompleteYet
}
