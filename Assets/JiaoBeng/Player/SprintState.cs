using UnityEngine;

public class SprintState : PlayerState
{
    public override StateType Type => StateType.Sprint;

    private float _dashVelocityX;
    private float _dashTimer;
    private float _originalGravity;
    private const float DASH_DURATION = 0.2f;

    public SprintState(PlayerStateMachine machine) : base(machine) { }

    // 消费缓冲+重力归零+无敌
    public override void Enter()
    {
        machine.sprintBuffer.Consume();

        if (!machine.isGrounded)
            machine.airSprintCount++;

        _originalGravity = machine.rb.gravityScale;
        machine.rb.gravityScale = 0f;
        machine.rb.velocity = Vector2.zero;

        float dir = Mathf.Sign(machine.transform.localScale.x);
        _dashVelocityX = dir * machine.SprintAttackSpeed;
        _dashTimer = DASH_DURATION;

        machine.isInvincible = true;
        if (machine.sprintTrailObject != null)
            machine.sprintTrailObject.SetActive(true);
        machine.anim.Play("Run");
    }

    public override void Update() { }

    // 持续突进
    public override void FixedUpdate()
    {
        if (_dashTimer > 0f)
        {
            _dashTimer -= Time.fixedDeltaTime;
            Vector2 vel = machine.rb.velocity;
            vel.x = _dashVelocityX;
            machine.rb.velocity = vel;

            if (_dashTimer <= 0f)
            {
                if (Mathf.Abs(machine.horizontalInput) > 0.01f)
                    machine.SwitchState(machine.runState);
                else
                    machine.SwitchState(machine.idleState);
            }
        }
    }

    // 恢复重力+关无敌
    public override void Exit()
    {
        machine.rb.gravityScale = _originalGravity;
        machine.isInvincible = false;
        if (machine.sprintTrailObject != null)
            machine.sprintTrailObject.SetActive(false);
    }
}
