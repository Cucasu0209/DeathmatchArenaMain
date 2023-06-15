using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using Player = Photon.Realtime.Player;

public class Weapon4 : BaseWeapon
{
    public Transform PosSpawnBullet;
    private string Normal_EffectPrefabLink = "Effect/Weapon/Weapon4/Normal_Weapon4Item";
    private string Q_EffectPrefabLink = "Effect/Weapon/Weapon4/Q_Weapon4Item";
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

        yield return new WaitForSeconds(props.TimePerform_Normal * 1 / 4);
        Weapon4Normal Item = Resources.Load<Weapon4Normal>(Normal_EffectPrefabLink);
        if (Item != null)
        {
            Vector2 dir = _character.CharactorTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Item = Instantiate(Item, PosSpawnBullet.position, Quaternion.identity);
            Item.Setup(dir, (_item, objHit) =>
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

            Item.Fly();
        }
    }

    private IEnumerator IEPerform_E;
    private IEnumerator IPerform_E(CharacterController2D _character)
    {

        yield return new WaitForSeconds(props.TimePerform_Normal * 1 / 4);
        Weapon4Normal Item = Resources.Load<Weapon4Normal>(Normal_EffectPrefabLink);
        if (Item != null)
        {
            Vector2 dir = _character.CharactorTransform.localScale.x > 0 ? Vector2.right : Vector2.left;
            Item = Instantiate(Item, PosSpawnBullet.position, Quaternion.identity);
            Item.Setup(dir, (_item, objHit) =>
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

            Item.Fly();
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
        yield return new WaitForSeconds(props.TimePerform_Q / 4);
        float R = 1.8f;
        int maxSpawner = 30;

        Weapon4Normal Item = Resources.Load<Weapon4Normal>(Normal_EffectPrefabLink);
        List<Weapon4Normal> newListItem = new List<Weapon4Normal>();
        if (Item != null)
        {
            for (int i = 0; i < maxSpawner; i++)
            {
                float angle = 180 - ((360 / maxSpawner) * i);
                Vector3 newPos = new Vector3(R * Mathf.Cos(angle * Mathf.Deg2Rad), R * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                Weapon4Normal NewItem = Instantiate(Item, _character.transform.position + Vector3.up * 1.1f + newPos,
                    Quaternion.identity);
                NewItem.Setup(newPos, (_item, objHit) =>
                {
                    if (objHit == gameObject) return;
                    if (objHit.layer == 8) return;
                    CharacterController2D _char = objHit.GetComponent<CharacterController2D>();
                    if (_char != null)
                    {
                        if (_char == _character) return;
                        TakeDamgeToPlayer(_char, props.Damage_Q);
                    }

                    _item.DestroySelf();
                });
                newListItem.Add(NewItem);

                yield return new WaitForSeconds(props.TimePerform_Q / 2 / maxSpawner);
            }
        }
        yield return new WaitForSeconds(props.TimePerform_Q / 8);
        foreach (var item in newListItem)
        {
            item.Fly();
        }

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

    float lastTimeTakeDamge = 0;
    private void TakeDamgeToPlayer(CharacterController2D player, float dmg)
    {
        if (Time.time - lastTimeTakeDamge >= 0.3f)
        {
            player.TakeDamage((int)dmg);
            lastTimeTakeDamge = Time.time;
        }

    }
}
