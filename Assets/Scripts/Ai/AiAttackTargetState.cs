using UnityEngine;

public class AiAttackTargetState : AiState
{
    public AiStateId GetId()
    {
        return AiStateId.AttackTarget;
    }

    public void Enter(AiAgent agent)
    {
        agent.weapons.ActivateWeapon();

        if (agent.weapons.IsEquippedMelee())
        {
            agent.navMeshAgent.stoppingDistance = agent.config.AttackStoppingDistanceKnifeFighter;
        }
        else
        {
            agent.navMeshAgent.stoppingDistance = agent.config.AttackStoppingDistance;
            //agent.navMeshAgent.speed = agent.config.AttackSpeed;
        }
        agent.navMeshAgent.speed = agent.config.AttackSpeedKnifeFighter;
    }

    public void Update(AiAgent agent)
    {
        if (!agent.targeting.HasTarget)
        {
            agent.rigControl.ResetPositionLook();
            agent.stateMachine.ChangeState(AiStateId.FindTarget);
            return;
        }

        agent.weapons.SetTarget(agent.targeting.Target.transform);
        agent.navMeshAgent.destination = agent.targeting.TargetPosition;
        agent.rigControl.SetPositionLook(agent.targeting.TargetPosition);

        RotationBodyOnTarget(agent);
        MeleeActionWeapon(agent);
        ReloadWeapon(agent);
        SelectWeapon(agent);
        UpdateFiring(agent);
        UpdateLowHealth(agent);
        UpdateLowAmmo(agent);
    }

    private void UpdateFiring(AiAgent agent)
    {
        if (!agent.weapons.IsEquippedMelee()) // Новая строчка

            if (agent.targeting.TargetInSight)
            {
                agent.weapons.SetFiring(true);
            }
            else
            {
                agent.weapons.SetFiring(false);
            }
    }

    public void Exit(AiAgent agent)
    {
        agent.weapons.DeactivateWeapon();
        agent.navMeshAgent.stoppingDistance = 0.0f;
    }

    void RotationBodyOnTarget(AiAgent agent)
    {
        bool distance;
        if (agent.weapons.IsEquippedMelee())
            distance = agent.navMeshAgent.remainingDistance <= agent.config.AttackSpeedKnifeFighter;
        else
            distance = agent.navMeshAgent.remainingDistance <= agent.config.AttackStoppingDistance;

        if (distance)
        {
            Vector3 targetPosition = agent.targeting.TargetPosition;
            Vector3 directionToTarget = (targetPosition - agent.transform.position).normalized;
            directionToTarget.y = 0f;

            if (directionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                float rotationSpeed = 5f;
                agent.transform.rotation = Quaternion.Slerp(
                    agent.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
    void MeleeActionWeapon(AiAgent agent)
    {
        if (agent.weapons.IsEquippedMelee())
        {
            bool distance = agent.navMeshAgent.remainingDistance < agent.config.AttackKnifeDistanceMax;

            if (distance)
                agent.weapons.AttackMelee();
            else
                agent.weapons.ResetAttackMelee();
        }
        else
            agent.navMeshAgent.stoppingDistance = agent.config.AttackStoppingDistance;
    }

    void ReloadWeapon(AiAgent agent)
    {
        if (agent.weapons.currentWeapon is RaycastWeapon weaponRW && weaponRW.ShouldReload())
            agent.weapons.ReloadWeapon();
    }

    void SelectWeapon(AiAgent agent)
    {
        var bestWeapon = ChooseWeapon(agent);
        if (bestWeapon != agent.weapons.currentWeaponSlot)
        {
            agent.weapons.SwitchWeapon(bestWeapon);
        }
    }

    AiWeapons.WeaponSlot ChooseWeapon(AiAgent agent)
    {
        float distance = agent.targeting.TargetDistance;
        if (distance > agent.config.AttackCloseRange)
        {
            return AiWeapons.WeaponSlot.Primary;
        }
        else if (distance <= agent.config.AttackKnifeDistanceMax)
        {
            return AiWeapons.WeaponSlot.Melee;
        }
        else
        {
            return AiWeapons.WeaponSlot.Secondary;
        }
    }

    void UpdateLowHealth(AiAgent agent)
    {
        if (agent.health.IsLowHealth())
        {
            agent.rigControl.ResetPositionLook();
            agent.stateMachine.ChangeState(AiStateId.FindHealth);
        }
    }

    void UpdateLowAmmo(AiAgent agent)
    {
        if (agent.weapons.IsLowAmmo())
        {
            agent.rigControl.ResetPositionLook();
            agent.stateMachine.ChangeState(AiStateId.FindAmmo);
        }
    }
}
