using System.Collections.Generic;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{


    [SerializeField] private GameObject random;
    [SerializeField] private int maxObjectCount = 5;

    [SerializeField] Vector2[] vectors;
    int objectCount = 0;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Generate();
    }


    void Generate() 
    {

       
      while (objectCount < maxObjectCount)
        {
            Vector2 randomPosition = vectors[Random.Range(0, vectors.Length)];
            Instantiate(random,randomPosition, Quaternion.identity);
            objectCount++;
        }
    }
}
