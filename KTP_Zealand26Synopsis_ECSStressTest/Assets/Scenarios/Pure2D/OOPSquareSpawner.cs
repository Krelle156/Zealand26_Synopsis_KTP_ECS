using UnityEngine;

public class OOPSquareSpawner : MonoBehaviour
{
    public SelfMovingSquare squarePrefab;
    public float spawnRate = 1f;
    private float cooldown;

    private static int ReportedNumOfSquares = 0;

    public void Awake()
    {
        ReportedNumOfSquares = 0;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;
        if (cooldown <= 0f)
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
        SelfMovingSquare newSquare = Instantiate(squarePrefab, transform.position, Quaternion.identity);
        newSquare.direction = Random.insideUnitCircle.normalized;
        newSquare.speed = Random.Range(0f, 10f);
        ReportedNumOfSquares++;
        cooldown += 1f/spawnRate;
        if(cooldown <= 0f)
        {
            SpawnSquare();
        }
    }
}
