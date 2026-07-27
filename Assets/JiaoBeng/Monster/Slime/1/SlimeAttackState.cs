using UnityEngine;
public class SlimeAttackState : SlimeState
{
    public SlimeAttackState(SlimeStateMachine machine) : base(machine)
    {
        
    }
    // 朝玩家+翻转+加力+设isAttack
    public override void Enter()
    {
        machine.isAttack=true;
        float direction = 0f;
        if (machine.Player != null)
        {
            float deltaX=machine.Player.position.x-machine.transform.position.x;
            direction=deltaX>0f?1f:-1f;

            float sign = direction > 0 ? 1 : -1;
            var s = machine.transform.localScale;
            s.x = Mathf.Abs(s.x) * sign;
            machine.transform.localScale = s;

        }
        else
        {
            direction=Random.Range(0,2)==0?-1f:1f;
        }
        machine.rb.velocity=Vector2.zero;
        Vector2 force=new Vector2(direction*machine.attackHorizontalForce,machine.attackJumpForce);
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