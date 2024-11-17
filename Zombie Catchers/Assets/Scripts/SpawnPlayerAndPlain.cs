using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlayerAndPlain : MonoBehaviour
{
   [SerializeField] public GameObject playerPrefab;
   [SerializeField] public GameObject airplanePrefab;
    public AimController aimController;

    private Vector2 spawnPositionPlayer = new Vector2(-22.3f, 4.7f);
    private Vector2 positionForSpawnAirplain = new Vector2(-23.9f, 4.7f);
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
        GameObject playerInstance = Instantiate(playerPrefab, spawnPositionPlayer, Quaternion.identity);

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
        airplanePrefab = Instantiate(airplanePrefab, positionForSpawnAirplain, Quaternion.identity);
    }
}
