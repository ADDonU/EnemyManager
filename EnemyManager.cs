using UnityEngine;
using Mirror;

public class EnemyManager : NetworkBehaviour
{
//    public PlayerHealth playerHealth;       // Reference to the player's heatlh.
    public GameObject enemy;                // The enemy prefab to be spawned.
    public float spawnTime = 2f;            // How long between each spawn.
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    public int maxEnemies = 10;

    void Start ()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating ("Spawn", spawnTime, spawnTime);
    }

    [Server]
    void Spawn ()
    {
        // to count the number of objects:
        int enemyCount = GameObject.FindGameObjectsWithTag(enemy.tag).Length;
        if(enemyCount < maxEnemies){
            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            GameObject go = Instantiate (enemy, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
            go.name = enemy.name;
            NetworkServer.Spawn(go);
        }
    }
}
