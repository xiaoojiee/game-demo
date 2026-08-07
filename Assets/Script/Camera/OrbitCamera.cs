using UnityEngine;

/// <summary>
/// 第三人称轨道摄像机（Elden Ring 风格）。
/// 玩家始终在屏幕中央，摄像机不会穿模到地下。
/// </summary>
public class OrbitCamera : MonoBehaviour
{
    [Header("目标")]
    public Transform target;

    [Header("距离")]
    public float distance = 6f;
    public float minDistance = 2f;
    public float maxDistance = 12f;
    public float zoomSpeed = 3f;

    [Header("旋转灵敏度")]
    public float rotateSpeed = 0.5f;   // 鼠标灵敏度，不用乘deltaTime

    [Header("上下限制")]
    [Range(-60f, 0f)]  public float minPitch = -40f;
    [Range(0f, 80f)]   public float maxPitch = 60f;

    [Header("自动跟随")]
    [Range(0f, 1f)] public float autoFollowStrength = 0.3f;

    [Header("碰撞检测（防止穿模）")]
    public LayerMask obstacleLayer = ~0;  // 默认检测所有层
    public float cameraRadius = 0.3f;     // 摄像机球体半径
    public float minCollisionDist = 0.5f;  // 离角色最近多远

    private float yaw;
    private float pitch;

    // 给 PlayerStateMachine 读的方向
    public Vector3 Forward { get; private set; }
    public Vector3 Right   { get; private set; }

    void Start()
    {
        Vector3 dir = transform.position - target.position;
        yaw   = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        pitch = Mathf.Asin(Mathf.Clamp(dir.y / Mathf.Max(dir.magnitude, 0.1f), -1f, 1f)) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void LateUpdate()
    {
        if (target == null) return;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            yaw   += Input.GetAxis("Mouse X") * rotateSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotateSpeed;
        }
        pitch  = Mathf.Clamp(pitch, minPitch, maxPitch);

        // —— 自动跟随（直接从 yaw 算方向，不依赖相机 Forward，消除反馈环） ——
        var ps = target.GetComponent<PlayerStateMachine>();
        if (ps != null && ps.moveInput.sqrMagnitude > 0.01f)
        {
            // 把玩家输入转成世界方向（用 yaw 角度算，不经过相机 transform）
            float rad = yaw * Mathf.Deg2Rad;
            Vector3 camForward = new Vector3(Mathf.Sin(rad), 0, Mathf.Cos(rad));
            Vector3 camRight   = new Vector3(Mathf.Cos(rad), 0, -Mathf.Sin(rad));
            Vector3 worldDir = camForward * ps.moveInput.normalized.y + camRight * ps.moveInput.normalized.x;

            if (worldDir.sqrMagnitude > 0.01f)
            {
                float targetYaw = Mathf.Atan2(worldDir.x, worldDir.z) * Mathf.Rad2Deg;
                float diff = Mathf.DeltaAngle(yaw, targetYaw);
                yaw += diff * autoFollowStrength;
            }
        }

        // —— 按键缩放（+/- 或 Keypad Plus/Minus） ——
        if (Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.KeypadPlus))
            distance = Mathf.Max(distance - zoomSpeed * Time.deltaTime, minDistance);
        if (Input.GetKey(KeyCode.Minus) || Input.GetKey(KeyCode.KeypadMinus))
            distance = Mathf.Min(distance + zoomSpeed * Time.deltaTime, maxDistance);

        // —— 碰撞检测：从角色到理想摄像机位置拉线 ——
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        Vector3 dirToCam = rot * Vector3.forward;  // 摄像机在 -Z 方向，forward 反着算

        float finalDist = distance;
        if (Physics.SphereCast(target.position, cameraRadius, -dirToCam, out RaycastHit hit, distance, obstacleLayer))
        {
            finalDist = Mathf.Max(hit.distance - cameraRadius, minCollisionDist);
        }

        // —— 直接设位置（轨道摄像机不需要平滑，角度已平滑） ——
        transform.position = target.position - dirToCam * finalDist;
        transform.LookAt(target.position);

        // —— 更新移动方向 ——
        Forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Right   = Vector3.ProjectOnPlane(transform.right,   Vector3.up).normalized;
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position, cameraRadius);
    }
}
