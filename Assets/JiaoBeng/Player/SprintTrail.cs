using UnityEngine;

public class SprintTrail : MonoBehaviour
{
    [Header("爆发（子物体 PaintSpawner: OnEnable）")]
    public PaintSpawner burstSpawner;

    [Header("拖尾（子物体 PaintSpawner: Manual）")]
    public PaintSpawner trailSpawner;
    public float trailInterval = 0.5f;

    private Vector3 _lastTrailPos;
    private bool _firstFrame;

    public int EstimatedTotalCost
    {
        get
        {
            int cost = 0;
            if (burstSpawner != null)
                cost += Mathf.CeilToInt(burstSpawner.dropCount * burstSpawner.paintChance);
            if (trailSpawner != null)
            {
                int spawns = Mathf.CeilToInt(0.2f * 25f / trailInterval);
                cost += Mathf.CeilToInt(trailSpawner.dropCount * spawns * trailSpawner.paintChance);
            }
            return cost;
        }
    }

    private void OnEnable()
    {
        _firstFrame = true;
        // burstSpawner 设了 TriggerMode=OnEnable，自动触发
        _lastTrailPos = transform.position;
    }

    private void Update()
    {
        if (trailSpawner == null) return;
        if (_firstFrame) { _firstFrame = false; return; }

        float dist = Vector2.Distance(transform.position, _lastTrailPos);
        if (dist >= trailInterval)
        {
            _lastTrailPos = transform.position;
            trailSpawner.Launch();
        }
    }
}
