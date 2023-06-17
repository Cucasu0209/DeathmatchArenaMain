using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Player = Photon.Realtime.Player;

public class Weapon3 : BaseWeapon
{

    private string Normal_EffectPrefabLink = "Effect/Weapon/Weapon3/Normal_Weapon3Item";
    private string Q_EffectPrefabLink = "Effect/Weapon/Weapon3/Q_Weapon3Item";
    float currentSwordDmg;
    private void Start()
    {

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

        yield return new WaitForSeconds(props.TimePerform_Normal * 1.4f / 4);
        Weapon3Normal Item = Resources.Load<Weapon3Normal>(Normal_EffectPrefabLink);
        if (Item != null)
        {
            Vector2 dir = _character.CharactorTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Weapon3Normal Itemr = Instantiate(Item, _character.handRight.position + Vector3.down * 0.06f, Quaternion.identity);
            Weapon3Normal Iteml = Instantiate(Item, _character.handLeft.position + Vector3.down * 0.06f, Quaternion.identity);
            Itemr.Setup(dir, (_item, objHit) =>
            {
                if (objHit == gameObject) return;
                CharacterController2D _char = objHit.GetComponent<CharacterController2D>();
                if (_char != null)
                {
                    if (_char == _character) return;
                    TakeDamgeToPlayer(_char, props.Damage_Normal);
                }

                _item.DestroySelf();
            });
            Iteml.Setup(dir, (_item, objHit) =>
            {
                if (objHit == gameObject) return;
                CharacterController2D _char = objHit.GetComponent<CharacterController2D>();
                if (_char != null)
                {
                    if (_char == _character) return;
                    TakeDamgeToPlayer(_char, props.Damage_Normal);
                }

                _item.DestroySelf();
            });
            Itemr.Fly();
            Iteml.Fly();
        }
    }

    private IEnumerator IEPerform_E;
    private IEnumerator IPerform_E(CharacterController2D _character)
    {

        yield return new WaitForSeconds(props.TimePerform_Normal * 1 / 2);
        Weapon3Normal Item = Resources.Load<Weapon3Normal>(Normal_EffectPrefabLink);
        if (Item != null)
        {
            Vector2 dir = _character.CharactorTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Weapon3Normal Itemr = Instantiate(Item, _character.handRight.position, Quaternion.identity);
            Weapon3Normal Iteml = Instantiate(Item, _character.handLeft.position, Quaternion.identity);
            Itemr.Setup(dir, (_item, objHit) =>
            {
                if (objHit == gameObject) return;
                CharacterController2D _char = objHit.GetComponent<CharacterController2D>();
                if (_char != null)
                {
                    if (_char == _character) return;
                    TakeDamgeToPlayer(_char, props.Damage_Normal);
                }

                _item.DestroySelf();
            });
            Iteml.Setup(dir, (_item, objHit) =>
            {
                if (objHit == gameObject) return;
                CharacterController2D _char = objHit.GetComponent<CharacterController2D>();
                if (_char != null)
                {
                    if (_char == _character) return;
                    TakeDamgeToPlayer(_char, props.Damage_Normal);
                }

                _item.DestroySelf();
            });
            Itemr.Fly();
            Iteml.Fly();
        }

    }

    private IEnumerator IEPerform_Q;
    private IEnumerator IPerform_Q(CharacterController2D _character)
    {
        if (_character.photonView.IsMine) CameraController.Instance.ZoomIn();
        WeaponItemQBackground BGItem = Resources.Load<WeaponItemQBackground>(Q_EffectPrefabLink);
        if (BGItem != null)
        {
            BGItem = Instantiate(BGItem, _character.skAnim.transform);
            BGItem.Show();

        }
        yield return new WaitForSeconds(props.TimePerform_Q);




        if (BGItem != null)
        {
            BGItem.Hide();
        }
        if (_character.photonView.IsMine) CameraController.Instance.ZoomOut();

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
