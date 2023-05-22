using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorUserInput : MonoBehaviour
{
    public CharacterController2D charactor;
    public Vector2 currenDiractiom;

    private void Update()
    {
        if (charactor.photonView.IsMine == false) return;
        currenDiractiom = Vector2.zero;
        if (Input.GetKey(KeyCode.D)) currenDiractiom += Vector2.right;
        if (Input.GetKey(KeyCode.A)) currenDiractiom += Vector2.left;
     

        if (Input.GetKeyDown(KeyCode.Space)) charactor.Jump();

        if (Input.GetKey(KeyCode.W)) currenDiractiom += Vector2.up;
        if (Input.GetKey(KeyCode.S)) currenDiractiom += Vector2.down;
        if (Input.GetMouseButtonDown(1)) charactor.Dash(currenDiractiom);
        if (Input.GetMouseButtonDown(0)) charactor.AttackNormal();
        if (Input.GetKeyDown(KeyCode.E)) charactor.AttackE();
        if (Input.GetKeyDown(KeyCode.Q)) charactor.AttackQ();
    }

    private void FixedUpdate()
    {
        if (charactor.photonView.IsMine == false) return;
        charactor.Move(currenDiractiom);
    }
}
