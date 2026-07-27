using UnityEngine;

public class SlimeIdleState : SlimeState
{
    private float idleTimer;
    private float idleDuration;

    public float IdleTimer => idleTimer;
    public bool quickRecovery;

    public SlimeIdleState(SlimeStateMachine machine) : base(machine)
    {
    }

    // 随机计时+停速+播动画
    public override void Enter()
    {
        if (quickRecovery)
        {
            idleDuration = 0.3f;
            quickRecovery = false;
        }
        else
        {
            idleDuration = Random.Range(machine.idleDurationMin, machine.idleDurationMax);
        }
        idleTimer = idleDuration;

        machine.rb.velocity = new Vector2(0, machine.rb.velocity.y);
        machine.PlayAnim("Idle");
    }

    // 倒计时
    public override void Update()
    {
        idleTimer -= Time.deltaTime;
    }

    public override void Exit()
    {
        idleTimer = 0f;
    }
}
