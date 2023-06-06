using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
public class Weapon6ItemNormalE : MonoBehaviour
{
    private Action<Weapon6ItemNormalE, GameObject> OnHitSomthing;
    private Vector2 myDir = Vector2.zero;
    private float speed = 50;
    private bool flying = false;
    private void Start()
    {
        Destroy(gameObject, 5);
        transform.DORotate(Vector3.forward * 300, 0.05f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }
    public void Setup(Vector2 dir, Action<Weapon6ItemNormalE, GameObject> OnHitSth)
    {
        OnHitSomthing = OnHitSth;
        myDir = dir.normalized;
    }
    public void Fly()
    {
        flying = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Reapon5 Item Hit " + collision.gameObject.name);
        OnHitSomthing?.Invoke(this, collision.gameObject);
    }
    public void DestroySelf()
    {
        ParticleSystem effect = Resources.Load<ParticleSystem>("Effect/EffectGear");
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

    }
}
