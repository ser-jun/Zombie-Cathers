using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileGeneration : MonoBehaviour
{


    [SerializeField] public GameObject random;
    [SerializeField] private GameObject zombie;
    [SerializeField] private int maxObjectCount = 10;
    [SerializeField] private float radiusSpawnZombie = 5f;
    float radiusForSearchSpawnPlace = 5f;
    [SerializeField] Vector2[] vectors;
    int objectCount = 0;

    [SerializeField] private Transform player;
 private List<GameObject> spawnObject = new List<GameObject>();

    [SerializeField] private GameObject brain;
    private bool isPlayerAway = false;
    private bool zombieSpawned = false;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Generate();
    }
    void Update()
    {
        CheckPositionPlayer(brain);

        // Если игрок отошел и зомби ещё не был заспавнен, спауним зомби
        if (isPlayerAway && !zombieSpawned)
        {
            CheckAndSpawnZombie(brain.transform.position);
        }
    }

    void Generate()
    {
        objectCount = Random.Range(3, maxObjectCount);

        while (objectCount < maxObjectCount)
        {
            Vector2 randomPosition = vectors[Random.Range(0, vectors.Length)];
           GameObject spawn= Instantiate(random, randomPosition, Quaternion.identity);
           spawnObject.Add(spawn);
            objectCount++;
        }
    }
   public void CheckAndSpawnZombie(Vector3 brainPosition)
    {
        bool zombieSpawned = false;
        foreach (Vector2 spawn in vectors)
        {
            float distanceToBrain = Vector2.Distance(spawn, brainPosition);
            
                if (distanceToBrain <= radiusSpawnZombie)
                {
                    Collider2D spawnPlaceCollider = Physics2D.OverlapCircle(spawn, radiusForSearchSpawnPlace, LayerMask.GetMask("spawnPlace"));
                    if (spawnPlaceCollider != null && spawnPlaceCollider.CompareTag("spawnPlace"))
                    {
                   
             
                        if (!zombieSpawned)
                        {
                            Instantiate(zombie, spawn, Quaternion.identity);
                            RemoveObject();
                            zombieSpawned = true;
                        }
                    }
                    
               }
            
        }
    }
   private void RemoveObject()
    {
        foreach (var spawn in spawnObject)
        {
            Collider2D brainCollider = Physics2D.OverlapCircle(spawn.transform.position, 2f, LayerMask.GetMask("Brain"));
            if (brainCollider != null && brainCollider.CompareTag("Brain"))
            {
                Destroy(spawn);
            }
        }
    }

    private void CheckPositionPlayer(GameObject brainPosition)
    {
        float distance = Vector2.Distance(player.position, brainPosition.transform.position);
        if (distance > 10f)
        {
        isPlayerAway = true;
        }
       
    }

}
