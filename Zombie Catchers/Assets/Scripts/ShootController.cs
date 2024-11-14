using TMPro;
using UnityEngine;

public class Shot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPref;
    [SerializeField] private Transform firePosition;
    [SerializeField] private Transform aimTransform;
    private float speedBullet = 20f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
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
