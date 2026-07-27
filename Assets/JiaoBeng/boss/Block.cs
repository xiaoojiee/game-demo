using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour,Idamage
{
    public int Health;
    private int MaxHp;
    public Sprite[] crackSprite=new Sprite[3];
    private SpriteRenderer crackRenderer;
    private Rigidbody2D Rigidbody;
    private void Awake()
    {
        MaxHp = Health;
        Rigidbody = GetComponent<Rigidbody2D>();
        crackRenderer = transform.Find("CrackOverlay").GetComponent<SpriteRenderer>();
        if( crackRenderer != null)
        {
            crackRenderer.enabled = false;
        }
    }
    public void Hit(int damage)
    {
        MaxHp-=damage;
        UpdataCrackVisual();
        if (MaxHp <= 0)
        {
            BlockDie();
        }
    }
    private void UpdataCrackVisual()
    {
        if (crackSprite.Length<3||crackRenderer==null)return;
        float Percentage = (float)MaxHp / Health;
        if (Percentage >=1)
        {
            crackRenderer.enabled = false;
        }
        
        else if(Percentage > 0.66f)
        {
            crackRenderer.enabled = true;
            crackRenderer.sprite = crackSprite[0];
        }
        else if (Percentage > 0.33f)
        {
            crackRenderer.enabled = true;
            crackRenderer.sprite = crackSprite[1];
        }
        else if (Percentage >= 0f)
        {
            crackRenderer.enabled = true;
            crackRenderer.sprite = crackSprite[2];
        }

    }
    void BlockDie()
    {
        Destroy(gameObject);
    }
}
