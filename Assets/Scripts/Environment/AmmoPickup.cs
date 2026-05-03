using UnityEngine;

public class AmmoPickup : PickupBase
{
    public int clipAmount = 2;

    private void OnTriggerEnter(Collider other) {
        if (_isEquip) return;

        ActiveWeapon playerWeapon = other.GetComponent<ActiveWeapon>();
        if (playerWeapon) {
            if (playerWeapon.IsEquipActiveMelee()) return;
            playerWeapon.RefillAmmo(clipAmount);
            SetEnable(false);
        }

        AiWeapons aiWeapons = other.GetComponent<AiWeapons>();
        if (aiWeapons && aiWeapons.IsLowAmmo()) {
            if (aiWeapons.IsEquipActiveMelee()) return;
            aiWeapons.RefillAmmo(clipAmount);
            SetEnable(false);
        }
    }
}
