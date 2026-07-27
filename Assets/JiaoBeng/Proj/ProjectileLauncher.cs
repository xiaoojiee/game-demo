using UnityEngine;
using UnityEngine.Tilemaps;

public class ProjectileLauncher : MonoBehaviour
{
    [Header("发射配置")]
    public Transform launchPoint;
    public GameObject projectilePrefab;

    [Header("地图引用（场景里的Tilemap直接拖这里）")]
    public Tilemap targetTilemap;

    private Camera _mainCam;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Launch();
        }
    }

    private void Launch()
    {
        Vector3 mouseWorldPos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector2 direction = (mouseWorldPos - launchPoint.position).normalized;

        GameObject proj = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>()?.Init(direction, targetTilemap);
    }
}