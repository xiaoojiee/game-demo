using UnityEngine;

public class RandomizeHealth : MonoBehaviour
{
    [Header("血量范围")]
    public int minHp = 10;
    public int maxHp = 100;

    // 随机设maxHp
    private void Awake()
    {
        var health = GetComponent<Healh>();
        if (health != null)
        {
            health.maxHp = Random.Range(minHp, maxHp + 1);
        }
    }
}
