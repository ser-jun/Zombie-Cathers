using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    public GameObject airplanePrefab;

    private float moveSpeed = 6f;
    private float moveOnY = 0.5f;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
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
    private AudioSource audioSource;
    public AudioClip moveAirplaneSound;
    private bool isPlayed = false;
    private bool isPlayedSvaling = false;
    public AudioClip scaleAirplaneSound;
    public AudioClip hookUp;
    public AudioClip hookDown;

    private bool isHookDownSoundPlayed = false;
    private bool isHookUpSoundPlayed = false;


    private LineRenderer lineRenderer;
    public GameObject hookPrefab;
    public Transform startPosition;
    private bool isHookMoving = false;
    private Transform hookTarget;
    private float hookSpeed = 9f;
    private GameObject currentHook;
    public GameObject hookLayer;
    public GameObject lineLayer;
    public Transform endPosition;
    private bool isMovingToTarget = true;
    private bool isReturning = false;
    private List<Transform> deadZombies = new List<Transform>();

    [SerializeField] private List<Transform> slots = new List<Transform>();
    private List<Transform> occupiedSlots = new List<Transform>();
    public Transform bone;
    private enum AirplaneState
    {
        Idle,
        MovingToZombie,
        Unscaling,
        GoHook
    }
    private AirplaneState currentState = AirplaneState.Idle;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        boxCollider = GetComponent<BoxCollider2D>();
        audioSource = gameObject.GetComponent<AudioSource>();
        originalScale = airplanePrefab.transform.localScale;
        targetScale = originalScale * 0.5f;
        targetPositionOffset = airplanePrefab.transform.position + new Vector3(0, -2f, 0);
        animator = GetComponent<Animator>();
        lineRenderer = airplanePrefab.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.08f;
        lineRenderer.endWidth = 0.08f;
        lineRenderer.startColor = Color.magenta;
        lineRenderer.endColor = Color.magenta;
        AttachedPlayerToAirplain();
    }

    void Update()
    {
        animator.SetBool("isMoving", isMoving);
        switch (currentState)
        {
            case AirplaneState.MovingToZombie:
                MoveAirplaneToDeadZombie();
                break;
            case AirplaneState.Unscaling:
                UnScaleAirplain();
                break;
            case AirplaneState.GoHook:
                if (isHookMoving)
                {

                    MoveHook();
                }
                break;
        }
        if (isPlayerAttached)
        {
            if (Mathf.Abs(airplanePrefab.transform.position.x - targetX) > stopThreshold)
            {
                MoveAirplane();
                PlayMoveSound();
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
    private void UnScaleAirplain()
    {

        if (isScaling)
        {
            airplanePrefab.transform.localScale = Vector3.Lerp(airplanePrefab.transform.localScale, originalScale, Time.deltaTime * scaleSpeed);

            Vector3 directionToTarget = (targetPositionOffset - airplanePrefab.transform.position).normalized;
            rb.velocity = new Vector2(0, directionToTarget.y * scaleSpeed);
            if (Vector3.Distance(airplanePrefab.transform.localScale, originalScale) < 0.9f)
            {

                rb.velocity = Vector2.zero;
                isScaling = false;
                currentState = AirplaneState.GoHook;
                isHookMoving = true;
                isMovingToTarget = true;
                animator.SetBool("loweringHook", true);
                InstantiateHook();

            }
        }

    }
    private void InstantiateHook()
    {
        if (currentHook == null)
        {
            currentHook = Instantiate(hookPrefab, startPosition.position, Quaternion.identity);
            hookLayer.SetActive(false);
            lineLayer.SetActive(false);
        }
    }
    private void DrawLine()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPositions(new Vector3[] { startPosition.position, currentHook.transform.position });
        }


    }
    public void GetPositionDeadZombie(Transform zombiePosition)
    {
        AddDeadZombieToList(zombiePosition);

        if (deadZombies.Count > 0)
        {
            hookTarget = deadZombies[0];
            currentState = AirplaneState.MovingToZombie;
            StopFollowingPlayer();
        }

    }

    public void AddDeadZombieToList(Transform zombie)
    {
        deadZombies.Add(zombie);
    }
    private void RemoveZombieFromList(Transform zombie)
    {
        if (deadZombies.Contains(zombie))
        {
            deadZombies.Remove(zombie);
        }
    }

    private void PlaceZombieInAirplane(Transform zombie)
    {
        Transform freeSlot = slots.Find(slot => !occupiedSlots.Contains(slot));

        if (freeSlot != null)
        {
            zombie.SetParent(freeSlot);
            zombie.localPosition = Vector3.zero;
            occupiedSlots.Add(freeSlot);
            Rigidbody2D rb = zombie.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
            }
            StartCoroutine(ScaleZombieSmoothly(zombie, zombie.localScale * 0.9f, 0.5f));
        }
    }


    private void MoveHook()
    {
        if (currentHook == null) return;

        DrawLine();
        if (hookTarget != null)
        {
            if (isMovingToTarget)
            {
                currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, hookTarget.position, hookSpeed * Time.deltaTime);

                if (!isHookDownSoundPlayed)
                {
                    audioSource.PlayOneShot(hookDown);
                    isHookDownSoundPlayed = true;
                }

                if (Vector3.Distance(currentHook.transform.position, hookTarget.position) < 0.1f)
                {
                    isMovingToTarget = false;
                    isReturning = true;

                    if (hookTarget.CompareTag("Zombie"))
                    {
                        hookTarget.SetParent(currentHook.transform);
                        //hookTarget.GetComponent<Rigidbody2D>().isKinematic = true;
                        Destroy(hookTarget.GetComponent<Rigidbody2D>());
                    }
                    else
                    {
                        ReturnState();
                    }
                }
            }
            else if (isReturning)
            {
                currentHook.transform.position = Vector3.MoveTowards(currentHook.transform.position, startPosition.position, hookSpeed * Time.deltaTime);

                if (!isHookUpSoundPlayed)
                {
                    audioSource.PlayOneShot(hookUp);
                    isHookUpSoundPlayed = true;
                }

                if (Vector3.Distance(currentHook.transform.position, startPosition.position) < 0.1f)
                {
                    isReturning = false;
                    lineRenderer.positionCount = 0;
                    hookLayer.SetActive(true);
                    lineLayer.SetActive(true);
                    currentState = AirplaneState.Idle;
                    PlaceZombieInAirplane(hookTarget);
                    CapsuleCollider2D zombieCollider = hookTarget.GetComponent<CapsuleCollider2D>();
                    zombieCollider.enabled = false;
                    RemoveZombieFromList(hookTarget);

                    isHookDownSoundPlayed = false;
                    isHookUpSoundPlayed = false;

                    if (deadZombies.Count > 0)
                    {
                        hookTarget = deadZombies[0];
                        Destroy(currentHook);
                        ScaleAirplain();
                        animator.SetBool("loweringHook", false);
                        currentState = AirplaneState.MovingToZombie;
                    }
                    else
                    {
                        isScaling = true;
                        ScaleAirplain();
                        Destroy(currentHook);
                        ScaleAirplain();
                        animator.SetBool("loweringHook", false);
                    }
                }
            }
        }
        else
        {
            ReturnState();
        }
    }

    private void ReturnState()
    {
        currentState = AirplaneState.Idle;
        isScaling = true;
        ScaleAirplain();
        hookLayer.SetActive(true);
        lineLayer.SetActive(true);
        isReturning = false;
        lineRenderer.positionCount = 0;
        Destroy(currentHook);
    }
    private void PlayMoveSound()
    {
        if (!audioSource.isPlaying && !isPlayed)
        {
            audioSource.PlayOneShot(moveAirplaneSound);
            isPlayed = true;
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
    private void MoveAirplaneToDeadZombie()
    {
        if (hookTarget == null)
        {
            ReturnState();
        }
        if (Vector3.Distance(new Vector3(airplanePrefab.transform.position.x, 0, 0), new Vector3(hookTarget.position.x + 1.1f, 0, 0)) > stopThreshold)
        {
            airplanePrefab.transform.position = new Vector3(Mathf.MoveTowards(airplanePrefab.transform.position.x, hookTarget.position.x + 1.1f, moveSpeed * Time.deltaTime),
                airplanePrefab.transform.position.y, airplanePrefab.transform.position.z);
        }
        else
        {

            currentState = AirplaneState.Unscaling;
            isScaling = true;
        }
    }
    private void StopFollowingPlayer()
    {
        isPlayerAttached = false;
        isMoving = false;
    }
    private IEnumerator ScaleZombieSmoothly(Transform zombie, Vector3 targetScale, float duration)
    {
        Vector3 initialScale = zombie.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            zombie.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / duration);
            yield return null;
        }
        zombie.localScale = targetScale;
    }

    private void ScaleAirplain()
    {

        if (isScaling)
        {

            airplanePrefab.transform.localScale = Vector3.Lerp(airplanePrefab.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

            Vector3 directionToTarget = (targetPositionOffset - airplanePrefab.transform.position).normalized;

            rb.velocity = new Vector2(0, directionToTarget.y * scaleSpeed);
            if (!audioSource.isPlaying && !isPlayedSvaling)
            {
                audioSource.PlayOneShot(scaleAirplaneSound);
                isPlayedSvaling = true;
            }
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
        if (currentState != AirplaneState.Idle) return;
        Vector3 previousPosition = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.up * 100), Vector2.down, Mathf.Infinity, groundLayer);
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, player.transform.position.x, upSpeed * Time.deltaTime),
            Mathf.Lerp(transform.position.y, hit.point.y + flightHeigth, upSpeed * Time.deltaTime), transform.position.z);
        isMoving = Vector3.Distance(transform.position, previousPosition) > 0.001f;
    }
    private void AttachedPlayerToAirplain()
    {
        player.transform.SetParent(bone);
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
