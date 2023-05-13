using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Target;

    private void Update()
    {
        if (Target != null)
        {
            float newX = Mathf.Lerp(Target.position.x, transform.position.x, 0.01f);
            newX = Mathf.Clamp(newX, -17.3f, 17.3f);
            float newY = Mathf.Lerp(Target.position.y, transform.position.y, 0.01f);
            newY = Mathf.Clamp(newY, -10f, 10.3f);

            transform.position = new Vector3(newX, newY, transform.position.z);
        }
    }
}
