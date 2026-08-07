using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public LayerMask groundMask = 1;
    public float checkRadius = 0.3f;
    public Vector3 checkOffset = new Vector3(0, -0.9f, 0);

    public bool isGrounded { get; private set; }

    void Update()
    {
        Vector3 checkPos = transform.position + checkOffset;
        isGrounded = Physics.CheckSphere(checkPos, checkRadius, groundMask);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 checkPos = transform.position + checkOffset;
        Gizmos.DrawWireSphere(checkPos, checkRadius);
    }
}
