using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnockbackReceiver : MonoBehaviour
{
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyKnockback(Vector3 force)
    {
        if (force.sqrMagnitude > 0.01f)
            rb.AddForce(force, ForceMode.Impulse);
    }
}
