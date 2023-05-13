using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorUserInput : MonoBehaviour
{
    public CharactorController charactor;
    public Vector2 currenDiractiom;

    private void Update()
    {
        currenDiractiom = Vector2.zero;
        if (Input.GetKey(KeyCode.D)) currenDiractiom += Vector2.right;
        if (Input.GetKey(KeyCode.A)) currenDiractiom += Vector2.left;
        charactor.Move(currenDiractiom);

        if (Input.GetKey(KeyCode.Space)) charactor.Jump();
    }
}
