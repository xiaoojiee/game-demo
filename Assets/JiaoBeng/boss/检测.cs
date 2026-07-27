using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class 检测 : MonoBehaviour
{
    [Header("检测配置")]
    public float 半径 = 8f;
    public float 间隔 = 0.1f;
    public LayerMask playerLayer;
    public bool playerInRange;
    public Transform player;
    private float checkTimer;
    private void Start()
    {
        
    }
    private void Update()
    {
        checkTimer += Time.deltaTime;
        if(checkTimer >= 间隔)
        {
            checkTimer= 0;

        }
    }
    public void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            player =playerObj.transform;
        }
    }
    void CheckPlayerInRange()
    {
        if (player == null)
        {
            FindPlayer();
            playerInRange = false;
            return;
        }
        float distance=Vector2.Distance(transform.position, player.position);
        playerInRange = distance <= 半径;
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,半径);
    }
}
