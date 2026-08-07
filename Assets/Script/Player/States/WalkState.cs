using UnityEngine;

public class WalkState : PlayerState
{
    public WalkState(PlayerStateMachine m) : base(m) { }
    public override StateType Type => StateType.Walk;

    public override void Enter()
    {
        machine.animator.Play("walk");
    }

    public override void FixedUpdate()
    {
        Vector3 dir = machine.GetMoveDirection();
        machine.SetHorizontalVelocity(dir, machine.walkSpeed);
        machine.RotateTowards(dir);
    }
}
