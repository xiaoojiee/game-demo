using UnityEngine;

public class SprintAttackState : PlayerState
{
    public override StateType Type => StateType.SprintAttack;

    private float _dashVelocityX;
    private float _dashTimer;
    private float _originalGravity;
    private const float DASH_DURATION = 0.28f;

    public SprintAttackState(PlayerStateMachine machine) : base(machine) { }

    // 消费缓冲+重力归零+开攻击
    public override void Enter()
    {
        machine.sprintAttackBuffer.Consume();

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
        machine.EnableAttackHitBox();
        machine.anim.Play("sprint");
    }

    public override void Update() { }

    // 持续突进+攻击
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
                machine.DisableAttackHitBox();
                if (Mathf.Abs(machine.horizontalInput) > 0.01f)
                    machine.SwitchState(machine.runState);
                else
                    machine.SwitchState(machine.idleState);
            }
        }
    }

    // 恢复重力+关攻击
    public override void Exit()
    {
        machine.rb.gravityScale = _originalGravity;
        machine.isInvincible = false;
        machine.DisableAttackHitBox();
        if (machine.sprintTrailObject != null)
            machine.sprintTrailObject.SetActive(false);
    }
}
