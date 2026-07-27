using UnityEngine;
public class SlimePatrolState : SlimeState
{
    public SlimePatrolState(SlimeStateMachine machine) : base(machine)
    {
        
    }
    // 随机方向+加力+播动画
    public override void Enter()
    {
        int direction=Random.Range(0,2)==0?-1:1;
        machine.rb.velocity=Vector2.zero;
        
        Vector2 force=new Vector2(direction*machine.patrolHorizontalForce,machine.patrolJumpForce);
        machine.rb.AddForce(force,ForceMode2D.Impulse);
        machine.PlayAnim("Jump");

    }
    public override void Update()
    {
        
    }
    public override void Exit()
    {
        
    }
}