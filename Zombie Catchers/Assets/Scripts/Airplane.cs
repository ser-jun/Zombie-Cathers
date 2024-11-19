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


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        fixedY = airplanePrefab.transform.position.y;
        originalScale = airplanePrefab.transform.localScale;
        targetScale = originalScale * 0.5f;
        targetPositionOffset = airplanePrefab.transform.position + new Vector3(0, -2f, 0);
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
    }
    private void MoveAirplane()
    {
        Vector3 moveDirection = new Vector3(moveSpeed, moveOnY, 0);
        rb.velocity = moveDirection;


    }
    private void DecreasePlain()
    {
        if (isScaling)
        {
            // Уменьшение масштаба
            airplanePrefab.transform.localScale = Vector3.Lerp(airplanePrefab.transform.localScale, targetScale, Time.deltaTime * scaleSpeed);

            // Вычисление направления вниз
            Vector3 directionToTarget = (targetPositionOffset - airplanePrefab.transform.position).normalized;

            // Установка скорости на Rigidbody2D
            rb.velocity = new Vector2(0, directionToTarget.y * scaleSpeed);

            // Проверка завершения уменьшения масштаба и положения
            if (Vector3.Distance(airplanePrefab.transform.localScale, targetScale) < 0.01f &&
                Vector3.Distance(airplanePrefab.transform.position, targetPositionOffset) < 0.01f)
            {
                airplanePrefab.transform.localScale = targetScale;
                airplanePrefab.transform.position = targetPositionOffset;
                rb.velocity = Vector2.zero; // Остановка движения
                isScaling = false;
            }
        }
    }

    private void FollowThePlayer ()
    {
        Vector3 targetPosition = new Vector3(player.transform.position.x, fixedY, 0);
        Vector3 directionToTarget = (targetPosition - airplanePrefab.transform.position).normalized;
        rb.velocity = new Vector3(directionToTarget.x * moveSpeed, 0, 0);
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
