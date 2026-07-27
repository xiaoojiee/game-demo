using UnityEngine;

public class FallState : PlayerState
{
    public override StateType Type => StateType.Fall;

    public FallState(PlayerStateMachine machine) : base(machine) { }

    // 播下落动画
    public override void Enter()
    {
        machine.anim.Play("Fall");
    }

    // 落地→切Run/Idle
    public override void Update()
    {
        if (machine.isGrounded)
        {
            if (Mathf.Abs(machine.horizontalInput) > 0.01f)
                machine.SwitchState(machine.runState);
            else
                machine.SwitchState(machine.idleState);
        }
    }

    // 空中移动
    public override void FixedUpdate()
    {
        machine.MoveCharacter();
    }
}
