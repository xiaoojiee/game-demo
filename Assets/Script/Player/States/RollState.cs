using UnityEngine;

public class RollState : PlayerState
{
    private float timer;

    public RollState(PlayerStateMachine m) : base(m) { }
    public override StateType Type => StateType.Roll;

    public override void Enter()
    {
        timer = 0f;

        // 关闭重力 + 清垂直速度
        rb.useGravity = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 inputDir = machine.GetMoveDirection();
        Vector3 rollDir = inputDir.sqrMagnitude > 0.01f ? inputDir : machine.transform.forward;

        machine.rollDirection = rollDir;
        machine.rollCooldownTimer = machine.rollCooldown;
    }

    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer >= machine.rollDuration)
            machine.SwitchState(machine.idleState);
    }

    public override void FixedUpdate()
    {
        // 水平冲刺，Y 保持 0（无重力）
        machine.SetHorizontalVelocity(machine.rollDirection, machine.rollSpeed);
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        // 车轮翻滚：转 rollSpinCount 圈，时长 rollDuration，自动算速度
        float spinSpeed = 360f * machine.rollSpinCount / machine.rollDuration;
        Vector3 rollAxis = Vector3.Cross(Vector3.up, machine.rollDirection).normalized;
        machine.transform.Rotate(rollAxis, spinSpeed * Time.fixedDeltaTime, Space.World);
    }

    public override void Exit()
    {
        // 恢复重力
        rb.useGravity = true;

        // 扶正角色，只保留面朝方向的 Y 旋转
        Vector3 facing = machine.rollDirection;
        if (facing.sqrMagnitude < 0.01f) facing = machine.transform.forward;
        facing.y = 0f;
        if (facing.sqrMagnitude > 0.01f)
            machine.transform.rotation = Quaternion.LookRotation(facing);
    }
}
