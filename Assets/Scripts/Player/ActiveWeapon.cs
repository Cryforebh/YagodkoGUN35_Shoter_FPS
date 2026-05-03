using System;
using System.Collections;
using UnityEngine;

public class ActiveWeapon : MonoBehaviour
{
    public enum WeaponSlot
    {
        Primary = 0,
        Secondary = 1,
        Melee = 2
    }

    public Animator rigController;
    public Transform[] weaponSlots;
    public bool isChangingWeapon;

    WeaponBase[] equipped_weapons = new WeaponBase[3];
    CharacterAiming characterAiming;
    AmmoWidget ammoWidget;
    Transform crossHairTarget;
    ReloadWeapon reload;

    int activeWeaponIndex = -1;
    bool isHolstered = false;
    private float _timeNextRigStage = 0.05f;
    private WeaponBase _oldWeapon;

    public event Action WeaponShotEvent;

    private void Awake()
    {
        crossHairTarget = Camera.main.transform.Find("CrossHairTarget");
        ammoWidget = FindObjectOfType<AmmoWidget>();
        characterAiming = GetComponent<CharacterAiming>();
        reload = GetComponent<ReloadWeapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        RaycastWeapon existingWeapon = GetComponentInChildren<RaycastWeapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }
    }

    public bool IsAttack()
    {
        WeaponBase currentWeapon = GetActiveWeapon();
        if (!currentWeapon)
        {
            return false;
        }
        return currentWeapon.isFiring;
    }

    public WeaponBase GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }

    public bool IsEquipActiveMelee()
    {
        return GetActiveWeapon() is MeleeWeapon;
    }

    WeaponBase GetWeapon(int index)
    {
        if (index < 0 || index >= equipped_weapons.Length)
        {
            return null;
        }
        return equipped_weapons[index];
    }

    // Update is called once per frame
    void Update()
    {
        var weapon = GetWeapon(activeWeaponIndex);
        bool notSprinting = rigController.GetCurrentAnimatorStateInfo(2).shortNameHash == Animator.StringToHash("not_sprinting");
        bool canFire = !isHolstered && notSprinting && !reload.isReloading;
        if (weapon)
        {
            if (weapon is MeleeWeapon weaponMW)
            {
                if (Input.GetButton("Fire1") || !weaponMW.CanAttack())
                {
                    if (weaponMW.CanAttack())
                    {
                        rigController.SetTrigger("reload_weapon");
                    }

                    weaponMW.StartFiring();

                    if (weaponMW.IsStopAttack())
                    {
                        rigController.SetBool("reload_weapon", false);
                    }
                }
            }
            else
            {
                if (Input.GetButton("Fire1") && canFire && !weapon.isFiring)
                {
                    weapon.StartFiring();
                    WeaponShotEvent?.Invoke();
                }

                if (Input.GetButtonUp("Fire1") || !canFire)
                {
                    weapon.StopFiring();
                }
            }

            weapon.UpdateWeapon(Time.deltaTime, crossHairTarget.position);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleActiveWeapon();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetActiveWeapon(WeaponSlot.Primary);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetActiveWeapon(WeaponSlot.Secondary);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetActiveWeapon(WeaponSlot.Melee);
        }
    }

    public void Equip(WeaponBase newWeapon)
    {
        int weaponSlotIndex = (int)newWeapon.weaponSlot;
        _oldWeapon = GetWeapon(weaponSlotIndex);

        if (newWeapon is RaycastWeapon newRW)
        {
            newRW.recoil.characterAiming = characterAiming;
            newRW.recoil.animator = rigController;
            newRW.transform.SetParent(weaponSlots[weaponSlotIndex], false);
            equipped_weapons[weaponSlotIndex] = newRW;
        }
        else if (newWeapon is MeleeWeapon newMeleeWeapon)
        {
            newMeleeWeapon.transform.SetParent(weaponSlots[weaponSlotIndex], false);
            equipped_weapons[weaponSlotIndex] = newMeleeWeapon;
        }

        SetActiveWeapon(newWeapon.weaponSlot);
    }

    void ToggleActiveWeapon()
    {
        bool isHolstered = rigController.GetBool("holster_weapon");
        if (isHolstered)
        {
            StartCoroutine(ActivateWeapon(activeWeaponIndex));
        }
        else
        {
            StartCoroutine(HolsterWeapon(activeWeaponIndex));
        }
    }

    void SetActiveWeapon(WeaponSlot weaponSlot)
    {
        int holsterIndex = activeWeaponIndex;
        int activateIndex = (int)weaponSlot;

        if (/*holsterIndex == activateIndex ||*/ isChangingWeapon)
        {
            return;
        }

        if (ammoWidget)
        {
            WeaponBase newWeapon = GetWeapon(activateIndex);
            if (newWeapon is RaycastWeapon newRWammo)
                ammoWidget.Refresh(newRWammo.ammoCount, newRWammo.clipCount, activateIndex);
            else
                ammoWidget.Refresh(00, 00, activateIndex);
        }

        StartCoroutine(SwitchWeapon(holsterIndex, activateIndex));
    }

    IEnumerator SwitchWeapon(int holsterIndex, int activateIndex)
    {
        rigController.SetInteger("weapon_index", activateIndex);
        if (_oldWeapon)
            Destroy(_oldWeapon.gameObject);
        else
            yield return StartCoroutine(HolsterWeapon(holsterIndex));
        yield return StartCoroutine(ActivateWeapon(activateIndex));
        activeWeaponIndex = activateIndex;
    }

    IEnumerator HolsterWeapon(int index)
    {
        isChangingWeapon = true;
        isHolstered = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", true);
            weapon.PlaySoundEquip(); // Sound Equip
            do
            {
                yield return new WaitForSeconds(_timeNextRigStage);
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        }
        isChangingWeapon = false;
    }

    IEnumerator ActivateWeapon(int index)
    {
        isChangingWeapon = true;
        var weapon = GetWeapon(index);
        if (weapon)
        {
            rigController.SetBool("holster_weapon", false);
            rigController.Play("weapon_" + weapon.weaponName + "_equip");
            weapon.PlaySoundEquip(); // Sound Equip
            //Debug.Log("weapon_" + weapon.weaponName + "_equip");
            do
            {
                yield return new WaitForSeconds(_timeNextRigStage);
            } while (rigController.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
            isHolstered = false;
        }
        isChangingWeapon = false;
    }

    public void DropWeapon()
    {
        var currentWeapon = GetActiveWeapon();
        if (currentWeapon)
        {
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            currentWeapon.gameObject.AddComponent<Rigidbody>();
            equipped_weapons[activeWeaponIndex] = null;
        }
    }

    public void RefillAmmo(int clipCount)
    {
        var weapon = GetActiveWeapon();
        if (weapon)
        {
            if (weapon is RaycastWeapon weaponRW)
            {
                weaponRW.clipCount += clipCount;
                ammoWidget.Refresh(weaponRW.ammoCount, weaponRW.clipCount, (int)weapon.weaponSlot);
            }
        }
    }
}
