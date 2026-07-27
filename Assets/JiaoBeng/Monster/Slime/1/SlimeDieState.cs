using UnityEngine;

public class SlimeDieState : SlimeState
{
    public SlimeDieState(SlimeStateMachine machine) : base(machine)
    {
        
    }
    // 停物理+关碰撞+冻动画
    public override void Enter()
    {
        machine.rb.velocity=Vector2.zero;
        machine.rb.isKinematic=true;
        machine.boxCollider2D.enabled=false;
        if (machine.attackTrigger != null)
        {
            machine.attackTrigger.SetActive(false);
        }
        machine.anim.enabled=false;
    }
    public override void Update()
    {
        
    }
    public override void Exit()
    {
        
    }
}