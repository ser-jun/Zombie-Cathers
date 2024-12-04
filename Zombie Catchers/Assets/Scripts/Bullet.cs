using UnityEngine;

public class Bullet : MonoBehaviour
{

    public float liveTimeBullet = 0.3f; 
    protected virtual void Start()
    {
        Destroy(gameObject, liveTimeBullet);

    }


    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (!zombie.IsDead())
            {
                zombie.Die();
                Destroy(gameObject);
            }

        }
        if (collision.CompareTag("Ground"))
        {

            Destroy(gameObject);

        }
        
    }
  
}
