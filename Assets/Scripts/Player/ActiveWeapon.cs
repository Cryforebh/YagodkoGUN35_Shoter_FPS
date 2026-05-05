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
    public AllAgents allEnemies;
    public LayerMask obstacleLayerForAutoAim;

    WeaponBase[] equipped_weapons = new WeaponBase[3];
    CharacterAiming characterAiming;
    AmmoWidget ammoWidget;
    Transform crossHairTarget;
    ReloadWeapon reload;

    int activeWeaponIndex = -1;
    bool isHolstered = false;
    bool isAutoAim = false;
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

    void Update()
    {
        var weapon = GetWeapon(activeWeaponIndex);

        UpdateInputFiring(weapon);

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

    void UpdateInputFiring(WeaponBase weapon)
    {
        bool notSprinting = rigController.GetCurrentAnimatorStateInfo(2).shortNameHash == Animator.StringToHash("not_sprinting");
        bool canFire = !isHolstered && notSprinting && !reload.isReloading;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            isAutoAim = !isAutoAim;
            Debug.Log("AutoAim: " + isAutoAim);
            if (!isAutoAim)
            {
                if (weapon is MeleeWeapon weaponMW)
                    weaponMW.StopFiring();
                else
                    weapon.StopFiring();
            }
        }

        if (weapon && !isAutoAim)
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
        else
        {
            characterAiming.SetRotationLock(false);
        }

        UpdateAutoAim(weapon);
    }

    void UpdateAutoAim(WeaponBase weapon)
    {
        if (!isAutoAim)
        {
            characterAiming.SetAutoAimTarget(null); // Отключаем автоприцеливание
            return;
        }
        if (allEnemies == null) return;

        var enemies = allEnemies.GetAllAgent();
        if (enemies.Length <= 0)
        {
            characterAiming.SetAutoAimTarget(null);
            return;
        }

        float minDistance = float.MaxValue;
        float maxDistance = 15f;
        AiAgent target = null;

        foreach (var enemy in enemies)
        {
            if (enemy.stateMachine.currentState == AiStateId.Death)
                continue;

            var distance = Vector3.Distance(weapon.transform.position, enemy.transform.position + Vector3.up * enemy.navMeshAgent.radius);
            if (distance < minDistance)
            {
                target = enemy;
                minDistance = distance;
            }
        }

        if (target != null && weapon != null)
        {
            Vector3 playerPosition = weapon.transform.position;
            Vector3 agent = target.transform.position + Vector3.up * target.navMeshAgent.radius;
            var direction = (agent - playerPosition).normalized;
            Ray ray = new Ray(playerPosition, direction);

            Debug.DrawRay(playerPosition, direction, Color.cyan);

            if (Physics.Raycast(ray, minDistance, obstacleLayerForAutoAim, QueryTriggerInteraction.Ignore))
            {
                characterAiming.SetAutoAimTarget(null);
                if (weapon is RaycastWeapon raycastWeapon)
                    weapon.StopFiring();
            }
            else
            {
                bool isMaxDistance = Vector3.Distance(transform.position, target.transform.position) > maxDistance;
                characterAiming.SetAutoAimTarget(target.navMeshAgent.transform);

                if (weapon is MeleeWeapon weaponMW)
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
                else
                {
                    weapon.StartFiring();
                }
                weapon.UpdateWeapon(Time.deltaTime, crossHairTarget.position);
            }
        }
        else
        {
            characterAiming.SetAutoAimTarget(null);
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

        if (isChangingWeapon)
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
