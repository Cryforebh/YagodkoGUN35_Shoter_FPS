using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public string weaponName;
    public ActiveWeapon.WeaponSlot weaponSlot;
    public MeshSockets.SocketId holsterSocket;
    public LayerMask layerMask;
    public bool debug = false;
    public bool isFiring = false;
    public RuntimeAnimatorController animator;
    public float damage = 10;
    [Tooltip("Camera rotation speed while aiming. Default: 1")] public float speedAimRotate = 1;

    public Transform raycastOrigin;

    public virtual void StartFiring() { }
    public virtual void StopFiring() { }
    public virtual void UpdateWeapon(float deltaTime, Vector3 target) { }
    public bool IsSlotMelee() => weaponSlot == ActiveWeapon.WeaponSlot.Melee;
    public virtual void PlaySoundEquip() { }
}
