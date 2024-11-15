using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class TileGeneration : MonoBehaviour
{


    [SerializeField] public GameObject random;
    [SerializeField] private GameObject zombiePref;
    [SerializeField] private int maxObjectCount = 10;
    [SerializeField] private float radiusSpawnZombie = 5f;
    float radiusForSearchSpawnPlace = 5f;
    [SerializeField] Vector2[] vectors;
    int objectCount = 0;

    [SerializeField] private Transform player;
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
 private List<GameObject> spawnObject = new List<GameObject>();

    [SerializeField] private GameObject brain;
   

    public List<GameObject> spawnedBrains = new List<GameObject>();
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        Generate();
    }
    void Update()
    {
        
        BrainsHendler();
    }
    void BrainsHendler()
    {
        for (int i = 0;i<spawnedBrains.Count;i++)
        {
            if (spawnedBrains[i] == null)
            {
                spawnedBrains.RemoveAt(i);
            }
            else
            {

                CheckAndSpawnZombie(spawnedBrains[i].transform.position);
            }
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
        foreach (Vector2 spawn in vectors)
        {
            float distanceToBrain = Vector2.Distance(spawn, brainPosition);
            
                if (distanceToBrain <= radiusSpawnZombie && CheckPositionPlayer(brainPosition))
                {
                    Collider2D spawnPlaceCollider = Physics2D.OverlapCircle(spawn, radiusForSearchSpawnPlace, LayerMask.GetMask("spawnPlace"));
                    if (spawnPlaceCollider != null && spawnPlaceCollider.CompareTag("spawnPlace"))
                    {
                   
                        
                      GameObject zombieObj =  Instantiate(zombiePref, spawn, Quaternion.identity);
                    Zombie zombie = zombieObj.GetComponent<Zombie>();

                    zombie.target = CheckClosesPoint(zombie);
                    RemoveObject();
                      
                        
                    }
                    
               }
            
        }
    }
    private Transform CheckClosesPoint(Zombie zombie)
    {
        float distanceToLeft = Vector2.Distance(zombie.transform.position, leftPoint.position);
        float distanceToRight= Vector2.Distance(zombie.transform.position, rightPoint.position);
        if (distanceToLeft <= distanceToRight) { return leftPoint; }
        else { return rightPoint; }
       
    }
   private void RemoveObject()
    {
        for (int i = 0;i<spawnObject.Count;i++)
        {
            GameObject spawn = spawnObject[i];
            Collider2D brainCollider = Physics2D.OverlapCircle(spawn.transform.position, 2f, LayerMask.GetMask("Brain"));
            if (brainCollider != null && brainCollider.CompareTag("Brain"))
            {
                spawnObject.Remove(spawn);
                Destroy(spawn);
            }
        }
        
    }

    private bool CheckPositionPlayer(Vector2 brainPosition)
    {
        return Vector2.Distance(player.position, brainPosition) > 10;

    }

}
