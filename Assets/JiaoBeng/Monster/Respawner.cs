using UnityEngine;
using System.Collections;

public class Respawner : MonoBehaviour
{
    [Header("史莱姆预制体")]
    public GameObject prefab;

    [Header("生成参数")]
    public int minSpawn = 2;
    public int maxSpawn = 4;
    public float spawnRadius = 5f;
    public float respawnDelay = 2f;
    [Header("血量范围")]
    public int minHp = 25;
    public int maxHp = 100;

    private int _aliveCount;
    private Coroutine _checkRoutine;

    private void Start()
    {
        // 生成一波史莱姆
        SpawnWave();
    }

    // 生成一波史莱姆
    public void SpawnWave()
    {
        int count = Random.Range(minSpawn, maxSpawn + 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 pos = transform.position + new Vector3(offset.x, offset.y, 0);
            var obj = Instantiate(prefab, pos, Quaternion.identity);

            var health = obj.GetComponent<Healh>();
            if (health != null)
            {
                health.maxHp = Random.Range(minHp, maxHp + 1);
                _aliveCount++;
                health.OnDeath += _ =>
                {
                    _aliveCount--;
                };
            }
        }
    }

    // 全死→延迟重生
    private void Update()
    {
        if (_aliveCount <= 0 && _checkRoutine == null)
        {
            _checkRoutine = StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        // 生成一波史莱姆
        SpawnWave();
        _checkRoutine = null;
    }
}
