using UnityEngine;

public class FishPassive : AbstractFish
{
    public override void Init(FishData data, Vector3 idleCenter, float idleRadius)
    {
        base.Init(data, idleCenter, idleRadius);
    }

    protected override void InitStateMachine()
    {
        var stateMachine = new EntityStateMachine();
        var idleState = new FishIdleState(StateMachine, this);
        var panicState = new FishPanicState(StateMachine, this);

        stateMachine.AddState(idleState);
        stateMachine.AddState(panicState);
        InitState(stateMachine);

        stateMachine.SetState<FishIdleState>();
    }

    protected override bool CheckState()
    {
        return StateMachine.CurrentEntityState is FishIdleState;
    }

    protected override void ReactionForPlayer()
    {
        StateMachine.SetState<FishPanicState>();
    }
}