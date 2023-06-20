using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class Weapon1ItemQ : BaseWeaponItem
{
    private string Q_SegmentLink = "Effect/Weapon/Weapon1/Q_Weapon1Segment";
    private Action<Weapon1ItemQ, GameObject> OnHitSomthing;
    private Vector2 myDir = Vector2.zero;
    private float speed = 30;
    private bool flying = false;
    private int index = 0;
    private const int maxindex = 4;
    private void Start()
    {
        Destroy(gameObject, 5);
    }
    public void Setup(int _index, Vector2 dir, Action<Weapon1ItemQ, GameObject> OnHitSth)
    {

        index = _index;
        OnHitSomthing = OnHitSth;
        myDir = dir.normalized;
    }
    public void Fly()
    {
        transform.DORotate(Vector3.forward * 300, 0.03f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

        Invoke(nameof(SpreadOut), 0.2f);
        flying = true;
    }
    public void SpreadOut()
    {
        if (index > maxindex)
        {
            return;
        }
        float R = 0.1f;
        int maxSpawner = 3;

        Weapon1ItemQ Item = Resources.Load<Weapon1ItemQ>(Q_SegmentLink);
        if (Item != null)
        {
            for (int i = 0; i < maxSpawner; i++)
            {
                float angle = 270 + (index * 40) - ((360 / maxSpawner) * i);
                Vector3 newPos = new Vector3(R * Mathf.Cos(angle * Mathf.Deg2Rad), R * Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                Weapon1ItemQ NewItem = Instantiate(Item, transform.position,
                    Quaternion.identity);
                NewItem.transform.localScale = Vector3.one / (index * 1.3f + 1);
                NewItem.Setup(index + 1, newPos, (_item, objHit) =>
                {
                    OnHitSomthing?.Invoke(_item, objHit);
                });
                NewItem.Fly();
            }
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (index <= 1) return;
        Debug.Log("Reapon5 Item Hit " + collision.gameObject.name);
        OnHitSomthing?.Invoke(this, collision.gameObject);
    }

    public void DestroySelf()
    {
        ParticleSystem effect = Resources.Load<ParticleSystem>("Effect/EffectDot");
        if (effect != null)
        {
            effect = Instantiate(effect, transform.position, Quaternion.identity);
            effect.Emit(5);
            Destroy(effect.gameObject, 1);
        }

        Destroy(gameObject);
    }

    private void Update()
    {
        if (myDir.magnitude > 0 && flying)
        {
            transform.position += (Vector3)myDir * Time.deltaTime * speed;

        }
        else
        {
            if (transform.parent != null)
            {
                transform.localPosition = Vector3.zero;
            }
        }

    }
}
