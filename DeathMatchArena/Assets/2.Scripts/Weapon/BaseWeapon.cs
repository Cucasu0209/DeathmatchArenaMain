using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public abstract class BaseWeapon : MonoBehaviour
{
    protected CharacterController2D ownChar;
    public WeaponProperties props;

    public abstract void PerformNormal(CharacterController2D _charecter, Action<string> doAnimation, Action<float> WaitForNextAttack);
    public abstract void PerformE(CharacterController2D _charecter, Action<string> doAnimation, Action<float> WaitForNextAttack);
    public abstract void PerformQ(CharacterController2D _charecter, Action<string> doAnimation, Action<float> FreezeChar);

}
