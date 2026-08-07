using UnityEngine;

public class AttackState : PlayerState
{
    private bool dashActive;
    private float dashTimer;
    private Vector3 dashDir;

    public AttackState(PlayerStateMachine m) : base(m) { }
    public override StateType Type => StateType.Attack;

    public override void Enter()
    {
        dashActive = false;
        dashTimer = 0f;

        Vector3 inputDir = machine.GetMoveDirection();
        dashDir = inputDir.sqrMagnitude > 0.01f ? inputDir : machine.transform.forward;
        machine.transform.rotation = Quaternion.LookRotation(dashDir);

        bool hasWeapon = HasWeapon();
        machine.animator.Play(hasWeapon ? "attack1" : "attack0");
        dashActive = hasWeapon;
        machine.attackCooldownTimer = machine.attackCooldown;
    }

    bool HasWeapon() => machine.GetComponent<PlayerCombat>()?.HasDamageCapable() ?? false;

    public void TriggerDash()
    {
        dashActive = true;
        dashTimer = 0f;
    }

    public override void FixedUpdate()
    {
        if (!dashActive) return;

        dashTimer += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(dashTimer / machine.attackDashTime);
        float speed = machine.attackDashCurve.Evaluate(t) * machine.attackDashSpeed;
        machine.SetHorizontalVelocity(dashDir, speed);
    }

    public override void Exit()
    {
        dashActive = false;
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        machine.GetComponent<PlayerCombat>()?.DisableWeaponDamage();
    }
}
