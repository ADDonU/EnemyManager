using UnityEngine.AI;
using UnityEngine;
using Mirror;

// requires proximity checker so that we can use .observers to check if any
// players are currently around before spawning

public class SpawnManagerItems : NetworkBehaviour
{
    [Space(2)]
    [Header("This Item Gameobject NEEDS TO BE TAGGED.")]
    [Space(2)]
    [Header("Select the Item Prefabs To Spawn:")]
    public SpawnManagerPrefabAndOdds[] prefabAndOdds;
    [Space(2)]
    [Header("Enter Total Max Items Desired To Spawn:")]
    public int maxItems = 10;
    [Space(2)]
    [Header("Enter Time Value between each Spawn Attempt:")]
    public float spawnTime = 1f;            // How long between each spawn.
    [Space(2)]
    [Header("Enter Minimum Distance Value between items:")]
    public float areaRadius = 1f;            // How long between each spawn.
    [Space(2)]
    [Header("Enter how far from spawn point will it try to spawn:")]
    public float maxSpawnDistanceRadius = 1f;            // How long between each spawn.
    [Space(2)]
    [Header("Enter Max extra Items to spawn within an Area:")]
    public int extraItemsPerArea = 1;            // How long between each spawn.
    [Space(2)]
    [Header("Enter Total Number of spawn points and select tranforms")]
    [Space(2)]
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.


    void Start ()
    {
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating ("SpawnManagerItemSpawn", spawnTime, spawnTime);
        
    }

    [Server]
    void SpawnManagerItemSpawn ()
    {    
        GameObject itemPrefab = null;
        bool findItem = true;
        foreach (SpawnManagerPrefabAndOdds prefabWithOdds in prefabAndOdds)
        {
            if(findItem)
            if (Random.value <= prefabWithOdds.probability)
            {
                itemPrefab = prefabWithOdds.itemPrefab.gameObject;
                findItem = false;
                
            }
        }
        if(findItem != true){
            // to count the number of objects:
            int itemCount = GameObject.FindGameObjectsWithTag(itemPrefab.tag).Length;
            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);
            
            if(itemCount < maxItems){

                Collider[] hitColliders = Physics.OverlapSphere(spawnPoints[spawnPointIndex].position, areaRadius);
                int itemsFound = 0;
                for (var i = 0; i < hitColliders.Length; i++) {
                    if(hitColliders[i].tag == itemPrefab.tag){
                        itemsFound++;
                    }
                }
                if(itemsFound < extraItemsPerArea){
                    Vector3 spawnPosition = GetLocationAttemptToMakeRandom(spawnPoints[spawnPointIndex].position, maxSpawnDistanceRadius);
                    // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
                    GameObject go = Instantiate (itemPrefab, spawnPosition, Quaternion.identity);
                    go.name = itemPrefab.name;
                    NetworkServer.Spawn(go);
                }
            }
        }
    }
    

    // random point on NavMesh for item drops, etc.
    public static Vector3 DrawCircleOnNavMesh(Vector3 position, float distanceRadius)
    {
        // random circle point
        Vector2 randomForLocation = UnityEngine.Random.insideUnitCircle * distanceRadius;

        // convert to 3d
        Vector3 randomNavPosition = new Vector3(position.x + randomForLocation.x, position.y, position.z + randomForLocation.y);

        // raycast to find valid point on NavMesh. otherwise return original one
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomNavPosition, out hit, distanceRadius, NavMesh.AllAreas))
            return hit.position;
        return position;
    }
    
    public static Vector3 GetLocationAttemptToMakeRandom(Vector3 position, float radiusMultiplier)
    {
        for (int i = 0; i < 30; ++i)
        {
            // get random point on navmesh around position
            Vector3 candidate = DrawCircleOnNavMesh(position, radiusMultiplier);

            // check if anything obstructs the way (walls etc.)
            NavMeshHit hit;
            if (!NavMesh.Raycast(position, candidate, out hit, NavMesh.AllAreas))
                return candidate;
        }

        // otherwise return original position if we can't find any good point.
        return position;
    }
}
