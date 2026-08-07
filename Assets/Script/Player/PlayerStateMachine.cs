using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PlayerStateMachine : MonoBehaviour
{
    // ========== 组件 ==========
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator animator;
    private GroundCheck groundCheck;

    // ========== 移动 ==========
    [Header("移动")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 15f;

    // ========== 攻击 ==========
    [Header("攻击")]
    public float attackDashSpeed = 8f;         // 突刺最大速度
    public float attackDashTime = 0.15f;       // 突刺持续多久（秒）
    public AnimationCurve attackDashCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);  // 突刺速度曲线
    public float attackCooldown = 0.5f;        // 攻击后冷却
    public float attackExitDelay = 0.1f;       // 动画结束后停顿多久再退出攻击

    [HideInInspector] public float attackCooldownTimer;
    [HideInInspector] public bool attackPressed;  // 左键按下瞬间

    // ========== 跳跃 ==========
    [Header("跳跃")]
    public float jumpForce = 10f;

    [HideInInspector] public bool jumpPressed;

    // ========== 翻滚 ==========
    [Header("翻滚")]
    public float rollSpeed = 12f;
    public float rollDuration = 0.4f;
    public float rollCooldown = 1f;
    public float rollSpinCount = 1.5f;

    [HideInInspector] public float rollCooldownTimer;
    [HideInInspector] public bool rollPressed;
    [HideInInspector] public Vector3 rollDirection;

    // ========== 击退 ==========
    private Vector3 pendingForce;    // 待加力

    /// <summary>外部调用，下一物理帧生效</summary>
    public void ApplyKnockback(Vector3 force)
    {
        pendingForce += force;
    }

    // ========== 运行时 ==========
    [HideInInspector] public Vector2 moveInput;
    [HideInInspector] public bool runHeld;

    // ========== 调试 ==========
    [Header("调试")]
    [SerializeField] private string debugState;
    public PlayerState currentState { get; private set; }
    public ActionPriority currentPriority { get; private set; }

    // ========== 状态实例 ==========
    [HideInInspector] public IdleState idleState;
    private WalkState walk;
    private RunState run;
    private JumpState jump;
    private AttackState attack;
    private RollState roll;

    // ==================== 初始化 ====================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        animator = GetComponent<Animator>();
        groundCheck = GetComponent<GroundCheck>();

        idleState = new IdleState(this);
        walk      = new WalkState(this);
        run       = new RunState(this);
        jump      = new JumpState(this);
        attack    = new AttackState(this);
        roll      = new RollState(this);
    }

    private void Start()
    {
        SwitchState(idleState);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ==================== 主循环 ====================

    private void Update()
    {
        // 读输入
        moveInput     = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        runHeld       = Input.GetKey(KeyCode.LeftShift);
        jumpPressed   = Input.GetKeyDown(KeyCode.Space);
        rollPressed   = Input.GetKeyDown(KeyCode.LeftAlt);
        attackPressed = Input.GetMouseButtonDown(0);

        // 冷却
        if (rollCooldownTimer   > 0f) rollCooldownTimer   -= Time.deltaTime;
        if (attackCooldownTimer  > 0f) attackCooldownTimer -= Time.deltaTime;

        // 决策
        StateType best = DetermineBestState();
        ActionPriority bestPri = GetPriority(best);

        if (bestPri != currentPriority)
            SwitchState(GetState(best));

        currentState?.Update();

        debugState = currentState?.Type.ToString();
    }

    private void FixedUpdate()
    {
        currentState?.FixedUpdate();

        // 在状态移动之后加力，不被覆盖
        if (pendingForce.sqrMagnitude > 0.01f)
        {
            rb.AddForce(pendingForce, ForceMode.Impulse);
            pendingForce = Vector3.zero;
        }
    }

    // ==================== 决策 ====================

    private StateType DetermineBestState()
    {
        if (currentState?.Type == StateType.Attack) return StateType.Attack;
        if (currentState?.Type == StateType.Roll)   return StateType.Roll;
        if (currentState?.Type == StateType.Jump)   return StateType.Jump;  // 空中锁住

        if (rollPressed && rollCooldownTimer <= 0f) return StateType.Roll;
        if (attackPressed && attackCooldownTimer <= 0f) return StateType.Attack;
        if (jumpPressed && IsGrounded()) return StateType.Jump;
        if (runHeld && moveInput.sqrMagnitude > 0.01f) return StateType.Run;
        if (moveInput.sqrMagnitude > 0.01f) return StateType.Walk;

        return StateType.Idle;
    }

    public bool IsGrounded() => groundCheck != null && groundCheck.isGrounded;

    private ActionPriority GetPriority(StateType type) => type switch
    {
        StateType.Idle   => ActionPriority.Idle,
        StateType.Walk   => ActionPriority.Walk,
        StateType.Run    => ActionPriority.Run,
        StateType.Jump   => ActionPriority.Jump,
        StateType.Attack => ActionPriority.Attack,
        StateType.Roll   => ActionPriority.Roll,
        _                => ActionPriority.None
    };

    private PlayerState GetState(StateType type) => type switch
    {
        StateType.Idle   => idleState,
        StateType.Walk   => walk,
        StateType.Run    => run,
        StateType.Jump   => jump,
        StateType.Attack => attack,
        StateType.Roll   => roll,
        _                => idleState
    };

    // ==================== 切换 ====================

    public void SwitchState(PlayerState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit();
        currentState = newState;
        currentPriority = GetPriority(newState.Type);
        currentState?.Enter();
    }

    // ==================== 动画事件（Animation 窗口调用） ====================

    /// <summary>attack1 动画事件：开始突刺</summary>
    public void OnAnimAttackDash()
    {
        (currentState as AttackState)?.TriggerDash();
    }

    /// <summary>attack1 动画事件：攻击结束，停顿后回 Idle</summary>
    public void OnAnimAttackEnd()
    {
        if (currentState?.Type == StateType.Attack)
            StartCoroutine(ExitAttackAfterDelay());
    }

    System.Collections.IEnumerator ExitAttackAfterDelay()
    {
        yield return new WaitForSeconds(attackExitDelay);
        if (currentState?.Type == StateType.Attack)
            SwitchState(idleState);
    }

    // ==================== 公用 ====================

    public Vector3 GetMoveDirection()
    {
        // 直接从摄像机当前位置实时算，避免 FixedUpdate/LateUpdate 时序抖动
        if (Camera.main == null) return Vector3.forward;

        Transform cam = Camera.main.transform;
        Vector3 forward = cam.forward; forward.y = 0f; forward.Normalize();
        Vector3 right   = cam.right;   right.y   = 0f; right.Normalize();

        Vector3 dir = forward * moveInput.y + right * moveInput.x;
        if (dir.sqrMagnitude > 1f) dir.Normalize();
        return dir;
    }

    public void RotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f) return;
        Quaternion target = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.fixedDeltaTime);
    }

    public void SetHorizontalVelocity(Vector3 dir, float speed)
    {
        Vector3 vel = dir * speed;
        vel.y = rb.velocity.y;
        rb.velocity = vel;
    }
}
