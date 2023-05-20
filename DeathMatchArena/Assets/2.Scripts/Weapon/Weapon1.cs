using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon1 : BaseWeapon
{
    public override void PerformNormal(CharacterController2D _charecter, Action<string> doAnimation, Action<float> WaitForNextAttack)
    {
        doAnimation?.Invoke(props.AnimationAttack_Normal);
        WaitForNextAttack?.Invoke(props.TimePerform_Normal);
    }
    public override void PerformE(CharacterController2D _charecter, Action<string> doAnimation, Action<float> WaitForNextAttack)
    {
        doAnimation?.Invoke(props.AnimationAttack_E);
        WaitForNextAttack?.Invoke(props.TimePerform_E);
    }
    public override void PerformQ(CharacterController2D _charecter, Action<string> doAnimation, Action<float> FreezeChar)
    {
        doAnimation?.Invoke(props.AnimationAttack_Q);
        FreezeChar?.Invoke(props.TimePerform_Q);
    }
}
