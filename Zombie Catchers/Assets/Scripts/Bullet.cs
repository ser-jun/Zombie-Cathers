using UnityEngine;

public class Bullet : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Target")  || collision.CompareTag("Ground"))
        {
            Destroy(gameObject); 
        }
    }
}
