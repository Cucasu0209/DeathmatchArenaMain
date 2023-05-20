using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Weapon2 : BaseWeapon
{
    public SpriteRenderer WeaponRenderer;
    public Collider2D MyCollider;
    public GameObject Trail;
    public Transform PosSpawnE;
    private string Q_EffectPrefabLink = "Effect/Weapon/Weapon2/Q_Weapon2Item";
    private string E_EffectPrefabLink = "Effect/Weapon/Weapon2/E_Weapon2Item";
    private void Start()
    {
        if (MyCollider == null) MyCollider = GetComponent<Collider2D>();
        MyCollider.enabled = false;
        Trail.SetActive(false);
    }
    public override void PerformNormal(CharacterController2D _character, Action<string> doAnimation, Action<float> WaitForNextAttack)
    {
        doAnimation?.Invoke(props.AnimationAttack_Normal);
        WaitForNextAttack?.Invoke(props.TimePerform_Normal);

        if (IEPerformNormal != null) StopCoroutine(IEPerformNormal);
        IEPerformNormal = IPerformNormal(_character);
        StartCoroutine(IEPerformNormal);
    }
    public override void PerformE(CharacterController2D _character, Action<string> doAnimation, Action<float> WaitForNextAttack)
    {
        doAnimation?.Invoke(props.AnimationAttack_E);
        WaitForNextAttack?.Invoke(props.TimePerform_E);

        if (IEPerform_E != null) StopCoroutine(IEPerform_E);
        IEPerform_E = IPerform_E(_character);
        StartCoroutine(IEPerform_E);
    }
    public override void PerformQ(CharacterController2D _character, Action<string> doAnimation, Action<float> FreezeChar)
    {
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

        float t = 0;
        WeaponRenderer.DOColor(new Color(0.8f, 0.4f, 0.4f), 0.2f);
        while (t < props.TimePerform_E)
        {


            Weapon2ItemE Item = Resources.Load<Weapon2ItemE>(E_EffectPrefabLink);
            if (Item != null)
            {
                Item = Instantiate(Item, PosSpawnE.position, Quaternion.identity);
                Item.Fly(PosSpawnE.up, (a) => Debug.Log(a.name));
            }
            yield return new WaitForSeconds(0.06f);
            t += 0.06f;
        }
        WeaponRenderer.DOColor(Color.white, 0.2f);

        MyCollider.enabled = false;
        Trail.SetActive(false);

    }

    private IEnumerator IEPerform_Q;
    private IEnumerator IPerform_Q(CharacterController2D _character)
    {
        MyCollider.enabled = true;
        Trail.SetActive(true);

        Weapon2ItemQ Item = Resources.Load<Weapon2ItemQ>(Q_EffectPrefabLink);
        if (Item != null)
        {
            Item = Instantiate(Item, _character.skAnim.transform);
            Item.Show();

        }
        WeaponRenderer.DOColor(Color.red, props.TimePerform_Q * 0.77f).SetEase(Ease.Linear);

        yield return new WaitForSeconds(props.TimePerform_Q * 0.77f);
        if (Item != null)
        {
            Item.Hide();
        }
        transform.DOScale(5, props.TimePerform_Q * 0.23f).SetEase(Ease.Linear);
        yield return new WaitForSeconds(props.TimePerform_Q * 0.23f);

        transform.DOScale(1, 0.2f);
        WeaponRenderer.DOColor(Color.white, 0.2f).SetEase(Ease.Linear);


        MyCollider.enabled = false;
        Trail.SetActive(false);

    }
    #endregion
}
