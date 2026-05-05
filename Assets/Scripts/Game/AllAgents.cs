using UnityEngine;

public class AllAgents : MonoBehaviour
{
    private AiAgent[] _allAgents;
    private int _countLiving;

    private void Start()
    {
        _allAgents = GetComponentsInChildren<AiAgent>();
    }

    public AiAgent[] GetAllAgent() => _allAgents;

    public int GetLivingAgents()
    {
        _countLiving = _allAgents.Length;
        foreach (var agent in _allAgents)
        {
            if (agent.stateMachine.currentState == AiStateId.Death)
                _countLiving--;
        }
        return _countLiving;
    }
}
