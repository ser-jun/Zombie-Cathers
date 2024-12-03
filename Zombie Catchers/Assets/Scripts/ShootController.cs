using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform firePosition;
    [SerializeField] private Transform aimTransform;
    private float speedBullet;

    private float shootTime;
    private float nextShoot = 0f;
    AimController aimController;
    Player player;
    public GameObject arrow;
    public GameObject garpun;
    private float speedArrow;
    private LineRenderer lineRenderer;
    public Transform startPosition;

    public bool isFire;
    public GameObject arrowPref;
    public GameObject spawnPosition;
    private AudioSource audioSource;

    public AudioClip shotGarpunSound;
    public AudioClip shotGunSound;

    public ParticleSystem particle;

    void Start()
    {
        UpgradeByLevel();
        
        audioSource = GetComponent<AudioSource>();
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
        if (Time.time <= nextShoot)
        {
            particle.Stop();
        }
        else
        {
            particle.Play();
        }
        if (Input.GetMouseButtonDown(0) && Time.time >= nextShoot && player.isGun)
        {

            ShootGun();
            nextShoot = Time.time + shootTime;
            
            audioSource.PlayOneShot(shotGunSound);
        }
        else if (Input.GetMouseButtonDown(0) && player.isGarpun)
        {

            ShootGarpun();
            audioSource.PlayOneShot(shotGarpunSound);
        }
        if (isFire)
        {
            DrawLine();
        }
        else
        {

        }

    }
    private void UpgradeByLevel()
    {
        GameData data = SaveManager.Instance.LoadData();
        switch (data.upgradeWeapons[0])
        {
            case 1:
                speedBullet = 6f;
                shootTime = 4f;
                break;
            case 2:
                speedBullet = 8f;
                shootTime = 3f;
                break;
            case 3:
                speedBullet = 10f;
                shootTime = 2f;
                break;
        }
        switch (data.upgradeWeapons[1])
        {
            case 1:
                speedArrow = 8f;
                break;
            case 2:
                speedArrow = 10f;
                break;
            case 3:
                speedArrow = 12f;
                break;
        }

    }

    private void ShootGun()
    {
        isFire = true;
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
