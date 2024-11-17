using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerAndPlain : MonoBehaviour
{
   [SerializeField] public GameObject playerPrefab;
   [SerializeField] public GameObject airplanePrefab;
    public AimController aimController;

    private Vector2 spawnPosition = new Vector2(-26.54f, 4.2f);
    private Vector2 positionForSpawn = new Vector2(-28f, 4.7f);
    public bool isSpawned = false;

    void Start()
    {
            
        SpawnAirplane();
        SpawnPlayer();
    }

 
    void Update()
    {
        
    }
    private void SpawnPlayer()
    {
        GameObject playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);

        Player playerScript = playerInstance.GetComponent<Player>();
        if (playerScript != null && aimController != null)
        {
            playerScript.SetAimController(aimController);
            aimController.playerTransform = playerInstance.transform; 
        }

        playerInstance.transform.SetParent(airplanePrefab.transform);

        isSpawned = true;
    }
    private void SpawnAirplane()
    {
        airplanePrefab = Instantiate(airplanePrefab, positionForSpawn, Quaternion.identity);
    }
}
