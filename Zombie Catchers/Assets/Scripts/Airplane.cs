using UnityEngine;

public class Airplane : MonoBehaviour
{
    public GameObject airplanePrefab;
    private float moveSpeed = 4f;
    private float moveOnY = 0.5f;
    private float fixedY = 7f;
    private Rigidbody2D rb;
    Player player;
    private bool isPlayerAttached = true;
    private float targetX = -10f;
    private float stopThreshold = 0.1f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private float scaleSpeed = 2f;
    private bool isScaling = false;
    private Vector3 targetPositionOffset;

    private Animator animator;
    public LayerMask groundLayer;
    private float checkDistance = 3f;
    private float upSpeed = 2f;
    private float downSpeed = 1f;

    private float currentVelocityX = 0f;  
    private float inertiaSmoothTime = 0.3f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        fixedY = airplanePrefab.transform.position.y;
        originalScale = airplanePrefab.transform.localScale;
        targetScale = originalScale * 0.5f;
        targetPositionOffset = airplanePrefab.transform.position + new Vector3(0, -2f, 0);
        animator = GetComponent<Animator>();
        AttachedPlayerToAirplain();
    }

    void Update()
    {
        if (isPlayerAttached)
        {
            if (Mathf.Abs(airplanePrefab.transform.position.x - targetX) > stopThreshold)
            {
                MoveAirplane();
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("isMoving", false);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UnattachPlayer();
            }
        }
        else
        {
            FollowThePlayer();
            DecreasePlain();
        }
        CheckGround();
        if (Mathf.Abs(rb.velocity.x) > 0.1f || Mathf.Abs(rb.velocity.y) > 0.1f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }

    }
    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * 1.5f, Vector2.down, checkDistance, groundLayer);

        if (hit.collider)
        {
            float distanceToGround = hit.distance;
            if (distanceToGround > 2.5f)
            {
                rb.velocity += new Vector2(0, upSpeed * Time.deltaTime);
            }
        }
        else
        {

            rb.velocity -= new Vector2(0, downSpeed * Time.deltaTime);
        }
    }
    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.right * 1.5f, transform.position + Vector3.right * 1.5f + Vector3.down * checkDistance);
    }
    private void MoveAirplane()
    {
        Vector3 moveDirection = new Vector3(moveSpeed, moveOnY, 0);
        rb.velocity = moveDirection;
        animator.SetBool("isMoving", Mathf.Abs(moveSpeed) > 0);


    }
    private void DecreasePlain()
    {
        if (isScaling)
        {

            airplanePrefab.transform.localScale = Vector3.Lerp(airplanePrefab.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

            Vector3 directionToTarget = (targetPositionOffset - airplanePrefab.transform.position).normalized;

            rb.velocity = new Vector2(0, directionToTarget.y * scaleSpeed);


            if (Vector3.Distance(airplanePrefab.transform.localScale, targetScale) < 0.01f &&
                Vector3.Distance(airplanePrefab.transform.position, targetPositionOffset) < 0.01f)
            {
                airplanePrefab.transform.localScale = targetScale;
                airplanePrefab.transform.position = targetPositionOffset;
                rb.velocity = Vector2.zero;
                isScaling = false;
            }
        }
    }

    private void FollowThePlayer()
    {
        float targetVelocityX = player.transform.position.x - transform.position.x;

        currentVelocityX = Mathf.SmoothDamp(currentVelocityX, targetVelocityX * moveSpeed, ref inertiaSmoothTime, 0.3f);
        rb.velocity = new Vector2(currentVelocityX, rb.velocity.y);
        animator.SetBool("isMoving", Mathf.Abs(currentVelocityX) > 0.1f);
    }

    private void AttachedPlayerToAirplain()
    {
        player.transform.SetParent(airplanePrefab.transform);
        player.rb.isKinematic = true;
        player.rb.gravityScale = 0;
        isPlayerAttached = true;

    }
    private void UnattachPlayer()
    {
        player.transform.SetParent(null);
        player.rb.gravityScale = 1;
        player.rb.isKinematic = false;
        isPlayerAttached = false;
        isScaling = true;
        targetPositionOffset = airplanePrefab.transform.position + new Vector3(0, -3f, 0);
    }
}
