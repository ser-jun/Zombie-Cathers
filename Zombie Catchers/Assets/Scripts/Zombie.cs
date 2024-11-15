using UnityEngine;

public class Zombie : MonoBehaviour
{
    private Animator animator;
    private bool isDead = false;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    public Transform target;
    private float moveSpeed = 2f;
    
 
    private void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Update()
    {
        if (!isDead)
        {
            MoveZombie();
          
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            if (!isDead)
            {
                animator.SetTrigger("Dead");
                Destroy(collision.gameObject);
                rb.freezeRotation = false;
                capsuleCollider.size = new Vector2(0.3f, 1f);
                isDead = true;
            }
        }
    }
    private void MoveZombie()
    {

        Vector2 direction = (target.position - transform.position).normalized;
        animator.SetBool("isMoving", direction.magnitude > 0);
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction.x > 0 ? 1f : -1f;
            transform.localScale = scale;   
        }
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }
    private void OnCollisionEnter2D(Collision2D collision) //Need fix
    {
        if (collision.gameObject.CompareTag("Brain"))
        {
            animator.SetBool("isMoving", false);
            animator.SetBool("beforeBrain", true);
       
        }
    }
}
