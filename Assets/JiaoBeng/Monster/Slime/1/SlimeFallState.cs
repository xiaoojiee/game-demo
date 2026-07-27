using UnityEngine;

public class SlimeFallState : SlimeState
{
    public SlimeFallState(SlimeStateMachine machine) : base(machine)
    {
    }

    // 按isAttack开伤害框
    public override void Enter()
    {
        if (machine.isAttack && machine.attackTrigger != null)
        {
            machine.attackTrigger.SetActive(true);
        }
        machine.PlayAnim("Jump");
    }

    // 空中速度控制
    public override void Update()
    {
        float vx = machine.rb.velocity.x;
        float vy = machine.rb.velocity.y;

        if (Mathf.Abs(vx) < 0.1f)
            return;

        float currentDir = Mathf.Sign(vx);
        float absVx = Mathf.Abs(vx);

        bool towardPlayer = false;
        if (machine.Player != null)
        {
            float toPlayerX = machine.Player.position.x - machine.transform.position.x;
            towardPlayer = Mathf.Sign(toPlayerX) == currentDir;
        }

        if (towardPlayer)
        {
            absVx += machine.airControlForce * Time.deltaTime;
            absVx = Mathf.Min(absVx, machine.attackHorizontalForce * 1.5f);
        }
        else
        {
            absVx -= machine.airControlForce * Time.deltaTime;
            absVx = Mathf.Max(absVx, 0.3f);
        }

        machine.rb.velocity = new Vector2(currentDir * absVx, vy);
    }

    // 关伤害框
    public override void Exit()
    {
        if (machine.attackTrigger != null)
            machine.attackTrigger.SetActive(false);
    }
}
