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

        if (Input.GetKeyDown(KeyCode.Space)) charactor.Jump();

        if (Input.GetKey(KeyCode.W)) currenDiractiom += Vector2.up;
        if (Input.GetKey(KeyCode.S)) currenDiractiom += Vector2.down;
        if (Input.GetMouseButtonDown(1)) charactor.Dash(currenDiractiom);
    }
}
