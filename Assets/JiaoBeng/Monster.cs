using UnityEngine;

public class Monster : MonoBehaviour, Idamage
{
    [Header("怪物基础属性")]
    public int MonsterHealth;
    private int HP_0;

    void Awake()
    {
        HP_0 = MonsterHealth;
    }

    public void Hit(int Hit)
    {
        HP_0 -= Hit;
        

        if (HP_0 <= 0)
        {
            MonsterDie();
        }
    }

    void MonsterDie()
    {
        Destroy(gameObject);
    }
}