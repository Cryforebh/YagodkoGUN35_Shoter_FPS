using UnityEngine;

public class HealthPickup : PickupBase
{
    public float amount = 50;

    private void OnTriggerEnter(Collider other) {
        if (_isEquip) return;

        Health health = other.GetComponent<Health>();
        if (health && health.IsLowHealth()) {
            health.Heal(amount);
            SetEnable(false);
        }
    }
}
