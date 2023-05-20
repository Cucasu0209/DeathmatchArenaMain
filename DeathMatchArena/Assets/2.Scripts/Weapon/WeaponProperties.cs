using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Data/WeaponData", order = 1)]
public class WeaponProperties : ScriptableObject
{
    public string AnimationAttack_Normal;
    public string AnimationAttack_E;
    public string AnimationAttack_Q;
    public float Damage_Normal;
    public float Damage_E;
    public float Damage_Q;
    public float AbilityCooldown_E;
    public float AbilityCooldown_Q;
}
