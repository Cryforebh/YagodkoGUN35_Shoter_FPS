using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressiveStateAggression : IStateAggression
{
    private float m_timerPassive = 10f;
    private float m_currentTime = 0;

    public StateAggressionEnum GetID()
    {
        return StateAggressionEnum.Aggressive;
    }

    public void EnterState(Bot bot)
    {
        Debug.Log("Рядом какой-то Пиздец!");
    }

    public void UpdateState(Bot bot)
    {
        m_currentTime += Time.deltaTime;

        if (m_currentTime >= m_timerPassive)
        {
            m_currentTime = 0;

            if (!bot.CheckingNearShotsBot.Shoting)
            {
                bot.StateMachineAggressiveBot.ChangeState(StateAggressionEnum.Passive);
            }
        }
    }

    public void ExitState(Bot bot)
    {

    }
}
