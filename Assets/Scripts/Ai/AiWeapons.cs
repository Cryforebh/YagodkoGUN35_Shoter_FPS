using System.Collections;
using UnityEngine;

public class AiWeapons : MonoBehaviour
{
    public enum WeaponState
    {
        Holstering,
        Holstered,
        Activating,
        Active,
        Reloading
    }

    public enum WeaponSlot
    {
        Primary,
        Secondary,
        Melee
    }

    public WeaponBase currentWeapon
    {
        get
        {
            try
            {
                return weapons[currentSlot];
            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }

    public WeaponSlot currentWeaponSlot
    {
        get
        {
            return (WeaponSlot)currentSlot;
        }
    }

    WeaponBase[] weapons = new WeaponBase[3];
    int currentSlot = 0;
    Animator animator;
    public Animator weaponRigAnimator;
    MeshSockets sockets;
    WeaponIk weaponIk;
    Transform currentTarget;
    WeaponState weaponState = WeaponState.Holstered;
    public float inaccuracy = 0.0f;
    public float dropForce = 1.5f;
    GameObject magazineHand;
    WeaponBase oldWeapon;

    public bool IsActive()
    {
        return weaponState == WeaponState.Active;
    }

    public bool IsHolstered()
    {
        return weaponState == WeaponState.Holstered;
    }

    public bool IsReloading()
    {
        return weaponState == WeaponState.Reloading;
    }

    public bool IsEquipActiveMelee()
    {
        return GetWeapon(currentSlot) is MeleeWeapon;
    }

    WeaponBase GetWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length)
        {
            return null;
        }
        return weapons[index];
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        sockets = GetComponent<MeshSockets>();
        weaponIk = GetComponent<WeaponIk>();
    }

    private void Update()
    {
        if (currentTarget && currentWeapon && IsActive())
        {
            Vector3 target = currentTarget.position + weaponIk.targetOffset;
            target += Random.insideUnitSphere * inaccuracy;
            currentWeapon.UpdateWeapon(Time.deltaTime, target);
        }
    }

