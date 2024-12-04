using UnityEngine;
using System.Collections;

public class Zombie : MonoBehaviour
{
    #region variables
    private Animator animator;
    public TileGeneration tileGeneration;

    private bool isDead = false;
    public bool IsDead () => isDead;   
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

    [SerializeField] private float jumpForce =5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float distanceToObstacle = 1f;
    [SerializeField] private float distanceToGround = 1f;
    Player player;
    bool isLookedPlayer;

    public GameObject coinPrefab;
    private AudioSource audioSource;
    public AudioClip soundZombie;
    private bool hasPlayed=false;
    public AudioClip eatingBrainSound;
    public AudioClip dieZombieSound;
    private float minY = -5f;
    #endregion
    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        tileGeneration = FindObjectOfType<TileGeneration>();    
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
        CheckPositionLimit();
        DestroyZombies();
        CountKilledZombies.Instance.UpdateKillCount();
    }
    private void GenerateCoins()
    {
        float spawnRadius = 2f;
        int count = Random.Range(2, 8);
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = transform.position + new Vector3 (Random.Range(-spawnRadius,spawnRadius), Random.Range(-spawnRadius, spawnRadius),0);
            GameObject coin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);

            Rigidbody2D rbCoin = coin.GetComponent<Rigidbody2D>();
            rbCoin.AddForce(new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 5f)), ForceMode2D.Impulse);
        }
    }
    public void Die()
    {
        isDead = true;
        animator.SetTrigger("Dead");
        audioSource.PlayOneShot(dieZombieSound);
        CountKilledZombies.Instance.IncrementKillCount();
        GenerateCoins();
        rb.freezeRotation = false;
        capsuleCollider.size = new Vector2(0.3f, 1f);
        Airplane airPlane = FindObjectOfType<Airplane>();
        airPlane.GetPositionDeadZombie(this.transform);
        

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
            if (!hasPlayed)
            {
            audioSource.PlayOneShot(soundZombie);
                hasPlayed = true;
            }
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
    private bool CheckPlayerWhileEating() 
    {
        if (brainTransform == null) return false; 

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        return distanceToPlayer < maxDistacneToPlayer; 
    }

  

    private void MoveZombie(Transform target)
    {
        Vector2 direction = (target.position - transform.position).normalized;
        DrawRaycast(direction);
        WalkingAndJumpCondition(direction);
    }
    private void DestroyZombies()
    {
        if (transform.position.y < -3f && !isDead)
        {
            Destroy(gameObject);
            tileGeneration.countZombies--;
        }
    }
    private void CheckPositionLimit()
    {
        if (transform.position.y < minY && isDead)
        {
            Vector3 newPosition = transform.position;
            newPosition.y = minY;
            transform.position = newPosition;

            // Останавливаем падение, если есть Rigidbody2D
            if (rb != null)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
            }
        }
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
            rb.velocity = new Vector2(rb.velocity.x, 7.5f);
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
        if (brainTransform == null || CheckPlayerWhileEating())
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
            if (!audioSource.isPlaying)
            {
            audioSource.PlayOneShot(eatingBrainSound);
            }
            animator.SetBool("isMoving", false);
            Brain brain = brainTransform.GetComponent<Brain>();
            brain.StartEating();
        }
    }

}
