using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Weapon2ItemE : MonoBehaviour
{
    private Action<Weapon2ItemE, GameObject> OnHitSomthing;
    private Vector2 myDir = Vector2.zero;
    private float speed = 30;
    private void Start()
    {
        Destroy(gameObject, 5);
    }
    public void Fly(Vector2 dir, Action<Weapon2ItemE, GameObject> OnHitSth)
    {
        OnHitSomthing = OnHitSth;
        myDir = dir.normalized;
        float angle = Mathf.Atan2(myDir.y, myDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Reapon2 Item Hit " + collision.gameObject.name);
        OnHitSomthing?.Invoke(this, collision.gameObject);
    }
    public void DestroySelf()
    {
        ParticleSystem effect = Resources.Load<ParticleSystem>("Effect/EffectMoon");
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
        if (myDir.magnitude > 0)
        {
            transform.position += (Vector3)myDir * Time.deltaTime * speed;

        }

    }
}
