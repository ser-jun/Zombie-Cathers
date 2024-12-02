using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Shot shot;
    void Start()
    {

        shot = GetComponentInParent<Shot>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

        //if (shot.isFire)
        //{
            if (collision.CompareTag("Zombie"))
            {
                Zombie zombie = collision.GetComponent<Zombie>();
                if (!zombie.IsDead())
                {
                    zombie.Die();
                Destroy(gameObject);
                }

            }
            if (collision.CompareTag("Target") || collision.CompareTag("Ground"))
            {

                Destroy(gameObject);

            }
        //}
    }
    //if (collision.CompareTag("Target")  || collision.CompareTag("Ground"))
    //{
    //    Destroy(gameObject);

    //}
}
