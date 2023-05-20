using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/WeaponData", order = 1)]
public class WeaponProperties : ScriptableObject
{
    [Header("Animation Name")]
    public string AnimationAttack_Normal;
    public string AnimationAttack_E;
    public string AnimationAttack_Q;

    [Header("Damage")]
    public float Damage_Normal;
    public float Damage_E;
    public float Damage_Q;

    [Header("Count Down Time")]
    public float AbilityCooldown_E;
    public float AbilityCooldown_Q;

    [Header("Time Perform")]
    public float TimePerform_Normal;
    public float TimePerform_E;
    public float TimePerform_Q;
}
