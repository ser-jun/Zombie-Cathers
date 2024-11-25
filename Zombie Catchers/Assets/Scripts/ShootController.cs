using TMPro;
using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform firePosition;
    [SerializeField] private Transform aimTransform;
    private float speedBullet = 10f;

    private float shootTime = 3f;
    private float nextShoot = 0f;
    AimController aimController;
    Player player;

    void Start()
    {
        aimController = FindObjectOfType<AimController>();
        aimTransform= aimController.transform;
        player = GetComponent<Player>();
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= nextShoot && player.isWeaponhand)
        {
            Shoot();
            nextShoot = Time.time+shootTime;
        }
    }
   private void Shoot()
    {

        GameObject bullet = Instantiate(bulletPref, firePosition.position, Quaternion.identity);
        Vector3 direction = (aimTransform.position - firePosition.position).normalized;

        Rigidbody2D rb2D = bullet.GetComponent<Rigidbody2D>();

        rb2D.velocity = direction * speedBullet;
     
       
    }
}
