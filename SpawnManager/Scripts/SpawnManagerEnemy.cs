using UnityEngine;
using Mirror;

public class SpawnManagerEnemy : NetworkBehaviour
{
    [Space(2)]
    [Header("This Enemy Gameobject NEEDS TO BE TAGGED.")]
    [Space(2)]
    [Header("Select the Enemy Prefab To Spawn:")]
    public GameObject enemyPrefab;                // The enemy prefab to be spawned.
    [Space(2)]
    [Header("Enter Total Max Enemies Desired To Spawn:")]
    public int maxEnemies = 100;
    [Space(2)]
    [Header("Enter Time Value between each Spawn Attempt:")]
    public float spawnTime = 2f;            // How long between each spawn.
    [Space(2)]
    [Header("Enter Total Number of spawn points and select tranforms")]
    [Header("Each tranform Needs a Network Proximity Checker for security")]
    [Space(2)]
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.

    void Start ()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating ("SpawnManagerEnemySpawn", spawnTime, spawnTime);
    }

    [Server]
    void SpawnManagerEnemySpawn ()
    {
        // to count the number of objects:
        int enemyCount = GameObject.FindGameObjectsWithTag(enemyPrefab.tag).Length;
        if(enemyCount < maxEnemies){
            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            GameObject go = Instantiate (enemyPrefab, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
            go.name = enemyPrefab.name;
            NetworkServer.Spawn(go);
        }
    }
}
