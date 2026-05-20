using UnityEngine;

public class OOPPhysicsSquareSpawner : MonoBehaviour
{
    public Rigidbody2D squarePrefab;
    public float spawnRate = 1f;
    private float nextSpawnTime;
    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            Instantiate(squarePrefab, transform.position, Quaternion.identity);
            nextSpawnTime = Time.time + 1f / spawnRate;
        }
    }
}
