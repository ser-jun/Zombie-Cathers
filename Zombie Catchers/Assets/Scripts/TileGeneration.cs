using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileGeneration : MonoBehaviour
{


    [SerializeField] public GameObject random;
    [SerializeField] private GameObject zombiePref;
    [SerializeField] private int maxObjectCount = 10;
    [SerializeField] private float radiusSpawnZombie = 2f;
    float radiusForSearchSpawnPlace = 2f;
    [SerializeField] Vector2[] vectors;
    int objectCount = 0;

    [SerializeField] private Player player;
    
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    private List<GameObject> spawnObject = new List<GameObject>();
    Transform[] pointToRun;
    public int countZombies;

    [SerializeField] private GameObject brain;
    public Text coinsCount;

    public List<GameObject> spawnedBrains = new List<GameObject>();
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        pointToRun = new Transform[2] { leftPoint, rightPoint };
        player = FindObjectOfType<Player>();
        Generate();
        CountKilledZombies.Instance.UpdateKillCount();
        UpdateCoinsCountUI();
    }
    void Update()
    {

        BrainsHendler();
    }
    void BrainsHendler()
    {
        for (int i = 0; i < spawnedBrains.Count; i++)
        {
            if (spawnedBrains[i] == null)
            {
                spawnedBrains.RemoveAt(i);
            }
            else
            {

                CheckAndSpawnZombie(spawnedBrains[i].transform);
            }
        }
    }
    private void UpdateCoinsCountUI()
    {
        GameData data = SaveManager.Instance.LoadData();
        coinsCount.text = data.coins.ToString();
    }
    void Generate()
    {
        objectCount = Random.Range(4, maxObjectCount);
        
        while (objectCount > 0)
        {
            Vector2 randomPosition = vectors[Random.Range(0, vectors.Length)];
            GameObject spawn = Instantiate(random, randomPosition, Quaternion.identity);
            spawnObject.Add(spawn);
            objectCount--;
            countZombies++;
        }
    }
    private void CheckAndSpawnZombie(Transform brainTransform)
    {
        foreach (Vector2 spawn in vectors)
        {
            float distanceToBrain = Vector2.Distance(spawn, brainTransform.position);

            if (distanceToBrain <= radiusSpawnZombie && CheckPositionPlayer(spawn))
            {
                Collider2D spawnPlaceCollider = Physics2D.OverlapCircle(spawn, radiusForSearchSpawnPlace, LayerMask.GetMask("spawnPlace"));
                if (spawnPlaceCollider != null && spawnPlaceCollider.CompareTag("spawnPlace"))
                {

                    GameObject zombieObj = Instantiate(zombiePref, spawn, Quaternion.identity);
                    Zombie zombie = zombieObj.GetComponent<Zombie>();
                    zombie.brainTransform = brainTransform;
                    zombie.playerTransform = player.transform;
                    zombie.leftTarget = leftPoint;
                    zombie.rightTarget = rightPoint;
                    RemoveObject();


                }

            }

        }
    }
   
    private void RemoveObject()
    {
        for (int i = 0; i < spawnObject.Count; i++)
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
        return Vector2.Distance(player.transform.position, brainPosition) > 8f;

    }

}
