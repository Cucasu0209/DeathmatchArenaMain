using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Player = Photon.Realtime.Player;

public class Weapon2 : BaseWeapon
{

    public SpriteRenderer WeaponRenderer;
    public Collider2D MyCollider;
    public GameObject Trail;
    private string Q_EffectPrefabLink = "Effect/Weapon/Weapon2/Q_Weapon2Item";
    private string E_EffectPrefabLink = "Effect/Weapon/Weapon2/E_Weapon2Item";
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

        WeaponRenderer.DOColor(new Color(0.8f, 0.4f, 0.4f), 0.2f);

        float R = 1;
        int maxSpawner = 8;
        for (int i = 0; i < maxSpawner; i++)
        {
            Weapon2ItemE Item = Resources.Load<Weapon2ItemE>(E_EffectPrefabLink);
            if (Item != null)
            {
                float angle = 180 - ((360 / maxSpawner) * i);
                Vector3 newPos = new Vector3(R * Mathf.Cos(angle * Mathf.Deg2Rad), R * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                Item = Instantiate(Item, _character.transform.position + Vector3.up * 1.5f + newPos,
                    Quaternion.identity);
                Item.Fly(newPos, (_item, objHit) =>
                {
                    if (objHit == gameObject) return;
                    CharacterController2D _char = objHit.GetComponent<CharacterController2D>();
                    if (_char != null)
                    {
                        if (_char == _character) return;
                        TakeDamgeToPlayer(_char, props.Damage_E);
                    }

                    _item.DestroySelf();
                });
            }
            yield return new WaitForSeconds(0.02f);
        }


        WeaponRenderer.DOColor(Color.white, 0.2f);

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
        WeaponRenderer.DOColor(Color.red, props.TimePerform_Q * 0.77f).SetEase(Ease.Linear);
        if (_character.photonView.IsMine) CameraController.Instance.ZoomIn();

        yield return new WaitForSeconds(props.TimePerform_Q * 0.77f - 0.3f);
        if (_character.photonView.IsMine) CameraController.Instance.ZoomOut();
        yield return new WaitForSeconds(0.3f);


        if (Item != null)
        {
            Item.Hide();
        }
        _character.WeaponHolder.DOScale(5, props.TimePerform_Q * 0.23f).SetEase(Ease.Linear);

        MyCollider.enabled = true;
        yield return new WaitForSeconds(props.TimePerform_Q * 0.23f);


        _character.WeaponHolder.DOScale(1, 0.2f);
        WeaponRenderer.DOColor(Color.white, 0.2f).SetEase(Ease.Linear);


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
