using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class PlayerUIItem : MonoBehaviour
{
    private PlayerPlayfabInformation myInfo;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Id;
    public Button btn;
    public Image Arrow;

    private void OnEnable()
    {
        Arrow.color = new Color(Arrow.color.r, Arrow.color.g, Arrow.color.b, 0);
        OtherPlayersController.OnPlayerFocusChange += OnPlayerFocusChange;
    }
    private void OnDisable()
    {
        OtherPlayersController.OnPlayerFocusChange -= OnPlayerFocusChange;
    }

    public void OnPlayerFocusChange()
    {
        if (myInfo == null) return;
        if (myInfo.PlayFabId ==
            OtherPlayersController.Instance.GetIdFocus())
        {
            Arrow.DOKill();
            Arrow.DOFade(1, 0.2f);
            btn.transform.DOKill();
            btn.transform.DOScale(Vector3.one * 1.1f, 0.2f);
        }
        else
        {
            Arrow.DOKill();
            Arrow.DOFade(0, 0.2f);
            btn.transform.DOKill();
            btn.transform.DOScale(Vector3.one, 0.2f);
        }
    }


    public void SetInformation(PlayerPlayfabInformation player)
    {
        myInfo = player;
        Name.SetText(player.DisplayName);
        Id.SetText("Id:" + player.PlayFabId);
        btn.onClick.AddListener(() => FriendUI.Instance.ShowDetail(player));
    }
    public void ClearInformation()
    {
        btn.onClick.RemoveAllListeners();

    }
}
