using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    public StateEnum GetID() => StateEnum.Idle;

    public void EnterState(Bot bot)
    {
        bot.BotNavMesh.ResetPath();
    }

    public void UpdateState(Bot bot)
    {
        Vector3 directionToPlayer = bot.PlayerTransform.transform.position - bot.transform.position;

        Vector3 botDirection = bot.transform.forward;
        directionToPlayer.Normalize();

        float dotProduct = Vector3.Dot(directionToPlayer, botDirection);
        if (dotProduct > 0.0f && bot.StateMachineAggressiveBot.CurrentStateAggression != StateAggressionEnum.Aggressive)
        {
            bot.StateMachineBot.ChangeState(StateEnum.ChasePlayer);
        }
    }

    public void ExitState(Bot bot)
    {
    }
}
