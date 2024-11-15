using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/WeaponTypeDatabase")]
public class WeaponTypeDatabase : ScriptableObject
{
    public List<string> weaponTypes = new List<string>();
}
