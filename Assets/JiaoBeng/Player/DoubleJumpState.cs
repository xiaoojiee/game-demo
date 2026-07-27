using UnityEngine;

public class DoubleJumpState : PlayerState
{
    public override StateType Type => StateType.DoubleJump;

    public float jumpForce = 18f;

    public DoubleJumpState(PlayerStateMachine machine) : base(machine) { }

    // 消费缓冲+跳跃力+开颜料子物体
    public override void Enter()
    {
        machine.jumpBuffer.Consume();
        machine.jumpCount++;

        machine.rb.velocity = new Vector2(machine.rb.velocity.x, jumpForce);

        if (machine.doubleJumpPaintObject != null)
            machine.doubleJumpPaintObject.SetActive(true);

        machine.anim.Play("Jump");
    }

    // 下落→切Fall
    public override void Update()
    {
        if (machine.rb.velocity.y < 0f)
            machine.SwitchState(machine.fallState);
    }

    // 空中移动
    public override void FixedUpdate()
    {
        machine.MoveCharacter();
    }

    // 关颜料子物体
    public override void Exit()
    {
        if (machine.doubleJumpPaintObject != null)
            machine.doubleJumpPaintObject.SetActive(false);
    }
}
