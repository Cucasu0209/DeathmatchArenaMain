using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon5ItemNormalE : MonoBehaviour
{
    private Action<Weapon5ItemNormalE, GameObject> OnHitSomthing;
    private Vector2 myDir = Vector2.zero;
    private float speed = 50;
    private bool flying = false;
    private void Start()
    {
        Destroy(gameObject, 5);
    }
    public void Setup(Vector2 dir, Action<Weapon5ItemNormalE, GameObject> OnHitSth)
    {
        OnHitSomthing = OnHitSth;
        myDir = dir.normalized;
        float angle = Mathf.Atan2(myDir.y, myDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
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

    }
}
