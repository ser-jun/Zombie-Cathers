using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Searcher : MonoBehaviour
{
    private float minYPosition = 100f;
    private float maxYPosition = 263f;
    private float minXPosition = -15f;
    private float maxXPosition = 640f;

    private float moveSpeed = 15f;
    private Vector3 targetPosition;
    void Start()
    {
        GenerateNewPosition();
    }
    void Update()
    {
        Move();
    }
    private float RandomGenerationXPosition ()
    {
        return Random.Range(minXPosition, maxXPosition);
    }
    private float RandomGenerationYPosition ()
    {
        return Random.Range(minYPosition, maxYPosition);
    }

   private void GenerateNewPosition()
    {
        targetPosition= new Vector3(RandomGenerationXPosition(), RandomGenerationYPosition(), transform.position.z);
    }
    private void Move()
    {

        float distance = Vector3.Distance(transform.position, targetPosition);

        if (distance < 1f)
        {
            GenerateNewPosition();
        }
        else
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }
}