    public void SetFiring(bool enabled)
    {
        if (enabled)
        {
            currentWeapon.StartFiring();
        }
        else
        {
            currentWeapon.StopFiring();
        }
    }

    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            currentWeapon.gameObject.AddComponent<Rigidbody>();
            weapons[currentSlot] = null;
        }
    }

    public bool HasWeapon()
    {
        return currentWeapon != null;
    }

    public void SetTarget(Transform target)
    {
        weaponIk.SetTargetTransform(target);
        currentTarget = target;
    }

    public void Equip(WeaponBase weapon)
    {
        oldWeapon = GetWeapon((int)weapon.weaponSlot);

        //if (oldWeapon)
        //{
        //    Destroy(oldWeapon.gameObject);
        //}
        if (currentWeapon == null)
        {
            currentSlot = (int)weapon.weaponSlot;
            weapons[currentSlot] = weapon;
            sockets.Attach(weapon.transform, weapon.holsterSocket);
        }
        else
        {
            weapons[(int)weapon.weaponSlot] = weapon;
            sockets.Attach(weapon.transform, weapon.holsterSocket);
            StartCoroutine(SwitchWeaponAnimation((int)weapon.weaponSlot));
        }
    }

    public void ActivateWeapon()
    {
        StartCoroutine(EquipWeaponAnimation());
    }

    public void DeactivateWeapon()
    {
        SetTarget(null);
        SetFiring(false);
        StartCoroutine(HolsterWeaponAnimation());
    }

    public void ReloadWeapon()
    {
        if (IsActive())
        {
            if (currentWeapon != null && currentWeapon is RaycastWeapon weaponRW)
            {
                weaponRW.PlaySoundReloadAmmo();
            }
            StartCoroutine(ReloadWeaponAnimation());
        }
    }

    public void SwitchWeapon(WeaponSlot slot)
    {
        if (weapons[(int)slot] == null)
        {
            return;
        }

        if (IsHolstered())
        {
            currentSlot = (int)slot;
            ActivateWeapon();
            return;
        }

        int equipIndex = (int)slot;
        if (IsActive() && currentSlot != equipIndex)
        {
            StartCoroutine(SwitchWeaponAnimation(equipIndex));
        }
    }

    public int Count()
    {
        int count = 0;
        foreach (var weapon in weapons)
        {
            if (weapon != null)
            {
                count++;
            }
        }
        return count;
    }

    public void AttackMelee()
    {
        if (currentWeapon is MeleeWeapon weaponMW)
        {
            if (weaponMW.CanAttack())
                weaponRigAnimator.SetTrigger("reload_weapon");

            weaponMW.StartFiring();

            if (weaponMW.IsStopAttack())
            {
                weaponRigAnimator.SetBool("reload_weapon", false);
            }
        }
    }

    public void ResetAttackMelee()
    {
        if (currentWeapon is MeleeWeapon weaponMW)
        {
            weaponMW.ResetAttack();
        }
    }

    IEnumerator EquipWeaponAnimation()
    {
        weaponState = WeaponState.Activating;
        animator.runtimeAnimatorController = currentWeapon.animator;
        currentWeapon.PlaySoundEquip(); // Sound Equip

        if (currentWeapon.IsSlotMelee())
        {
            weaponRigAnimator.SetBool("holster_weapon", false);
            weaponRigAnimator.SetBool("equip", true);
            weaponRigAnimator.Play("weapon_" + currentWeapon.weaponName + "_equip");
        }
        else
        {
            animator.SetBool("equip", true);
        }
        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }

        if (currentWeapon != null && !currentWeapon.IsSlotMelee()) // Это отвечает за поворот оружия в руках ИИ в сторону цели (Слишком резко)
        {
            weaponIk.enabled = true;
            weaponIk.SetAimTransform(currentWeapon.raycastOrigin);
        }
        weaponState = WeaponState.Active;
    }

    IEnumerator HolsterWeaponAnimation()
    {
        weaponState = WeaponState.Holstering;
        currentWeapon.PlaySoundEquip(); // Sound Equip

        if (currentWeapon.IsSlotMelee())
        {
            weaponRigAnimator.SetBool("equip", false);
            weaponRigAnimator.SetBool("holster_weapon", true);
            weaponRigAnimator.Play("weapon_" + currentWeapon.weaponName + "_holster");
        }
        else
        {
            animator.SetBool("equip", false);
        }

        weaponIk.enabled = false;
        yield return new WaitForSeconds(0.5f);

        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }

        weaponState = WeaponState.Holstered;
    }

    IEnumerator ReloadWeaponAnimation()
    {
        weaponState = WeaponState.Reloading;

        if (!currentWeapon.IsSlotMelee())
            animator.SetTrigger("reload_weapon");

        weaponIk.enabled = false;
        yield return new WaitForSeconds(0.5f);
        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1.0f)
        {
            yield return null;
        }

        weaponIk.enabled = true;
        weaponState = WeaponState.Active;
    }

    IEnumerator SwitchWeaponAnimation(int index)
    {
        if (oldWeapon)
            Destroy(oldWeapon.gameObject);

        yield return StartCoroutine(HolsterWeaponAnimation());
        currentSlot = index;
        yield return StartCoroutine(EquipWeaponAnimation());
    }

    public void OnAnimationEvent(string eventName)
    {
        switch (eventName)
        {
            case "attach_weapon":
                AttachWeapon();
                break;
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

    void AttachWeapon()
    {
        bool equipping = animator.GetBool("equip") || weaponRigAnimator.GetBool("equip");
        if (equipping)
        {
            if (currentWeapon is MeleeWeapon)
                sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.LeftHip);
            else
                sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.RightHand);
        }
        else
        {
            sockets.Attach(currentWeapon.transform, currentWeapon.holsterSocket);
        }
    }

    void DetachMagazine()
    {
        var leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        if (currentWeapon is RaycastWeapon weaponRW)
        {
            magazineHand = Instantiate(weaponRW.magazine, leftHand, true);
            weaponRW.magazine.SetActive(false);
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
        if (currentWeapon is RaycastWeapon weaponRW)
        {
            weaponRW.magazine.SetActive(true);
            Destroy(magazineHand);
            weaponRW.RefillAmmo();
            animator.ResetTrigger("reload_weapon");
        }
    }

    public void RefillAmmo(int clipCount)
    {
        if (currentWeapon is RaycastWeapon weaponRW)
        {
            if (weaponRW)
            {
                weaponRW.clipCount += clipCount;
            }
        }
    }

    public bool IsLowAmmo()
    {
        if (currentWeapon is RaycastWeapon weaponRW)
        {
            if (weaponRW)
            {
                return weaponRW.IsLowAmmo();
            }
        }
        return false;
    }

    public bool IsEquippedMelee() => currentWeapon is MeleeWeapon;
}
