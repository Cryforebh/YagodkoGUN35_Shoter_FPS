using UnityEngine;

public class ReloadWeapon : MonoBehaviour
{
    public Animator animator;
    public WeaponAnimationEvents animationEvents;
    public Transform leftHand;
    public bool isReloading;
    public float dropForce = 1.5f;

    GameObject magazineHand;
    ActiveWeapon activeWeapon;
    AmmoWidget ammoWidget;

    private void Awake()
    {
        ammoWidget = FindObjectOfType<AmmoWidget>();
        activeWeapon = GetComponent<ActiveWeapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        animationEvents.WeaponAnimationEvent.AddListener(OnWeaponAnimationEvent);
    }

    // Update is called once per frame
    void Update()
    {
        WeaponBase weapon = activeWeapon.GetActiveWeapon();
        if (weapon && !activeWeapon.isChangingWeapon)
        {
            if (weapon is RaycastWeapon weaponRW)
            {
                if ((Input.GetKeyDown(KeyCode.R) && weaponRW.CanReload()) || weaponRW.ShouldReload())
                {
                    Reloading();
                }

                if (weaponRW.isFiring && ammoWidget)
                {
                    ammoWidget.Refresh(weaponRW.ammoCount, weaponRW.clipCount, (int)weapon.weaponSlot);
                }
            }
        }
    }

    void Reloading()
    {
        isReloading = true;
        animator.SetTrigger("reload_weapon");
    }

    void OnWeaponAnimationEvent(string eventName)
    {
        Debug.Log("OnAnimationEvent:" + eventName);
        switch (eventName)
        {
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }
    }

    void DetachMagazine()
    {
        WeaponBase weapon = activeWeapon.GetActiveWeapon();
        if (weapon is RaycastWeapon weaponRW)
        {
            magazineHand = Instantiate(weaponRW.magazine, leftHand, true);
            weaponRW.magazine.SetActive(false);
            weaponRW.PlaySoundReloadAmmo();
        }
    }

    void DropMagazine()
    {
        GameObject droppedMagazine = Instantiate(magazineHand, magazineHand.transform.position, magazineHand.transform.rotation);
        droppedMagazine.SetActive(true);
        Rigidbody body = droppedMagazine.AddComponent<Rigidbody>();

        Vector3 dropDirection = -gameObject.transform.right;
        dropDirection += Vector3.down;

        body.AddForce(dropDirection * dropForce, ForceMode.Impulse);
        droppedMagazine.AddComponent<BoxCollider>();
        magazineHand.SetActive(false);
    }

    void RefillMagazine()
    {
        magazineHand.SetActive(true);
    }

    void AttachMagazine()
    {
        WeaponBase weaponBase = activeWeapon.GetActiveWeapon();

        if (weaponBase is RaycastWeapon weapon)
            if (weapon)
            {
                weapon.magazine.SetActive(true);
                Destroy(magazineHand);
                weapon.RefillAmmo();
                animator.ResetTrigger("reload_weapon");
                if (ammoWidget)
                {
                    ammoWidget.Refresh(weapon.ammoCount, weapon.clipCount, (int)weapon.weaponSlot);
                }
                isReloading = false;
            }
    }
}
