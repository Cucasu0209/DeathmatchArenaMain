using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharactorUserInput : MonoBehaviour
{
    public CharacterController2D charactor;
    public Vector2 currenDiractiom;

    private void Update()
    {
        if (charactor.photonView?.IsMine == false) return;
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

        if (Input.GetKeyDown(KeyCode.Alpha1)) charactor.SwitchWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) charactor.SwitchWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) charactor.SwitchWeapon(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) charactor.SwitchWeapon(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) charactor.SwitchWeapon(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) charactor.SwitchWeapon(5);

    }

    private void FixedUpdate()
    {
        if (charactor.photonView?.IsMine == false) return;
        charactor.Move(currenDiractiom);
    }
}
