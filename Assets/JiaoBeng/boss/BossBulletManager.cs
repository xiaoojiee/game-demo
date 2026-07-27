using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

public class BossBulletManager : MonoBehaviour
{
    [Header("发射配置（核心统管）")]
    [Tooltip("弹幕预制体（需挂载Bullet脚本）")]
    public Bullet bulletPrefab;
    [Tooltip("弹幕飞行速度")]
    public float bulletSpeed = 12f;

    [Header("地面生成配置")]
    [Tooltip("命中地面后生成的方块预制体")]
    public GameObject groundBlockPrefab;
    [Tooltip("目标TileMap，用于对齐网格")]
    public Tilemap targetTilemap;

    [Header("引用缓存")]
    [Tooltip("关联的Boss核心组件，用于获取环绕方块列表")]
    public BossCore bossCore;

    private Transform player;

    private void Start()
    {
        if (bossCore == null)
        {
            bossCore = GetComponent<BossCore>();
            if (bossCore == null)
            {
                enabled = false;
                return;
            }
        }
        if (bulletPrefab == null)
        {
            enabled = false;
            return;
        }
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            enabled = false;
        }
    }

    private void Update()
    {
        if (player == null || bossCore.blocks.Count <= 0) return;

        BossPjo nearestBlock = FindNearestBlock();
        if (nearestBlock == null) return;

        BlockBulletLauncher launcher = nearestBlock.GetComponent<BlockBulletLauncher>();
        if (launcher != null)
        {
            launcher.TryLaunch(bulletPrefab, bulletSpeed, player.position, groundBlockPrefab, targetTilemap);
        }
    }

    BossPjo FindNearestBlock()
    {
        List<BossPjo> blockList = bossCore.blocks;
        BossPjo target = null;
        float minDis = float.MaxValue;

        for (int i = 0; i < blockList.Count; i++)
        {
            BossPjo b = blockList[i];
            if (b == null) continue;

            float dis = Vector2.Distance(b.transform.position, player.position);
            if (dis < minDis)
            {
                minDis = dis;
                target = b;
            }
        }
        return target;
    }
}