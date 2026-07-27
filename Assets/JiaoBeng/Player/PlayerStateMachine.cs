using UnityEngine;

[System.Serializable]
public struct InputBuffer
{
    [HideInInspector] public bool hasInput;
    public float bufferDuration;
    private float _timer;

    public void Trigger()
    {
        hasInput = true;
        _timer = bufferDuration;
    }

    public void Tick()
    {
        if (_timer > 0)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
                hasInput = false;
        }
    }

    public bool Consume()
    {
        if (hasInput)
        {
            hasInput = false;
            _timer = 0;
            return true;
        }
        return false;
    }
}

public class PlayerStateMachine : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public BoxCollider2D boxCollider;
    public Transform groundCheck;
    public LayerMask groundLayer;

    public float speed = 10f;
    public float jumpSpeed = 15f;
    [Tooltip("松键时截断跳跃的系数。0.5=松键上升力减半。1=长按短按一样高")]
    [Range(0.2f, 1f)] public float jumpCutMultiplier = 0.5f;
    public int JumpTimes = 2;
    public float AttackSpeed = 15f;
    public float SprintAttackSpeed = 25f;

    [HideInInspector] public PaintMeter paintMeter;

    [Header("Cooldowns")]
    public float attackCooldown = 0.3f;
    public float sprintCooldown = 0.3f;
    public float sprintAttackCooldown = 3f;
    private float _attackCooldownTimer;
    private float _sprintCooldownTimer;
    private float _sprintAttackCooldownTimer;

    [Header("Buffer Time")]
    public float attackbs = 0.45f;
    public float jumpbs = 0.2f;

    public InputBuffer jumpBuffer;
    public InputBuffer attackBuffer;
    public InputBuffer sprintBuffer;
    public InputBuffer sprintAttackBuffer;

    public GameObject[] attackHitBoxes;
    public GameObject sprintAttackHitBox;
    public GameObject sprintTrailObject;
    public GameObject doubleJumpPaintObject;
    public int AttackDamage = 1;

    public IdleState    idleState;
    public RunState     runState;
    public JumpState    jumpState;
    public FallState    fallState;
    public SprintState      sprintState;
    public SprintAttackState sprintAttackState;
    public DoubleJumpState  doubleJumpState;
    public AttackState  attackState;

    public PlayerState playerState { get; private set; }
    public ActionPriority bestPriority { get; private set; }

    [HideInInspector] public bool isInvincible;
    public float horizontalInput { get; set; }
    public bool isGrounded { get; set; }
    public int jumpCount { get; set; }
    public int airSprintCount { get; set; }

    // 获取组件+初始化状态
    private void Awake()
    {
        if (rb == null)          rb = GetComponent<Rigidbody2D>();
        if (anim == null)        anim = GetComponent<Animator>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (paintMeter == null)  paintMeter = GetComponent<PaintMeter>();

        jumpBuffer.bufferDuration         = jumpbs;
        attackBuffer.bufferDuration       = attackbs;
        sprintBuffer.bufferDuration       = 0.2f;
        sprintAttackBuffer.bufferDuration = 0.2f;

        idleState        = new IdleState(this);
        runState         = new RunState(this);
        jumpState        = new JumpState(this);
        fallState        = new FallState(this);
        sprintState      = new SprintState(this);
        sprintAttackState = new SprintAttackState(this);
        doubleJumpState  = new DoubleJumpState(this);
        attackState      = new AttackState(this);
    }

    private void Start()
    {
        // 切换状态(Exit→Enter)
        SwitchState(idleState);
    }

    // 读输入→决策→切换→驱动
    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        isGrounded = boxCollider.IsTouchingLayers(groundLayer);
        if (isGrounded) { jumpCount = 0; airSprintCount = 0; }

        if (Input.GetButtonDown("Jump"))        jumpBuffer.Trigger();
        if (Input.GetMouseButtonDown(0))        attackBuffer.Trigger();
        if (Input.GetKeyDown(KeyCode.LeftShift)) sprintBuffer.Trigger();
        if (Input.GetKeyDown(KeyCode.E))        sprintAttackBuffer.Trigger();

        _attackCooldownTimer -= Time.deltaTime;
        _sprintCooldownTimer -= Time.deltaTime;
        _sprintAttackCooldownTimer -= Time.deltaTime;

        jumpBuffer.Tick();
        attackBuffer.Tick();
        sprintBuffer.Tick();
        sprintAttackBuffer.Tick();

        StateType best = DetermineBestState();
        ActionPriority bestPri = GetPriority(best);

        if (attackBuffer.hasInput && best != StateType.Attack)
            Debug.Log($"[AttackDebug] 攻击被拦 best={best}({bestPri}) curPri={bestPriority} cooldown={_attackCooldownTimer:F2} paint={paintMeter?.CurrentPaint ?? -1} canAfford={attackHitBoxes.Length>0 && CanAfford(attackHitBoxes[0])}");

        if (bestPri > bestPriority)
        {
            SwitchState(GetState(best));
        }

        playerState?.Update();
    }

    // 驱动当前状态物理
    private void FixedUpdate()
    {
        playerState?.FixedUpdate();
    }

    private ActionPriority GetPriority(StateType type)
    {
        switch (type)
        {
            case StateType.Idle:         return ActionPriority.Idle;
            case StateType.Run:          return ActionPriority.Run;
            case StateType.Fall:         return ActionPriority.Fall;
            case StateType.Jump:         return ActionPriority.Jump;
            case StateType.DoubleJump:   return ActionPriority.DoubleJump;
            case StateType.Sprint:       return ActionPriority.Sprint;
            case StateType.Attack:       return ActionPriority.Attack;
            case StateType.SprintAttack: return ActionPriority.SprintAttack;
            default:                     return ActionPriority.None;
        }
    }

    private PlayerState GetState(StateType type)
    {
        switch (type)
        {
            case StateType.Idle:         return idleState;
            case StateType.Run:          return runState;
            case StateType.Fall:         return fallState;
            case StateType.Jump:         return jumpState;
            case StateType.DoubleJump:   return doubleJumpState;
            case StateType.Sprint:       return sprintState;
            case StateType.Attack:       return attackState;
            case StateType.SprintAttack: return sprintAttackState;
            default:                     return idleState;
        }
    }

    // 优先级决策选出最优状态
    private StateType DetermineBestState()
    {
        ActionPriority bestPri = ActionPriority.None;
        StateType best = StateType.Idle;

        if (sprintAttackBuffer.hasInput && _sprintAttackCooldownTimer <= 0f
            // 检查颜料是否够
            && CanAfford(sprintAttackHitBox)
            && (isGrounded || airSprintCount < 1))
        {
            if (ActionPriority.SprintAttack > bestPri)
            {
                bestPri = ActionPriority.SprintAttack;
                best = StateType.SprintAttack;
            }
        }

        // Attack（50）
        if (attackBuffer.hasInput && _attackCooldownTimer <= 0f
            && (attackHitBoxes.Length == 0 || CanAfford(attackHitBoxes[0])))
        {
            if (ActionPriority.Attack > bestPri)
            {
                bestPri = ActionPriority.Attack;
                best = StateType.Attack;
            }
        }

        if (jumpBuffer.hasInput && !isGrounded && jumpCount < JumpTimes)
        {
            if (ActionPriority.DoubleJump > bestPri)
            {
                bestPri = ActionPriority.DoubleJump;
                best = StateType.DoubleJump;
            }
        }

        // Sprint（45）
        if (sprintBuffer.hasInput
            && _sprintCooldownTimer <= 0f
            && (paintMeter == null || HasEnoughForSprint())
            && (isGrounded || airSprintCount < 1))
        {
            if (ActionPriority.Sprint > bestPri)
            {
                bestPri = ActionPriority.Sprint;
                best = StateType.Sprint;
            }
        }

        // Jump（40）
        if (jumpBuffer.hasInput && isGrounded)
        {
            if (ActionPriority.Jump > bestPri)
            {
                bestPri = ActionPriority.Jump;
                best = StateType.Jump;
            }
        }

        // Fall（30）
        if (!isGrounded && rb.velocity.y < -0.01f)
        {
            if (ActionPriority.Fall > bestPri)
            {
                bestPri = ActionPriority.Fall;
                best = StateType.Fall;
            }
        }

        // Run（20）
        if (isGrounded && Mathf.Abs(horizontalInput) > 0.01f)
        {
            if (ActionPriority.Run > bestPri)
            {
                bestPri = ActionPriority.Run;
                best = StateType.Run;
            }
        }

        return best;
    }

    // 检查颜料是否够
    private bool HasEnoughForSprint()
    {
        if (paintMeter == null) return true;
        var trail = GetComponentInChildren<SprintTrail>(true);
        if (trail == null) return true;
        return paintMeter.CurrentPaint >= trail.EstimatedTotalCost;
    }

    public bool CanAfford(GameObject hitBox)
    {
        if (paintMeter == null || hitBox == null) return true;
        var ps = hitBox.GetComponent<PaintSpawner>();
        if (ps != null) return paintMeter.HasEnough(ps.dropCount, ps.paintChance);
        var aps = hitBox.GetComponent<AttackPaintSpawner>();
        if (aps != null) return paintMeter.HasEnough(aps.dropCount, aps.spreadSettings.paintChance);
        return true;
    }

    // 切换状态(Exit→Enter)
    public void SwitchState(PlayerState newState, bool forceReenter = false)
    {
        if (!forceReenter && playerState == newState) return;

        if (newState == sprintState)
            _sprintCooldownTimer = sprintCooldown;
        if (newState == sprintAttackState)
            _sprintAttackCooldownTimer = sprintAttackCooldown;

        playerState?.Exit();
        playerState = newState;
        bestPriority = GetPriority(newState.Type);
        playerState?.Enter();
    }

    // 水平移动+翻转朝向
    public void MoveCharacter()
    {
        Vector2 vel = rb.velocity;
        vel.x = horizontalInput * speed;
        rb.velocity = vel;
        if (horizontalInput > 0.1f)      transform.localScale = new Vector3(1, 1, 1);
        else if (horizontalInput < -0.1f) transform.localScale = new Vector3(-1, 1, 1);
    }

    // 开攻击碰撞体
    public void EnableAttackHitBox()
    {
        if (playerState is AttackState attack)
        {
            attack.EnableHitBox();
        }
        else if (playerState is SprintAttackState && sprintAttackHitBox != null)
        {
            sprintAttackHitBox.SetActive(true);
            var col = sprintAttackHitBox.GetComponent<PolygonCollider2D>();
            if (col != null) col.enabled = true;
            var ps = sprintAttackHitBox.GetComponent<PaintSpawner>();
            if (ps != null) ps.Launch();
            else sprintAttackHitBox.GetComponent<AttackPaintSpawner>()?.Launch();
        }
    }

    // 关攻击碰撞体
    public void DisableAttackHitBox()
    {
        if (playerState is AttackState attack)
        {
            attack.DisableHitBox();
        }
        else if (playerState is SprintAttackState && sprintAttackHitBox != null)
        {
            var col = sprintAttackHitBox.GetComponent<PolygonCollider2D>();
            if (col != null) col.enabled = false;
            sprintAttackHitBox.SetActive(false);
        }
        else if (playerState is DoubleJumpState && doubleJumpPaintObject != null)
        {
            doubleJumpPaintObject.SetActive(false);
        }
    }

    public void TriggerEndAttack()
    {
        attackState.EndAttack();
    }

    public void StartAttackCooldown()
    {
        _attackCooldownTimer = attackCooldown;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}
