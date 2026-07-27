using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public Vector2 v2;
    public float speed;
    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()=>SpriteRenderer.material.mainTextureOffset += v2*speed*Time.deltaTime;
}
