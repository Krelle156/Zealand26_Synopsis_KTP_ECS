using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;

public class OOPPhysicsSquareSpawner : MonoBehaviour
{
    public static int ReportedNumOfSquares;

    public Rigidbody2D squarePrefab;
    public float spawnRate = 1f;
    private float cooldown;

    public void Awake()
    {
        ReportedNumOfSquares = 0;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0)
        {
            SpawnSquare();
            SharedUIController instance = SharedUIController.Instance;
            if (instance != null)
            {
                instance.ReportedNumOfSquares = ReportedNumOfSquares;
            }
        }
    }

    private void SpawnSquare()
    {
        Rigidbody2D newSquare = Instantiate(squarePrefab, transform.position, Quaternion.identity);
        ReportedNumOfSquares++;
        cooldown += 1f / spawnRate;
        if (cooldown <= 0f)
        {
            SpawnSquare();
        }
    }
}
