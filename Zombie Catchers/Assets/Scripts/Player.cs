using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.IK;

public class Player : MonoBehaviour
{
    #region variables
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 7f;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] float cameraSpeed = 2f;
    [SerializeField] private LayerMask groundLayer;
    private Animator animator;

    public Rigidbody2D rb;
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
    [SerializeField] Gun currentGun;

    [SerializeField] private TileGeneration tileGeneration;

    public GameObject airplane;
 
    AimController aimController;
    private float throwForce = 7f;

    public List<GameObject> weapons = new List<GameObject>();
    public bool isGun;
    public bool isGarpun;


    public Transform hand1;
    public Transform hand2;
    private Transform hand1Target; 
    private Transform hand2Target;
    public static Player Instance;
    public GameObject hat;
    #endregion
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        animator = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        inventory = gameObject.GetComponent<Inventory>();
        tileGeneration = FindObjectOfType<TileGeneration>();
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        PutHat();
        ChangeWeapon(0);
    }

    void Update()
    {
       

        MovePlayer();
        CheckPositionPlayer();
        DropObject();
        UpdateHandPositions();
        ChooseWeapon();
    

    }
   
    private void FixedUpdate()
    {
        HandleCamera();
    }
   
    void HandleCamera()
    {
        borderX = Mathf.Lerp(cameraTransform.position.x, transform.position.x, cameraSpeed * Time.fixedDeltaTime);
        borderX = Mathf.Clamp(borderX, minPositionCamera, maxPositionCamera);
        cameraTransform.position = new Vector3(
            borderX,
            Mathf.Lerp(cameraTransform.position.y, transform.position.y, cameraSpeed * Time.fixedDeltaTime),
            cameraTransform.position.z
        );
    }
    public void SetAimController(AimController aim)
    {

        aimController = aim;
    }
   
    private void DropObject()
    {
        if (inventory.listOfObjects.Count > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            GameObject objectToDrop = inventory.listOfObjects[inventory.listOfObjects.Count - 1];
            inventory.listOfObjects.Remove(objectToDrop);
            GameObject dropObject = Instantiate(objectToDrop, dropPosition.position, Quaternion.identity);
            Vector3 direction = (dropPosition.position - aimController.transform.position).normalized;
            Rigidbody2D rb = dropObject.GetComponent<Rigidbody2D>();
            rb.AddForce(-direction*throwForce, ForceMode2D.Impulse);
            tileGeneration.spawnedBrains.Add(dropObject);
            dropObject.SetActive(true);
            Destroy(dropObject, 8f);
            
             
            
            CheckCountBrains(objectToDrop);
        }

    }
    private void PutHat()
    {
        GameData data = SaveManager.Instance.LoadData();
        if (data.caps[0]==1)
        {
            hat.SetActive(true);
        }
        else { hat.SetActive(false); }
    }
    async private void CheckCountBrains(GameObject obj)
    {
        while (inventory.listOfObjects.Count != 3)
        {
            await Task.Delay(3000);
            inventory.listOfObjects.Add(obj);

        }
    }

    private void MovePlayer()
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
            foreach (var weapon in weapons)
            {
                if (weapon != null)
                {
                    Vector3 weaponScale = weapon.transform.localScale;
                    weapon.transform.localScale = new Vector3(
                        Mathf.Abs(weaponScale.x) * scale.x,
                        weaponScale.x,
                        weaponScale.z
                    );
                }
            }
            //if (currentGun != null)
            //{
            //    currentGun.transform.localScale = new Vector3(scale.x, scale.x, scale.z);
            //}
        }

        rb.velocity = new Vector2(dir * moveSpeed, rb.velocity.y);
      
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

    private void ChangeWeapon(int weaponIndex)
    {
       
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(i == weaponIndex);

                if (i == weaponIndex)
                {
                    hand1Target = weapons[i].transform.Find("Hand1Gun");
                    hand2Target = weapons[i].transform.Find("Hand2Gun");
                }
            }
        }
    }
    private void UpdateHandPositions()
    {

        if (hand1Target != null)
        {
            hand1.position = hand1Target.position;
        }

        if (hand2Target != null)
        {
            hand2.position = hand2Target.position;
        }
    }

    private void ChooseWeapon()
    {
        GameData data = SaveManager.Instance.LoadData();
        if (Input.GetKeyDown(KeyCode.Alpha1) && data.upgradeWeapons[0] >=1)
        {
            isGun=true;
            isGarpun=false;
            ChangeWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && data.upgradeWeapons[1] >= 1)
        {
            isGarpun=true;
            isGun=false;
            ChangeWeapon(1);
        }
       
    }

}
