using System.Threading.Tasks;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region variables
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] float cameraSpeed = 2f;
    [SerializeField] private LayerMask groundLayer;
    private Animator animator;

    private Rigidbody2D rb;
    private bool isGrounded;


    [SerializeField] private float groundCheckDistance = 1.2f;
    private float maxJumpForce = 12f;
    private float minPositionCamera = -15;
    private float maxPositionCamera = 61;
    private float minPositionPlayer = -24;
    private float maxPositionPlayer = 70;
    private float minPositionPlayerY = -4f;
    private float borderX;

    [SerializeField] private Transform dropPosition;
    private Inventory inventory;
    public GameObject gunObject;
    [SerializeField] Gun currentGun;

    [SerializeField] private TileGeneration tileGeneration;

    public GameObject airplane;
    private bool isOnAirplane = true;


    #endregion

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        inventory = gameObject.GetComponent<Inventory>();
        transform.position = airplane.transform.position;
    }

    void Update()
    {
        if (!isOnAirplane)
        {
            FollowAirplane();
        }

        MovePlayerAndCamera();
        CheckPositionPlayer();
        DropObject();
        ChooseWeapon();

    }
    private void FollowAirplane()
    {
        transform.position = airplane.transform.position+new Vector3 (0,1,0);
    }
    private void DropObject()
    {
        if (inventory.listOfObjects.Count > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            GameObject objectToDrop = inventory.listOfObjects[inventory.listOfObjects.Count - 1];
            inventory.listOfObjects.Remove(objectToDrop);
            GameObject dropObject = Instantiate(objectToDrop, dropPosition.position, Quaternion.identity);
            dropObject.SetActive(true);
            Destroy(dropObject, 8f);
            tileGeneration.spawnedBrains.Add(dropObject);
            
             
            
            CheckCountBrains(objectToDrop);
        }

    }
    async private void CheckCountBrains(GameObject obj)
    {
        while (inventory.listOfObjects.Count != 3)
        {
            await Task.Delay(3000);
            inventory.listOfObjects.Add(obj);

        }
    }

    private void MovePlayerAndCamera()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        float dir = Input.GetAxisRaw("Horizontal");
        animator.SetBool("isMoving", Mathf.Abs(dir) > 0);
      

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (dir != 0)
        {

            Vector3 scale = transform.localScale;
            scale.x = dir > 0 ? 1f : -1f;
            transform.localScale = scale;
            if (currentGun != null)
            {
                currentGun.transform.localScale = new Vector3(scale.x, scale.x, scale.z);
            }
        }

        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);


        borderX = Mathf.Lerp(cameraTransform.position.x, transform.position.x, cameraSpeed * Time.deltaTime);
        borderX = Mathf.Clamp(borderX, minPositionCamera, maxPositionCamera);
        cameraTransform.position = new Vector3(
            borderX,
            Mathf.Lerp(cameraTransform.position.y, transform.position.y, cameraSpeed * Time.deltaTime),
            cameraTransform.position.z
        );
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
    private void CheckPositionPlayer()
    {
        if (transform.position.x < minPositionPlayer)
        {

            rb.position = new Vector2(minPositionPlayer + 1f, transform.position.y);


        }
        else if (transform.position.x > maxPositionPlayer)
        {
            rb.position = new Vector2(maxPositionPlayer - 1f, transform.position.y);
        }
        if (transform.position.y < minPositionPlayerY)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * maxJumpForce, ForceMode2D.Impulse);
        }

    }

    private void ChooseWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gunObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        { 
            gunObject.SetActive(true); 
        }
    }
}
