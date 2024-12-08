using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class TileGeneration : MonoBehaviour
{
    [SerializeField] public GameObject random;
    [SerializeField] private GameObject zombiePref;
    [SerializeField] private int maxObjectCount = 10;
    [SerializeField] private float radiusSpawnZombie = 1.5f;
    float radiusForSearchSpawnPlace = 1f;
    [SerializeField] Vector2[] vectors;
    int objectCount = 0;

    [SerializeField] private Player player;

    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    private List<GameObject> spawnObject = new List<GameObject>();
    private HashSet<GameObject> usedSpawnObjects = new HashSet<GameObject>(); 
    Transform[] pointToRun;
    public int countZombies;

    [SerializeField] private GameObject brain;
    public Text coinsCount;

    public List<GameObject> spawnedBrains = new List<GameObject>();
    private AudioSource audioSource;
    public AudioClip spawnZombieSound;
    public AudioClip backgroundSoundMusic;

    public GameObject exitGamePanel;
    public Button goMenuButton;
    public Button exitGameButton;
    public Button continueButton;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        pointToRun = new Transform[2] { leftPoint, rightPoint };
        player = FindObjectOfType<Player>();
        audioSource.volume = 0.5f;
        audioSource.Play();
        Generate();
        CountKilledZombies.Instance.UpdateKillCount();
        UpdateCoinsCountUI();
    }

    void Update()
    {
        BrainsHendler();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 0f;
            exitGamePanel.SetActive(true);
            goMenuButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Map");
                exitGamePanel.SetActive(false);
             
            });
            exitGameButton.onClick.AddListener(() => { Application.Quit(); });
            continueButton.onClick.AddListener(() => {
                exitGamePanel.SetActive(false);
                Time.timeScale = 1f;
            });
    
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

    private void CheckAndSpawnZombie(Transform brainTransform)
    {
        if (brainTransform == null) return;

        usedSpawnObjects.RemoveWhere(spawn => spawn == null);

        List<GameObject> toRemove = new List<GameObject>();

        for (int i = 0; i < spawnObject.Count; i++)
        {
            GameObject spawn = spawnObject[i];
            if (spawn == null || usedSpawnObjects.Contains(spawn)) continue; 

            float distanceToBrain = Vector2.Distance(spawn.transform.position, brainTransform.position);

            if (distanceToBrain <= radiusSpawnZombie && CheckPositionPlayer(spawn.transform.position))
            {

                bool zombieAlreadySpawnedNearby = usedSpawnObjects.Any(usedSpawn =>
                    usedSpawn != null && Vector2.Distance(usedSpawn.transform.position, spawn.transform.position) < radiusSpawnZombie);

                if (!zombieAlreadySpawnedNearby)
                {
                    Collider2D spawnPlaceCollider = Physics2D.OverlapCircle(spawn.transform.position, radiusForSearchSpawnPlace, LayerMask.GetMask("spawnPlace"));
                    if (spawnPlaceCollider != null && spawnPlaceCollider.CompareTag("spawnPlace"))
                    {
                        usedSpawnObjects.Add(spawn); 

                        GameObject zombieObj = Instantiate(zombiePref, spawn.transform.position, Quaternion.identity);
                        audioSource.PlayOneShot(spawnZombieSound);
                        Zombie zombie = zombieObj.GetComponent<Zombie>();
                        zombie.brainTransform = brainTransform;
                        zombie.playerTransform = player.transform;
                        zombie.leftTarget = leftPoint;
                        zombie.rightTarget = rightPoint;

                        toRemove.Add(spawn); 
                    }
                }
            }
        }

        foreach (GameObject obj in toRemove)
        {
            if (spawnObject.Contains(obj))
            {
                spawnObject.Remove(obj);
                Destroy(obj);
            }
        }
    }

    private bool CheckPositionPlayer(Vector2 brainPosition)
    {
        return Vector2.Distance(player.transform.position, brainPosition) > 8f;
    }
}
