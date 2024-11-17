using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    public GameObject airplanePrefab;
    private float moveSpeed = 5f;
    private float moveOnY = 0.5f;
    private Rigidbody2D rb;
   Player player;
    private bool isPlayerAttached = true;
    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();
        player.transform.SetParent(airplanePrefab.transform);
        player.rb.isKinematic = true;
    }

    void Update()
    {
        MoveAirplane();
        if (isPlayerAttached && Input.GetKeyDown(KeyCode.Space))
        {
            UnattachedPlayer();
        }
    }
    private void MoveAirplane()
    {
            Vector3  moveDirection = new Vector3 (moveSpeed, moveOnY, 0);
        rb.velocity = moveDirection;     
    }
    private void UnattachedPlayer ()
    {
        player.transform.SetParent(null);
        player.rb.isKinematic = false;
        isPlayerAttached = false;
    } 
}
