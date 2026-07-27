using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class SlashPaintEffect : MonoBehaviour
{
    public int directionalCout = 15;
    public int monsterCount = 20;
    public Color panitcolor;
   

    private void Start()
    {
        
        Vector2 attackDir = transform.right;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        panitcolor = sr.color;
        
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster"))
        {
            PaintManager.Instance.SplashMonster(collision.transform.position, monsterCount, panitcolor);
            enabled = false;
        }
    }
}
