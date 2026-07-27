using UnityEngine;
using System.Collections.Generic;

public class BossCore : MonoBehaviour
{
    [Header("预制体引用")]
    public BossPjo blockPrefab;

    [Header("旋转参数")]
    [Tooltip("旋转半径：中心到方块的距离")]
    public float orbitRadius = 2f;
    [Tooltip("旋转速度：度/秒，正数逆时针，负数顺时针")]
    public float rotateSpeed = 90f;
    [Tooltip("方块数量：默认4个正好是正方形四角")]
    public int blockCount = 4;

    [Header("运行状态")]
    public List<BossPjo> blocks = new List<BossPjo>();

    private float currentAngleRad;

    private void Start()
    {
        if (blockPrefab == null)
        {
            enabled = false;
            return;
        }
        SpawnAllBlock();
    }

    private void Update()
    {
        if (blockPrefab == null) return;
        currentAngleRad += rotateSpeed * Mathf.Deg2Rad * Time.deltaTime;

        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            if (blocks[i] != null)
            {
                blocks[i].UpdatePosition(currentAngleRad);
            }
            else
            {
                blocks.RemoveAt(i);
            }
        }
    }

    void SpawnAllBlock()
    {
        float angleStepRad = (Mathf.PI * 2f) / blockCount;

        for (int i = 0; i < blockCount; i++)
        {
            BossPjo newBlock = Instantiate(blockPrefab, transform.position, Quaternion.identity, transform);
            float offset = angleStepRad * i;

            newBlock.Init(transform, orbitRadius, offset);
            blocks.Add(newBlock);
        }
    }
}