using System;
using UnityEngine;

public class StateMachine
{
    public IState[] m_states;
    public Bot m_bot;
    public StateEnum m_currentState;

    public StateMachine(Bot bot)
    {
        m_bot = bot;
        int numberOfStates = Enum.GetNames(typeof(StateEnum)).Length;
        m_states = new IState[numberOfStates];
    }

    public void RegistryState(IState state)
    {
        int id = (int)state.GetID();
        m_states[id] = state;
    }

    public IState GetState(StateEnum iDState)
    {
        int id = (int)iDState;
        return m_states[id];
    }

    public void Update()
    {
        GetState(m_currentState)?.UpdateState(m_bot);
    }

    public void ChangeState(StateEnum newState)
    {
        var state = GetState(newState);
        if (state == null)
        {
            Debug.LogError($"{newState} - Состояние не зарегестрированно!");
        }

        if (newState != m_currentState)
        {
            GetState(m_currentState)?.ExitState(m_bot);
            m_currentState = newState;
            GetState(m_currentState)?.EnterState(m_bot);
        }
    }
}
