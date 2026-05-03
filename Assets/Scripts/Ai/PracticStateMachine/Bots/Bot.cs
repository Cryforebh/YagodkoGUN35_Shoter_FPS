using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    public StateEnum InitialState;
    public StateAggressionEnum InitialStateAggression;
    public ActiveWeapon PlayerTransform;

    [HideInInspector] public StateMachine StateMachineBot;
    [HideInInspector] public StateMachineAggressive StateMachineAggressiveBot;
    [HideInInspector] public NavMeshAgent BotNavMesh;
    [HideInInspector] public CheckingNearShots CheckingNearShotsBot;

    private void Start()
    {
        BotNavMesh = GetComponent<NavMeshAgent>();
        CheckingNearShotsBot = GetComponent<CheckingNearShots>();

        StateMachineAggressiveBot = new StateMachineAggressive(this);
        StateMachineAggressiveBot.RegistryState(new PassiveStateAggression());
        StateMachineAggressiveBot.RegistryState(new AggressiveStateAggression());
        StateMachineAggressiveBot.RegistryState(new ScaryStateAggression());
        StateMachineAggressiveBot.ChangeState(InitialStateAggression);

        StateMachineBot = new StateMachine(this);
        StateMachineBot.RegistryState(new IdleState());
        StateMachineBot.RegistryState(new ChasePlayerState());
        StateMachineBot.ChangeState(InitialState);
    }

    private void Update()
    {
        StateMachineAggressiveBot.Update();
        StateMachineBot.Update();
    }
}
