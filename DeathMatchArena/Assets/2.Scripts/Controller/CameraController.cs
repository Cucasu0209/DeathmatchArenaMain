using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public Transform Target;
    public static CameraController Instance;
    private Camera _camera;


    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void ZoomIn()
    {
        _camera.DOOrthoSize(3, 0.3f);
    }

    public void ZoomOut()
    {
        _camera.DOOrthoSize(8, 0.3f);
    }

    private void FixedUpdate()
    {
        if (Target != null)
        {
            float newX = Mathf.Lerp(Target.position.x, transform.position.x, 0.97f);
            //newX = Mathf.Clamp(newX, -17.3f, 17.3f);
            float newY = Mathf.Lerp(Target.position.y + 2, transform.position.y, 0.97f);
            //newY = Mathf.Clamp(newY, -10f, 10.3f);

            transform.position = new Vector3(newX, newY, transform.position.z);
        }
    }
    private void Awake()
    {
        Time.fixedDeltaTime = 0.005f;
        Instance = this;
    }

}
