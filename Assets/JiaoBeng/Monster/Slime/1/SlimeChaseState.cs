using UnityEngine;
public class SlimeChaseState : SlimeState
{
    public SlimeChaseState(SlimeStateMachine machine) : base(machine)
    {
        
    }
    // 朝玩家方向+加力+播动画
    public override void Enter()
    {
        float direction = 0f;
        if (machine.Player != null)
        {
            float deltaX=machine.Player.position.x-machine.transform.position.x;
            direction=deltaX>0f?1f:-1f;

        }
        else
        {
            direction=Random.Range(0,2)==0?-1f:1f;
        }
        machine.rb.velocity=Vector2.zero;
        Vector2 force=new Vector2(direction*machine.chaseHorizontalForce,machine.chaseJumpForce);
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