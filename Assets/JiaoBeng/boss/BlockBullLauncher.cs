using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockBulletLauncher : MonoBehaviour
{
    [Header("自身冷却配置")]
    [Tooltip("该方块的发射冷却时间（秒）")]
    public float shootCooldown = 1.5f;

    [Header("发射点配置")]
    [Tooltip("发射点相对于方块中心的偏移")]
    public Vector2 shootOffset = Vector2.zero;

    private float lastShootTime;

    public void TryLaunch(Bullet bulletPrefab, float speed, Vector2 targetPos, GameObject blockPrefab, Tilemap tilemap)
    {
        if (Time.time < lastShootTime + shootCooldown) return;

        DoLaunch(bulletPrefab, speed, targetPos, blockPrefab, tilemap);
    }

    private void DoLaunch(Bullet bulletPrefab, float speed, Vector2 targetPos, GameObject blockPrefab, Tilemap tilemap)
    {
        Vector2 shootPos = (Vector2)transform.position + shootOffset;

        Bullet bullet = Instantiate(bulletPrefab, shootPos, Quaternion.identity);

        Vector2 direction = (targetPos - shootPos).normalized;

        bullet.Init(direction, speed, blockPrefab, tilemap);

        lastShootTime = Time.time;
    }
}