using UnityEngine;

public class RunState : PlayerState
{
    public RunState(PlayerStateMachine m) : base(m) { }
    public override StateType Type => StateType.Run;

    public override void Enter()
    {
        machine.animator.Play("run");
    }

    public override void FixedUpdate()
    {
        Vector3 dir = machine.GetMoveDirection();
        machine.SetHorizontalVelocity(dir, machine.runSpeed);
        machine.RotateTowards(dir);
    }
}
