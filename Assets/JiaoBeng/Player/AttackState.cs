using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : PlayerState
{
    public override StateType Type => StateType.Attack;

    public int ComboIndex { get; private set; }
    private bool _hasHitTarget;

    private float _dashVelocityX;
    private float _dashTimer;
    private float _originalGravity;
    private const float DASH_DURATION = 0.12f;

    private bool _waitingForCombo;
    private float _comboWindowTimer;
    private const float COMBO_WINDOW = 0.25f;

    public AttackState(PlayerStateMachine machine) : base(machine) { }

    public override void Enter()
    {
        float attackDir = Mathf.Sign(machine.transform.localScale.x);

        _dashVelocityX = attackDir * machine.AttackSpeed;
        _dashTimer = DASH_DURATION;

        _originalGravity = machine.rb.gravityScale;
        machine.rb.gravityScale *= 0.5f;

        _hasHitTarget = false;
        _waitingForCombo = false;

        if (ComboIndex == 0)
        {
            ComboIndex = 1;
        }

        machine.anim.Play("attack" + ComboIndex);

        machine.attackBuffer.Consume();
        // 关所有碰撞体
        DisableAllHitBox();
    }

    // 连击宽容窗口
    public override void Update()
    {
        if (_waitingForCombo)
        {
            _comboWindowTimer -= Time.deltaTime;

            if (machine.attackBuffer.Consume() && ComboIndex < 3)
            {
                _waitingForCombo = false;
                ComboIndex++;
                machine.SwitchState(this, true);
                return;
            }

            if (_comboWindowTimer <= 0f)
            {
                _waitingForCombo = false;
                // 重置连击+冷却
                FinishCombo();
            }
        }
    }

    // 突进/静止
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
                _dashVelocityX = 0f;
            }
        }
        else if (!_waitingForCombo)
        {
            Vector2 vel = machine.rb.velocity;
            vel.x = 0f;
            machine.rb.velocity = vel;
        }
    }

    // 开碰撞体+泼颜料
    public void EnableHitBox()
    {
        int index = ComboIndex - 1;
        if (index >= 0 && index < machine.attackHitBoxes.Length)
        {
            GameObject hitBoxObj = machine.attackHitBoxes[index];
            if (hitBoxObj != null)
            {
                hitBoxObj.SetActive(true);
                PolygonCollider2D collider2D = hitBoxObj.GetComponent<PolygonCollider2D>();
                if (collider2D != null) collider2D.enabled = true;
                var ps = hitBoxObj.GetComponent<PaintSpawner>();
                if (ps != null) ps.Launch();
                else hitBoxObj.GetComponent<AttackPaintSpawner>()?.Launch();
            }
        }
    }

    // 关碰撞体
    public void DisableHitBox()
    {
        int index = ComboIndex - 1;
        if (index >= 0 && index < machine.attackHitBoxes.Length)
        {
            GameObject hitBoxObj = machine.attackHitBoxes[index];
            if (hitBoxObj != null)
            {
                PolygonCollider2D collider = hitBoxObj.GetComponent<PolygonCollider2D>();
                if (collider != null) collider.enabled = false;
                hitBoxObj.SetActive(false);
            }
        }
    }

    // 连击窗口/结束攻击
    public void EndAttack()
    {
        // 关所有碰撞体
        DisableAllHitBox();

        if (machine.attackBuffer.Consume() && ComboIndex < 3)
        {
            ComboIndex++;
            machine.SwitchState(this, true);
            return;
        }

        if (ComboIndex < 3)
        {
            _waitingForCombo = true;
            _comboWindowTimer = COMBO_WINDOW;
        }
        else
        {
            // 重置连击+冷却
            FinishCombo();
        }
    }

    // 重置连击+冷却
    private void FinishCombo()
    {
        machine.rb.gravityScale = _originalGravity;
        ComboIndex = 0;
        machine.StartAttackCooldown();

        if (machine.isGrounded)
        {
            machine.SwitchState(Mathf.Abs(machine.horizontalInput) > 0.01f
                ? machine.runState
                : machine.idleState);
        }
        else
        {
            machine.SwitchState(machine.fallState);
        }
    }

    // 关所有碰撞体
    private void DisableAllHitBox()
    {
        foreach (var box in machine.attackHitBoxes)
        {
            if (box != null)
            {
                PolygonCollider2D collider = box.GetComponent<PolygonCollider2D>();
                if (collider != null) collider.enabled = false;
                box.SetActive(false);
            }
        }
    }

    public override void Exit()
    {
        machine.rb.gravityScale = _originalGravity;
    }

    public void MarkHit() => _hasHitTarget = true;
    public bool HasHitTarget => _hasHitTarget;
}
