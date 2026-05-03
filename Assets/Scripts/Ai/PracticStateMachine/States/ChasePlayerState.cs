using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerState : IState
{
    public StateEnum GetID() => StateEnum.ChasePlayer;

    public void EnterState(Bot bot)
    {
    }

    public void UpdateState(Bot bot)
    {
        var distanceToPlayer = Vector3.Distance(bot.PlayerTransform.transform.position, bot.transform.position);
        if (distanceToPlayer > 2f && bot.StateMachineAggressiveBot.CurrentStateAggression != StateAggressionEnum.Aggressive)
            bot.BotNavMesh.destination = bot.PlayerTransform.transform.position;
        else
            bot.StateMachineBot.ChangeState(StateEnum.Idle);
    }

    public void ExitState(Bot bot)
    {
    }
}
