using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform firePosition;
    [SerializeField] private Transform aimTransform;
    private float speedBullet = 8f;

    private float shootTime = 3f;
    private float nextShoot = 0f;
    AimController aimController;
    Player player;
    public GameObject arrow;
    public GameObject garpun;
    private float speedArrow = 8f;
    private LineRenderer lineRenderer;
    public Transform startPosition;

    public bool isFire;
    public GameObject arrowPref;
    public GameObject spawnPosition;

    void Start()
    {
        aimController = FindObjectOfType<AimController>();
        aimTransform = aimController.transform;
        player = GetComponent<Player>();
        lineRenderer = garpun.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.03f;
        lineRenderer.endWidth = 0.03f;
        lineRenderer.startColor = Color.magenta;
        lineRenderer.endColor = Color.magenta;

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextShoot && player.isGun)
        {
            ShootGun();
            nextShoot = Time.time + shootTime;
        }
        else if (Input.GetMouseButtonDown(0) && player.isGarpun)
        {

            ShootGarpun();
        }
        if (isFire)
        {
            DrawLine();
        }
        else
        {

        }

    }
   
    private void ShootGun()
    {

        GameObject bullet = Instantiate(bulletPref, firePosition.position, Quaternion.identity);
        Vector3 direction = (aimTransform.position - firePosition.position).normalized;

        Rigidbody2D rb2D = bullet.GetComponent<Rigidbody2D>();

        rb2D.velocity = direction * speedBullet;
    }

    private void ShootGarpun()
    {
        isFire = true;
        arrow.transform.SetParent(null);
        Vector3 direction = (aimTransform.position - firePosition.position).normalized;
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        rb.velocity = direction * speedArrow;
    }
    private void DrawLine()
    {
        if (arrow != null)
        {
            lineRenderer.SetPositions(new Vector3[] { startPosition.position, arrow.transform.position });
        }
        else
        {
            isFire = false;
            lineRenderer.SetPositions(new Vector3[] { Vector3.zero, Vector3.zero });
            if (arrow == null)
            {
                Vector3 direction = (aimTransform.position - garpun.transform.position).normalized;
                arrow = Instantiate(arrowPref, spawnPosition.transform.position, Quaternion.identity);
                arrow.transform.SetParent(garpun.transform);
                arrow.transform.right = direction;

            }
        }
    }
}
 