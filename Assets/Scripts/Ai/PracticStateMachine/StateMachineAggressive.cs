using System;
using UnityEngine;

public class StateMachineAggressive
{
    public IStateAggression[] StateAggressions;
    public Bot ThisBot;
    public StateAggressionEnum CurrentStateAggression;

    public StateMachineAggressive(Bot bot)
    {
        ThisBot = bot;
        int numState = Enum.GetNames(typeof(StateAggressionEnum)).Length;
        StateAggressions = new IStateAggression[numState];
    }

    public void RegistryState(IStateAggression stateAggression)
    {
        int id = (int)stateAggression.GetID();
        StateAggressions[id] = stateAggression;
    }

    public IStateAggression GetState(StateAggressionEnum stateAggressionEnum)
    {
        return StateAggressions[(int)stateAggressionEnum];
    }

    public void Update()
    {
        GetState(CurrentStateAggression)?.UpdateState(ThisBot);
    }

    public void ChangeState(StateAggressionEnum nextStateEnum)
    {
        if (GetState(nextStateEnum) == null)
        {
            Debug.LogError($"{nextStateEnum} - Состояние не зарегестрированно!");
        }

        if (nextStateEnum != CurrentStateAggression)
        {
            GetState(CurrentStateAggression)?.ExitState(ThisBot);
            CurrentStateAggression = nextStateEnum;
            GetState(CurrentStateAggression)?.EnterState(ThisBot);
        }
    }
}
