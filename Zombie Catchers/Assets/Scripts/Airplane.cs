using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Airplane : MonoBehaviour
{
    public GameObject airPlanePrefab;
    private float moveSpeed = 5f;
    private float moveOnY = 0.5f;
    private Rigidbody2D rb;
   
    void Start()
    {
      rb = GetComponent<Rigidbody2D>();
       
    }

    void Update()
    {
        MoveAirplane();
    }
    private void MoveAirplane()
    {
            Vector3  moveDirection = new Vector3 (moveSpeed, moveOnY, 0);
        rb.velocity = moveDirection;

    }
 
}
