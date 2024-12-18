using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private Bullet bulletPref;
    [SerializeField] private Transform firePosition;
    [SerializeField] private Transform aimTransform;
    private float speedBullet;

    private float shootTime;
    private float nextShoot = 0f;
    AimController aimController;
    Player player;
    public Arrow arrow;
    public GameObject garpun;
    private float speedArrow;
    private LineRenderer lineRenderer;
    public Transform startPosition;

    public bool isFire;
    public Arrow arrowPref;
    public GameObject spawnPosition;
    private AudioSource audioSource;

    public AudioClip shotGarpunSound;
    public AudioClip shotGunSound;
    public AudioClip shotGunMachine;

    public ParticleSystem particle;
    public GameObject dartPref;
    public Transform firePositionDart;
    private float speedDart=10f;

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

            ShootGun(bulletPref.transform, speedBullet, firePosition);
            nextShoot = Time.time + shootTime;
            
            audioSource.PlayOneShot(shotGunSound);
        }
        else if (Input.GetMouseButtonDown(0) && player.isGarpun)
        {

            ShootGarpun();
            audioSource.PlayOneShot(shotGarpunSound);
        }
        else if (Input.GetMouseButtonDown(0) && !player.isGarpun && !player.isGun && Time.time >= nextShoot)
        {
            ShootGun(dartPref.transform, speedDart, firePositionDart);
            nextShoot = Time.time + 0.5f;
            audioSource.PlayOneShot(shotGunMachine);
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
                bulletPref.liveTimeBullet = 0.5f;
                break;
            case 2:
                speedBullet = 8f;
                shootTime = 3f;
                bulletPref.liveTimeBullet = 0.6f;
                break;
            case 3:
                speedBullet = 10f;
                shootTime = 2f;
                bulletPref.liveTimeBullet = 0.7f;
                break;
        }
        switch (data.upgradeWeapons[1])
        {
            case 1:
                speedArrow = 8f;
                arrowPref.liveTimeBullet = 0.35f;
                arrowPref.maxCountCollision = 1;
                break;
            case 2:
                speedArrow = 10f;
                arrowPref.liveTimeBullet = 0.4f;
                arrowPref.maxCountCollision = 2;
                break;
            case 3:
                speedArrow = 12f;
                arrowPref.liveTimeBullet = 0.55f;
                arrowPref.maxCountCollision = 2;
                break;
        }

    }

    private void ShootGun(Transform prefab, float speed, Transform firePos)
    {
        isFire = true;
        GameObject bullet = Instantiate(prefab.gameObject, firePos.position, Quaternion.identity);
        Vector3 direction = (aimTransform.position - firePos.position).normalized;

        Rigidbody2D rb2D = bullet.GetComponent<Rigidbody2D>();

        rb2D.velocity = direction * speed;
    }

    private void ShootGarpun()
    {
        if (arrow ==null) { CreateNewArrow(); }
        isFire = true;
        arrow.transform.SetParent(null);    
        arrow.StartShot();
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
                CreateNewArrow();
            }
        }
    }
    private void CreateNewArrow()
    {
        Vector3 direction = (aimTransform.position - garpun.transform.position).normalized;
        arrow = Instantiate(arrowPref.gameObject, spawnPosition.transform.position, Quaternion.identity).GetComponent<Arrow>();
        arrow.transform.SetParent(garpun.transform);
        arrow.transform.right = direction;
    }

}
