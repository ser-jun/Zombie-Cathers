using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Shot shot;
    void Start()
    {
        
        shot = GetComponentInParent<Shot>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (shot.isFire)
        {
            if (collision.CompareTag("Zombie"))
            {
                Zombie zombie = collision.GetComponent<Zombie>();
                if (!zombie.IsDead())
                {
                    zombie.Die();
                }

            }
            if (collision.CompareTag("Target") || collision.CompareTag("Ground"))
            {

                Destroy(gameObject);

            }
        }
    }
}
