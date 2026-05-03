using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaryStateAggression : IStateAggression
{
    public StateAggressionEnum GetID()
    {
        return StateAggressionEnum.Scary;
    }

    public void EnterState(Bot bot)
    {
    }

    public void UpdateState(Bot bot)
    {
    }

    public void ExitState(Bot bot)
    {
    }
}
