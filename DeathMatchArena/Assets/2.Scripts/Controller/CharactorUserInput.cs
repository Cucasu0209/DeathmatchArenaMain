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

        GamePlayController.instance.SetFill(Mathf.Clamp(charactor.timeE, 0, charactor.MaxtimeE) / charactor.MaxtimeE,
           Mathf.Clamp(charactor.timeQ, 0, charactor.MaxtimeQ) / charactor.MaxtimeQ);

        currenDiractiom = Vector2.zero;
        if (Input.GetKey(KeyCode.D)) currenDiractiom += Vector2.right;
        if (Input.GetKey(KeyCode.A)) currenDiractiom += Vector2.left;


        if (Input.GetKeyDown(KeyCode.Space)) charactor.Jump();
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
    public void UIEvent_OnleftHold()
    {
        currenDiractiom += Vector2.right;
    }
    public void UIEvent_OnRightHold()
    {
        currenDiractiom += Vector2.left;
    }

    public void UIEvent_OnQClick()
    {
        charactor.AttackQ();
    }

    public void UIEvent_OnEClick()
    {
        charactor.AttackE();
    }
    public void UIEvent_OnAttackClick()
    {
        charactor.AttackNormal();
    }
    public void UIEvent_OnJumpClick()
    {
        charactor.Jump();
    }
    public void UIEvent_OnDashClick()
    {
        charactor.Dash(currenDiractiom);
    }
    private void FixedUpdate()
    {
        if (charactor.photonView?.IsMine == false) return;
        charactor.Move(currenDiractiom);
    }

    private void OnEnable()
    {
        GamePlayController.OnleftHold += UIEvent_OnleftHold;
        GamePlayController.OnRightHold += UIEvent_OnRightHold;
        GamePlayController.OnQClick += UIEvent_OnQClick;
        GamePlayController.OnEClick += UIEvent_OnEClick;
        GamePlayController.OnDashClick += UIEvent_OnDashClick;
        GamePlayController.OnAttackClick += UIEvent_OnAttackClick;
        GamePlayController.OnJumpClick += UIEvent_OnJumpClick;
    }
    private void OnDisable()
    {
         GamePlayController.OnleftHold -= UIEvent_OnleftHold;
        GamePlayController.OnRightHold -= UIEvent_OnRightHold;
        GamePlayController.OnQClick -= UIEvent_OnQClick;
        GamePlayController.OnEClick -= UIEvent_OnEClick;
        GamePlayController.OnDashClick -= UIEvent_OnDashClick;
        GamePlayController.OnAttackClick -= UIEvent_OnAttackClick;
        GamePlayController.OnJumpClick -= UIEvent_OnJumpClick;
    }
}
