using UnityEngine;

[RequireComponent(typeof(WeaponSound))]
public class MeleeWeapon : WeaponBase
{
    public float delayNextHit = 1f;
    public bool isOneTargetToHit = false;

    private WeaponSound _sound;
    private Collider _triggerDamageCollider;
    private bool _isAttack = false;
    private bool _isStopAttack = false;
    private float _currentTimeNextAttack = 0f;

    private float _timeDelayOnDisableTriggerDamage = 0.4f;
    private float _timeDelayOnEnableTriggerDamage = 0.1f;
    private float _currentTimeEnableTriggerDamage = 0f;

    public override void StartFiring()
    {
        if (!_isAttack)
        {
            _isAttack = true;
            _sound.PlaySoundAttack();
        }
        else
        {
            UpdateAttack();
        }
    }

    public void UpdateAttack()
    {
        _currentTimeNextAttack += Time.deltaTime;
        if (_currentTimeNextAttack >= delayNextHit)
        {
            _currentTimeNextAttack = 0;
            _currentTimeEnableTriggerDamage = 0;
            _isAttack = false;
            _isStopAttack = false;
        }
        TriggerDamageLogic();
    }

    public void ResetAttack()
    {
        _currentTimeNextAttack = 0;
        _currentTimeEnableTriggerDamage = 0;
        _isAttack = false;
        _isStopAttack = false;
        TriggerDamageOnDisable();
    }

    private void OnEnable()
    {
        _sound = GetComponent<WeaponSound>();
        _triggerDamageCollider = GetComponentInChildren<MeleeDamageTrigger>().GetComponent<Collider>();
        TriggerDamageOnDisable();
    }

    private void TriggerDamageOnEnable() => _triggerDamageCollider.enabled = true;
    private void TriggerDamageOnDisable() => _triggerDamageCollider.enabled = false;
    private bool IsStartTriggerEnable() => !_triggerDamageCollider.enabled && !_isStopAttack;

    private void TriggerDamageLogic()
    {
        _currentTimeEnableTriggerDamage += Time.deltaTime;
        if (_currentTimeEnableTriggerDamage >= _timeDelayOnEnableTriggerDamage && IsStartTriggerEnable())
        {
            TriggerDamageOnEnable();
        }
        if (_currentTimeEnableTriggerDamage >= _timeDelayOnDisableTriggerDamage)
        {
            _isStopAttack = true;
            TriggerDamageOnDisable();
        }
    }

    public bool CanAttack() => !_isAttack;
    public bool IsStopAttack() => _isStopAttack;

    public override void PlaySoundEquip()
    {
        if (_sound != null)
            _sound.PlaySoundEquip();
    }

    public override void StopFiring()
    {
        ResetAttack();
    }
}
