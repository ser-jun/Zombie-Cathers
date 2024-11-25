using UnityEngine;

public class Airplane : MonoBehaviour
{
    public GameObject airplanePrefab;
    private float moveSpeed = 4f;
    private float moveOnY = 0.5f;
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
    [SerializeField] float flightHeigth;

    private bool isMoving = true;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        originalScale = airplanePrefab.transform.localScale;
        targetScale = originalScale * 0.5f;
        targetPositionOffset = airplanePrefab.transform.position + new Vector3(0, -2f, 0);
        animator = GetComponent<Animator>();
        AttachedPlayerToAirplain();
    }

    void Update()
    {
        animator.SetBool("isMoving", isMoving);
        if (isPlayerAttached)
        {
            if (Mathf.Abs(airplanePrefab.transform.position.x - targetX) > stopThreshold)
            {
                MoveAirplane();
            }
            else
            {
                isMoving = false;
            }
          
            if (Input.GetKeyDown(KeyCode.Space))
            {
                UnattachPlayer();
            }
        }
        else
        {
            FollowThePlayer();
            ScaleAirplain();
        }
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.right * 1.5f, transform.position + Vector3.right * 1.5f + Vector3.down * checkDistance);
    }
    private void MoveAirplane()
    {
        Vector3 previousPosition = transform.position;
        Vector3 moveDirection = new Vector3(moveSpeed, moveOnY, 0);
        transform.position += moveDirection * Time.deltaTime;
        isMoving = Vector3.Distance(transform.position, previousPosition) > 0.001f;
    }

    private void ScaleAirplain()
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
        Vector3 previousPosition = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.up * 100), Vector2.down, Mathf.Infinity, groundLayer);
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, upSpeed * Time.deltaTime),
            Mathf.Lerp(transform.position.y, hit.point.y + flightHeigth, upSpeed * Time.deltaTime), transform.position.z);
        isMoving = Vector3.Distance(transform.position, previousPosition) > 0.001f;
    }

    private void AttachedPlayerToAirplain()
    {
        player.transform.SetParent(airplanePrefab.transform);
        player.rb.isKinematic = true;
        player.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        player.rb.gravityScale = 0;
        isPlayerAttached = true;

    }
    private void UnattachPlayer()
    {
        player.transform.SetParent(null);
        player.rb.gravityScale = 1;
        player.rb.isKinematic = false;
        player.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        isPlayerAttached = false;
        GetComponent<BoxCollider2D>().enabled = false;
        isScaling = true;
        targetPositionOffset = airplanePrefab.transform.position + new Vector3(0, -3f, 0);
    }
}
