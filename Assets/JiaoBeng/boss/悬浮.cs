using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class 悬浮 : MonoBehaviour
{
    [Header("悬浮配置")]
    public float 悬浮高度 = 1f;
    public float 高度平滑 = 1f;
    public float 地面检测 = 1f;
    public LayerMask 图层;
    public Tilemap tilemap;
    private Rigidbody2D rb;
    private Collider2D Collider2D;
    public bool is漂浮 = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Collider2D =GetComponent<Collider2D>();
    }
    void FixedUpdate()
    {
        if (!is漂浮||tilemap==null)return    ;
        UpdateFloatHeight();
    }
    private void UpdateFloatHeight()
    {
        Vector2 rayOrigin =new Vector2(transform.position.x, Collider2D.bounds.min.y);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, 地面检测, 图层);
        if (hit == false) return;
        float height = tilemap.cellSize.y;
        float targetY = hit.point.y + 悬浮高度 * height;
        Vector2 targetpos = new Vector2(rb.position.x, targetY);
        rb.position = Vector2.Lerp(rb.position, targetpos, 高度平滑 * Time.fixedDeltaTime);
    }
    public void onFloating()
    {
        is漂浮 = true;
    }
    public void offFloating()
    {
        is漂浮 = false;
    }

}
