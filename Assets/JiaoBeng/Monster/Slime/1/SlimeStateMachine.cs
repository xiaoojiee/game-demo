using UnityEngine;

public class SlimeStateMachine : MonoBehaviour
{
    [HideInInspector] public Transform Player;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public BoxCollider2D boxCollider2D;
    [HideInInspector] public Healh healh;

    [HideInInspector] public bool isAttack;

    [Header("攻击碰撞体子物体")]
    public GameObject attackTrigger;

    [Header("地面检测")]
    public LayerMask groundLayer;

    public bool isGrounded { get; private set; }
    private bool wasGrounded;

    [Header("检测范围")]
    [Tooltip("Chase")]
    public float detectionRange = 10f;
    [Tooltip("Attack")]
    public float attackRange = 2f;
    [Tooltip("Attack Cooldown")]
    public float AttackCooldown = 2f;
    [HideInInspector] public float AttackCooldownTimer;
    [Tooltip("Patrol Cooldown")]
    public float PatrolCooldown = 0.5f;
    [HideInInspector] public float PatrolCooldownTimer;
    [Tooltip("Chase Cooldown")]
    public float ChaseCooldown = 0.8f;
    [HideInInspector] public float ChaseCooldownTimer;
    [Tooltip("受击硬直时长（秒）")]
    public float hurtDuration = 0.3f;

    [Header("小跳")]
    public float patrolJumpForce = 7f;
    public float patrolHorizontalForce = 3f;
    [Header("中跳")]
    public float chaseJumpForce = 9f;
    public float chaseHorizontalForce = 6f;
    [Header("大跳")]
    public float attackJumpForce = 15f;
    public float attackHorizontalForce = 8f;

    [Header("空中微调")]
    [Tooltip("每秒调整的水平速度量")]
    public float airControlForce = 3f;

    [Header("地面待机")]
    public float idleDurationMin = 1.5f;
    public float idleDurationMax = 3f;

    [Header("落地消除颜料")]
    [Tooltip("普通落地衰减率（0~1，越接近1圈数越多）")]
    [Range(0.1f, 0.95f)] public float normalClearDecay = 0.5f;
    [Tooltip("攻击落地衰减率")]
    [Range(0.1f, 0.95f)] public float attackClearDecay = 0.7f;
    [Tooltip("最小强度阈值（两个共用）")]
    [Range(0.001f, 0.1f)] public float clearMinIntensity = 0.01f;

    private SlimeState currentState;
    [HideInInspector] public SlimeIdleState idleState;
    [HideInInspector] public SlimePatrolState patrolState;
    [HideInInspector] public SlimeChaseState chaseState;
    [HideInInspector] public SlimeAttackState attackState;
    [HideInInspector] public SlimeFallState fallState;
    [HideInInspector] public SlimeHurtState hurtState;
    [HideInInspector] public SlimeDieState dieState;

    private bool hurtPending;
    private bool diePending;

    // 获取组件+初始化状态+找玩家
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        healh = GetComponent<Healh>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            Player = playerObj.transform;

        idleState   = new SlimeIdleState(this);
        patrolState = new SlimePatrolState(this);
        chaseState  = new SlimeChaseState(this);
        attackState = new SlimeAttackState(this);
        fallState   = new SlimeFallState(this);
        hurtState   = new SlimeHurtState(this);
        dieState    = new SlimeDieState(this);

        currentState = idleState;
        currentState.Enter();

