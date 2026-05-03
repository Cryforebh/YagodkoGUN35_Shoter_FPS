using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateAggressionEnum
{
    Passive,
    Scary,
    Aggressive
}

public interface IStateAggression
{
    StateAggressionEnum GetID();
    void EnterState(Bot bot);
    void UpdateState(Bot bot);
    void ExitState(Bot bot);
}
