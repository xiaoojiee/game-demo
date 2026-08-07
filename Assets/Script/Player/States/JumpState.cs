using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateMachine m) : base(m) { }
    public override StateType Type => StateType.Jump;

    public override void Enter()
    {
        var vel = rb.velocity; vel.y = machine.jumpForce; rb.velocity = vel;
    }

    public override void Update()
    {
        if (rb.velocity.y <= 0 && machine.IsGrounded())
            machine.SwitchState(machine.idleState);
    }

    public override void FixedUpdate()
    {
        var dir = machine.GetMoveDirection();
        machine.SetHorizontalVelocity(dir, machine.walkSpeed);
        machine.RotateTowards(dir);
    }
}
