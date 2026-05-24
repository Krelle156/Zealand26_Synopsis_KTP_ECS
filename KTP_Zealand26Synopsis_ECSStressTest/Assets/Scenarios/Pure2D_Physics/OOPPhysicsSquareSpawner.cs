using UnityEngine;
using Unity.Entities;

public class OOPPhysicsSquareSpawner : MonoBehaviour
{
    public static int ReportedNumOfSquares;

    public Rigidbody2D squarePrefab;
    public float spawnRate = 1f;
    private float nextSpawnTime;

    public void Awake()
    {
        ReportedNumOfSquares = 0;
    }

    void Update()
    {
        if (Time.time >= nextSpawnTime)
        {
            Instantiate(squarePrefab, transform.position, Quaternion.identity);
            nextSpawnTime = Time.time + 1f / spawnRate;
            SharedUIController instance = SharedUIController.Instance;
            ReportedNumOfSquares++;
            if (instance != null)
            {
                instance.ReportedNumOfSquares = ReportedNumOfSquares;
            }
        }
    }
}
