using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dart : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] Rigidbody2D rb;
    float shiftAngle = -26; 

    private void Update()
    {
        if (rb == null || rb.velocity.normalized == Vector2.zero) return;

        float angle = Mathf.Atan2(rb.velocity.normalized.y, rb.velocity.normalized.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + shiftAngle);
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Zombie"))
        {
            Zombie zombie = collision.GetComponent<Zombie>();
            if (!zombie.IsDead())
            {
                zombie.Die();
            }
            col.enabled = false;
            Destroy(rb);
            transform.SetParent(zombie.transform);

        }
        if (collision.CompareTag("Ground"))
        {
            col.enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

    }

}
