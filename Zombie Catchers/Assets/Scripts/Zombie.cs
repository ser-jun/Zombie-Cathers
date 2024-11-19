using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    #region variables
    private Animator animator;
    private bool isDead = false;
    private bool isEating = true;
    private bool isGrounted = true;
    private bool isObstacle = false;
    private bool isGroundUnderLegs = false;
    private bool canJump = true;
    private Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    public Transform target;
    public Transform playerTransform;
    public Transform leftTarget;
    public Transform rightTarget;   
    private float moveSpeed = 2f;
    public Transform brainTransform;
    [SerializeField] float maxDistacneToPlayer; 

    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float distanceToObstacle = 1f;
    [SerializeField] private float distanceToGround = 1f;
    Player player;
    bool isLookedPlayer;
    #endregion
    private void Start()
    {
        player = FindObjectOfType<Player>();
        playerTransform = player.transform;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    private void Update()
    {
        if (!isDead)
        {

            EatingBrain();
        }
        if (!isDead && !isEating)
        {
            Transform tryTarget = CheckingPlayerInRadius();
            if (tryTarget != null) { target = tryTarget; }
            MoveZombie(target);

        }

    }
    private Transform CheckClosesPoint()
    {


        float distanceToLeft = Vector2.Distance(transform.position, leftTarget.position);
        float distanceToRight = Vector2.Distance(transform.position, rightTarget.position);
        return distanceToLeft <= distanceToRight ? leftTarget : rightTarget;

    }
    public Transform CheckingPlayerInRadius()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer < maxDistacneToPlayer)
        {

            isLookedPlayer = true;
            bool isFacingRight = transform.position.x - playerTransform.position.x < 0;
            if (isFacingRight)
            {
                return leftTarget;
            }
            else { return rightTarget; }
        }
        if (!isLookedPlayer)
        {

            return CheckClosesPoint();
        }
        else
        {
            return null;
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
                CountKilledZombies.Instance.IncrementKillCount();
                rb.freezeRotation = false;
                capsuleCollider.size = new Vector2(0.3f, 1f);
                isDead = true;
                
            }
        }
    }
    
    private void MoveZombie(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;
        DrawRaycast(direction);
        WalkingAndJumpCondition(direction);
    }
    private void DrawRaycast(Vector2 direction)
    {
        isGrounted = Physics2D.Raycast(transform.position, Vector2.down, distanceToGround, groundLayer);
        Debug.DrawRay(transform.position, Vector2.down * distanceToGround, isGrounted ? Color.blue : Color.red);

        Vector2 rayDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        isObstacle = Physics2D.Raycast(transform.position, rayDirection, distanceToObstacle, groundLayer);
        Debug.DrawRay(transform.position, rayDirection * distanceToObstacle, isObstacle ? Color.blue : Color.red);

        Vector2 underLegs = transform.position + new Vector3(rayDirection.x * distanceToObstacle, 0, 0);
        isGroundUnderLegs = Physics2D.Raycast(underLegs, Vector2.down, distanceToGround, groundLayer);
        Debug.DrawRay(underLegs, Vector2.down * distanceToGround, isGroundUnderLegs ? Color.blue : Color.red);

        animator.SetBool("isMoving", direction.magnitude > 0);
        animator.SetBool("beforeBrain", false);
    }
    private void WalkingAndJumpCondition(Vector2 direction)
    {
        if (direction.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = direction.x > 0 ? 1f : -1f;
            transform.localScale = scale;
        }
        if (isObstacle && isGrounted && canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
        }
        else if (isGrounted && !isGroundUnderLegs)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canJump = false;
        }
        else
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        canJump = true;
    }
    private void EatingBrain()
    {
        if (brainTransform == null)
        {
            isEating = false; return;
        }
        if (Vector3.Distance(transform.position, brainTransform.position) > 1)
        {
            MoveZombie(brainTransform);
        }
        else
        {
            animator.SetBool("beforeBrain", true);
            animator.SetBool("isMoving", false);
        }
    }

}
