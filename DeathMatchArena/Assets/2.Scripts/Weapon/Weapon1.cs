using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Player = Photon.Realtime.Player;

public class Weapon1 : BaseWeapon
{
    public Collider2D MyCollider;
    public GameObject Trail;
    private string Q_EffectPrefabLink = "Effect/Weapon/Weapon1/Q_Weapon1Item";
    float currentSwordDmg;
    private void Start()
    {
        if (MyCollider == null) MyCollider = GetComponent<Collider2D>();
        MyCollider.enabled = false;
        Trail.SetActive(false);

    }
    public override void PerformNormal(CharacterController2D _character, Action<string> doAnimation, Action<float> WaitForNextAttack)
    {
        ownChar = _character;
        currentSwordDmg = props.Damage_Normal;
        doAnimation?.Invoke(props.AnimationAttack_Normal);
        WaitForNextAttack?.Invoke(props.TimePerform_Normal);

        if (IEPerformNormal != null) StopCoroutine(IEPerformNormal);
        IEPerformNormal = IPerformNormal(_character);
        StartCoroutine(IEPerformNormal);
    }
    public override void PerformE(CharacterController2D _character, Action<string> doAnimation, Action<float> WaitForNextAttack)
    {
        ownChar = _character;
        currentSwordDmg = props.Damage_Normal;
        doAnimation?.Invoke(props.AnimationAttack_E);
        WaitForNextAttack?.Invoke(props.TimePerform_E);

        if (IEPerform_E != null) StopCoroutine(IEPerform_E);
        IEPerform_E = IPerform_E(_character);
        StartCoroutine(IEPerform_E);
    }
    public override void PerformQ(CharacterController2D _character, Action<string> doAnimation, Action<float> FreezeChar)
    {
        ownChar = _character;
        currentSwordDmg = props.Damage_Q;
        doAnimation?.Invoke(props.AnimationAttack_Q);
        FreezeChar?.Invoke(props.TimePerform_Q);

        if (IEPerform_Q != null) StopCoroutine(IEPerform_Q);
        IEPerform_Q = IPerform_Q(_character);
        StartCoroutine(IEPerform_Q);
    }

    #region IEnumberator
    private IEnumerator IEPerformNormal;
    private IEnumerator IPerformNormal(CharacterController2D _character)
    {
        MyCollider.enabled = true;
        Trail.SetActive(true);
        yield return new WaitForSeconds(props.TimePerform_Normal);
        MyCollider.enabled = false;
        Trail.SetActive(false);

    }

    private IEnumerator IEPerform_E;
    private IEnumerator IPerform_E(CharacterController2D _character)
    {
        MyCollider.enabled = true;
        Trail.SetActive(true);
        yield return new WaitForSeconds(props.TimePerform_E);
        MyCollider.enabled = false;
        Trail.SetActive(false);
    }

    private IEnumerator IEPerform_Q;
    private IEnumerator IPerform_Q(CharacterController2D _character)
    {

        Trail.SetActive(true);
        WeaponItemQBackground Item = Resources.Load<WeaponItemQBackground>(Q_EffectPrefabLink);
        if (Item != null)
        {
            Item = Instantiate(Item, _character.skAnim.transform);
            Item.Show();

        }
        if (_character.photonView.IsMine) CameraController.Instance.ZoomIn();

        yield return new WaitForSeconds(props.TimePerform_Q * 0.77f - 0.3f);
        if (_character.photonView.IsMine) CameraController.Instance.ZoomOut();
        yield return new WaitForSeconds(0.3f);


        MyCollider.enabled = true;
        yield return new WaitForSeconds(props.TimePerform_Q * 0.23f);
        if (Item != null)
        {
            Item.Hide();
        }

        MyCollider.enabled = false;
        Trail.SetActive(false);

    }


    public bool CheckCanAttack(Player mine, Player otherPlayer)
    {
        return RoomController.Instance.GetTeam(mine) != RoomController.Instance.GetTeam(otherPlayer);
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CharacterController2D>() != null)
        {
            CharacterController2D otherPlayer = collision.gameObject.GetComponent<CharacterController2D>();
            if (CheckCanAttack(ownChar.photonView.Controller, otherPlayer.photonView.Controller))
            {
                TakeDamgeToPlayer(otherPlayer, currentSwordDmg);
            }
        }

    }

    private void TakeDamgeToPlayer(CharacterController2D player, float dmg)
    {
            player.TakeDamage((int)dmg);
    }
}
