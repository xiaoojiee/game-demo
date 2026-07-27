using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    [Header("飞行参数")]
    public float moveSpeed = 12f;
    public float MaxTime = 4f;

    [Header("触发模式开关")]
    public bool enableCollisionTrigger = true;
    public bool enableMouseTrigger = true;

    private Vector2 _moveDir;
    public LayerMask Layer;
    private float _lifeTimer;
    private ExplosionHandle _explosionHandle;
    private Camera _mainCam;

    private void Awake()
    {
        _explosionHandle = GetComponent<ExplosionHandle>();
        _mainCam = Camera.main;
    }

    public void Init(Vector2 direction, Tilemap targetTilemap)
    {
        _moveDir = direction.normalized;
        _lifeTimer = MaxTime;
        _explosionHandle?.SetTargetTilemap(targetTilemap);
    }

    private void Update()
    {
        transform.Translate(_moveDir * moveSpeed * Time.deltaTime, Space.World);

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0)
        {
            _explosionHandle?.ExecuteExplosion(transform.position);
            
        }

        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enableCollisionTrigger)
            return;
        if((Layer.value&(1<<other.gameObject.layer)) == 0)return;
        _explosionHandle?.ExecuteExplosion(transform.position);
        Destroy(gameObject);
    }
}