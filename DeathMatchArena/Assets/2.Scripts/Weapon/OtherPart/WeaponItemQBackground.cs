using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class WeaponItemQBackground : MonoBehaviour
{
    public SpriteRenderer _renderer;
    private void Awake()
    {
        _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, 0);
    }

    public void Show()
    {
        _renderer.DOFade(1, 0.3f);
    }
    public void Hide()
    {
        _renderer.DOFade(0, 0.3f).OnComplete(()=>
        {
            Destroy(gameObject);
        });
    }
}
