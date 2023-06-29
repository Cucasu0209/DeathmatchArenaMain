using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData", order = 2)]
public class ItemData : ScriptableObject
{
    public float Attack;
    public float Defense;
    public float Speed;
}
