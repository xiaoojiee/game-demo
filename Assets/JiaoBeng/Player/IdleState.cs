using UnityEngine;

public class IdleState : PlayerState
{
    public override StateType Type => StateType.Idle;

    public IdleState(PlayerStateMachine machine) : base(machine) { }

    // 播待机动画
    public override void Enter()
    {
        machine.anim.Play("Idle");
    }

    // 检测方向输入→切Run
    public override void Update()
    {
        if (machine.isGrounded && Mathf.Abs(machine.horizontalInput) > 0.01f)
        {
            machine.SwitchState(machine.runState);
        }
    }

    // 水平速度归零
    public override void FixedUpdate()
    {
        machine.rb.velocity = new Vector2(0, machine.rb.velocity.y);
    }
}
