using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateEnum
{
    Idle,
    Walk,
    ChasePlayer
}

public interface IState
{
    public StateEnum GetID();
    public void EnterState(Bot bot);
    public void UpdateState(Bot bot);
    public void ExitState(Bot bot);
}