        if (attackTrigger != null)
            attackTrigger.SetActive(false);
    }

    void Start()
    {
        if (healh != null)
        {
            healh.OnHurt += OnHurtHandler;
            healh.OnDeath += OnDeathHandler;
        }
    }

    void OnDisable()
    {
        if (healh != null)
        {
            healh.OnHurt -= OnHurtHandler;
            healh.OnDeath -= OnDeathHandler;
        }
    }

    private void OnHurtHandler(Damage damage)
    {
        hurtPending = true;
    }

    private void OnDeathHandler(Transform killer)
    {
        diePending = true;
    }

    // 地面检测+冷却+决策+驱动
    void Update()
    {
        isGrounded = boxCollider2D.IsTouchingLayers(groundLayer);

        if (AttackCooldownTimer > 0f)
            AttackCooldownTimer -= Time.deltaTime;
        if (PatrolCooldownTimer > 0f)
            PatrolCooldownTimer -= Time.deltaTime;
        if (ChaseCooldownTimer > 0f)
            ChaseCooldownTimer -= Time.deltaTime;

        // 优先级决策
        EvaluateStateTransition();

        currentState?.Update();

        wasGrounded = isGrounded;
    }

    // 安全播放动画
    public void PlayAnim(string stateName)
    {
        if (anim == null) return;

        int hash = Animator.StringToHash(stateName);
        if (anim.HasState(0, hash))
        {
            anim.Play(stateName);
        }
    }

    // 优先级决策
    private void EvaluateStateTransition()
    {
        int bestPriority = -1;
        SlimeState bestState = null;

        if (diePending && bestPriority < 100)
        {
            if (currentState != dieState)
            {
                bestPriority = 100;
                bestState = dieState;
            }
            else
            {
                diePending = false;
            }
        }

        if (hurtPending && currentState != hurtState && currentState != dieState && bestPriority < 90)
        {
            bestPriority = 90;
            bestState = hurtState;
        }

        if (currentState == hurtState && hurtPending)
        {
            hurtPending = false;
        }

        bool justLanded = !wasGrounded && isGrounded;
        bool isFallOrJump = currentState == fallState
                         || currentState == patrolState
                         || currentState == chaseState
                         || currentState == attackState;

        if (justLanded && isFallOrJump && bestPriority < 15)
        {
            var clear = GetComponent<PaintClear>();
            if (clear != null && isAttack)
            {
                clear.ClearPaintAt(transform.position, attackClearDecay, clearMinIntensity);
            }

            bestPriority = 15;
            bestState = idleState;
        }

        bool isJumping = currentState == patrolState
                      || currentState == chaseState
                      || currentState == attackState;

        if (isJumping && rb.velocity.y < -0.1f && bestPriority < 14)
        {
            bestPriority = 14;
            bestState = fallState;
        }

        if (currentState == hurtState && hurtState.HurtTimer <= 0f && bestPriority < 13)
        {
            bestPriority = 13;
            bestState = idleState;
        }

        if (currentState == idleState && isGrounded)
        {
            if (Player != null)
            {
                float distance = Vector2.Distance(transform.position, Player.position);

                if (distance <= attackRange && AttackCooldownTimer <= 0f && bestPriority < 40)
                {
                    bestPriority = 40;
                    bestState = attackState;
                }

                if (distance <= detectionRange && ChaseCooldownTimer <= 0f && bestPriority < 30)
                {
                    bestPriority = 30;
                    bestState = chaseState;
                }
            }

            if (idleState.IdleTimer <= 0f && PatrolCooldownTimer <= 0f && bestPriority < 20)
            {
                bestPriority = 20;
                bestState = patrolState;
            }
        }

        if (bestState != null && bestState != currentState)
        {
            // 切换状态
            ChangeState(bestState);
        }
    }

    // 切换状态
    public void ChangeState(SlimeState newState)
    {
        if (newState == null || currentState == newState)
            return;

        if (newState == idleState)
        {
            if (isAttack)
            {
                AttackCooldownTimer = AttackCooldown;
            }
            isAttack = false;
            hurtPending = false;

            if (currentState == hurtState)
            {
                idleState.quickRecovery = true;
            }
        }

        if (newState == hurtState)
        {
            hurtPending = false;

            if (anim != null)
                anim.speed = 1f;
        }

        if (newState == patrolState)
            PatrolCooldownTimer = PatrolCooldown;

        if (newState == chaseState)
            ChaseCooldownTimer = ChaseCooldown;

        if (newState == dieState)
        {
            diePending = false;
            isAttack = false;
        }

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
