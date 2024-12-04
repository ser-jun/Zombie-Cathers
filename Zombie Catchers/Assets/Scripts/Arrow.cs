using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Bullet
{
    private Shot shot;
    protected override void Start()
    {
        shot = GetComponentInParent<Shot>();
       
    }
    public void StartShot()
    {
        Destroy(gameObject, liveTimeBullet);
    }

    public override void OnTriggerEnter2D(Collider2D collision)
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
            if (collision.CompareTag("Ground"))
            {

                Destroy(gameObject);

            }
        }
    }
}
