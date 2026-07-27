using UnityEngine;

public class JumpState : PlayerState
{
    public override StateType Type => StateType.Jump;

    public JumpState(PlayerStateMachine machine) : base(machine) { }

    // 消费缓冲+施加跳跃力
    public override void Enter()
    {
        machine.jumpBuffer.Consume();
        machine.jumpCount++;

        machine.rb.velocity = new Vector2(machine.rb.velocity.x, machine.jumpSpeed);

        machine.anim.Play("Jump");
    }

    // 松键砍跳/下落→切Fall
    public override void Update()
    {
        if (Input.GetButtonUp("Jump") && machine.rb.velocity.y > 0f)
        {
            machine.rb.velocity = new Vector2(
                machine.rb.velocity.x,
                machine.rb.velocity.y * machine.jumpCutMultiplier
            );
        }

        if (machine.rb.velocity.y < 0f)
        {
            machine.SwitchState(machine.fallState);
        }
    }

    // 移动+突进
    public override void FixedUpdate()
    {
        machine.MoveCharacter();
    }
}
