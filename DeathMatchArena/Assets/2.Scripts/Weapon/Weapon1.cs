using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon1 : BaseWeapon
{
    public override void PerformNormal(CharacterController2D _charecter, Action<string> doAnimation)
    {
        doAnimation?.Invoke(props.AnimationAttack_Normal);
    }
    public override void PerformE(CharacterController2D _charecter, Action<string> doAnimation)
    {
        doAnimation?.Invoke(props.AnimationAttack_E);
    }
    public override void PerformQ(CharacterController2D _charecter, Action<string> doAnimation)
    {
        doAnimation?.Invoke(props.AnimationAttack_Q);
    }
}
