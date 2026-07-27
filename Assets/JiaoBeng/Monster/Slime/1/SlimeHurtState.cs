using UnityEngine;

public class SlimeHurtState : SlimeState
{
    private float hurtTimer;

    public float HurtTimer => hurtTimer;

    public SlimeHurtState(SlimeStateMachine machine) : base(machine)
    {
    }

    public override void Enter()
    {
        hurtTimer = machine.hurtDuration;

        if (machine.anim != null)
            machine.anim.speed = 1f;

        machine.PlayAnim("Idle");
    }

    public override void Update()
    {
        hurtTimer -= Time.deltaTime;

        if (hurtTimer < -2f)
        {
            if (machine.anim != null) machine.anim.speed = 1f;
            machine.ChangeState(machine.idleState);
        }
    }

    public override void Exit()
    {
        hurtTimer = 0f;
    }
}
