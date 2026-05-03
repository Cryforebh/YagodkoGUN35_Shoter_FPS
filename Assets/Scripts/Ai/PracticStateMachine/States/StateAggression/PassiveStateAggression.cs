using UnityEngine;

public class PassiveStateAggression : IStateAggression
{
    Bot bot;

    public StateAggressionEnum GetID()
    {
        return StateAggressionEnum.Passive;
    }

    public void EnterState(Bot bot)
    {
        Debug.Log("Все спокойно...");
        this.bot = bot;
        this.bot.PlayerTransform.WeaponShotEvent += PlayerTransform_WeaponShotEvent;
    }

    private void PlayerTransform_WeaponShotEvent()
    {
        this.bot.StateMachineAggressiveBot.ChangeState(StateAggressionEnum.Aggressive);
    }

    public void UpdateState(Bot bot)
    {

    }

    public void ExitState(Bot bot)
    {
        if (this.bot != null)
        this.bot.PlayerTransform.WeaponShotEvent -= PlayerTransform_WeaponShotEvent;
    }
}
