using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitBoxId
{
    None,
    Head,
}

public class HitBox : MonoBehaviour
{
    public HitBoxId hitBoxID = HitBoxId.None;
    public Health health;

    public void OnRaycastHit(WeaponBase weapon, Vector3 direction) 
    {
        if (hitBoxID == HitBoxId.Head)
        {
            health.TakeDamage(weapon.damage * 15, direction);
        }
        else
            health.TakeDamage(weapon.damage, direction);
    }

    //public void OnRaycastHitMelee(MeleeWeapon meleeWeapon)
    //{
    //    health.TakeDamage
    //}
}
