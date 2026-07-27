using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackMap : MonoBehaviour
{
    public Camera mainCamera;
    public float mapWidth;
    
    
    private void Start()
    {
        mainCamera = Camera.main;
        GetBgWidth();
    }
    private void Update()
    {
        BgMove();
    }
    private void GetBgWidth()
    {
        SpriteRenderer spriteRenderer= GetComponent<SpriteRenderer>();
        mapWidth=spriteRenderer.bounds.size.x;
    }
    private void BgMove()
    {
        float totalWidth = mainCamera.transform.position.x - transform.position.x;
        if (Mathf.Abs(totalWidth) > mapWidth)
        {
            transform.position += Vector3.right * mapWidth * 2 * Mathf.Sign(totalWidth);
        }
    }
}
