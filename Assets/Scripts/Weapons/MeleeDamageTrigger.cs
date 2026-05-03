using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class MeleeDamageTrigger : MonoBehaviour
{
    public MeleeWeapon weapon;

    private HashSet<Collider> _processedColliders = new HashSet<Collider>(15);
    private bool _isReset = false;

    public void OnTriggerEnter(Collider other)
    {
        if (!weapon.CanAttack() && _isReset)
        {
            var hitBox = other.GetComponent<HitBox>();
            if (hitBox != null)
            {
                if (_processedColliders.Contains(other)) return;
                _processedColliders.Add(other);

                _isReset = false;
                hitBox.OnRaycastHit(weapon, weapon.transform.up);

                if (weapon.debug)
                {
                    if (hitBox.hitBoxID == HitBoxId.Head) Debug.LogWarning("HeadShot");
                    Debug.Log($"Enter: {hitBox.gameObject.name}");
                }
            }
        }
    }

    private void Reset()
    {
        _processedColliders.Clear();
        _isReset = true;
    }

    public void Update()
    {
        if (weapon.isOneTargetToHit && weapon.CanAttack() && !_isReset)
        {
            Reset();
        }
        else if (!weapon.isOneTargetToHit && weapon.IsStopAttack() && !_isReset)
        {
            Reset();
        }

        if (weapon.debug)
            Debug.DrawRay(weapon.transform.position, weapon.transform.up, Color.red);
    }

            //if (weapon.isOneTargetToHit)
            //    if (_isReset == true)
            //        return;

}
