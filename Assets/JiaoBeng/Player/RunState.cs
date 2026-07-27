using UnityEngine;

public class RunState : PlayerState
{
    public override StateType Type => StateType.Run;

    public RunState(PlayerStateMachine machine) : base(machine) { }

    // 播跑步动画
    public override void Enter()
    {
        machine.anim.Play("Run");
    }

    // 无输入→切Idle
    public override void Update()
    {
        if (machine.isGrounded && Mathf.Abs(machine.horizontalInput) < 0.01f)
        {
            machine.SwitchState(machine.idleState);
        }
    }

    // 水平移动
    public override void FixedUpdate()
    {
        machine.MoveCharacter();
    }
}
