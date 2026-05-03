using UnityEngine;

public class WeaponPickup : PickupBase
{
    public WeaponBase weaponBase;

    private void OnTriggerEnter(Collider other)
    {
        if (_isEquip) return;

        ActiveWeapon activeWeapon = other.gameObject.GetComponent<ActiveWeapon>();
        if (activeWeapon)
        {
            _isEquip = true;

            if (weaponBase is RaycastWeapon weapon)
            {
                RaycastWeapon newWeapon = Instantiate(weapon);
                activeWeapon.Equip(newWeapon);
                SetEnable(false);
            }
            else if (weaponBase is MeleeWeapon weaponMW)
            {
                MeleeWeapon newWeapon = Instantiate(weaponMW);
                activeWeapon.Equip(newWeapon);
                SetEnable(false);
            }
        }

        AiWeapons aiWeapons = other.gameObject.GetComponent<AiWeapons>();
        if (aiWeapons)
        {
            _isEquip = true;

            if (weaponBase is RaycastWeapon weapon)
            {
                RaycastWeapon newWeapon = Instantiate(weapon);
                aiWeapons.Equip(newWeapon);
                SetEnable(false);
            }
            else if(weaponBase is MeleeWeapon weaponMW)
            {
                MeleeWeapon newWeapon = Instantiate(weaponMW);
                aiWeapons.Equip(newWeapon);
                SetEnable(false);
            }
        }
    }
}
