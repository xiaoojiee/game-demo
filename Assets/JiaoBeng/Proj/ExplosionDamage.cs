using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    public int damage;
    public float lifeTime;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       

        Idamage damageable = other.GetComponent<Idamage>();
        if (damageable != null)
        {
            damageable.Hit(damage);
            Destroy(gameObject);

        }
        
    }
}