using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine m) : base(m) { }
    public override StateType Type => StateType.Idle;

    public override void Enter()
    {
        machine.animator.Play("idle");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    public override void FixedUpdate()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        // 静止不动，保持原有朝向
    }
}
