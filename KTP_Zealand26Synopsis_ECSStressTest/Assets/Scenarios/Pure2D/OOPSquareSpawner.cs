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

    // Update is called once per frame
    void Update()
    {
        if (cooldown <= 0f)
        {
            SpawnSquares(cooldown);
            SharedUIController instance = SharedUIController.Instance;
            if (instance != null)
            {
                instance.ReportedNumOfSquares = ReportedNumOfSquares;
            }
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }

    private void SpawnSquares(float coolDownState)
    {
        SelfMovingSquare newSquare = Instantiate(squarePrefab, transform.position, Quaternion.identity);
        newSquare.direction = Random.insideUnitCircle.normalized;
        newSquare.speed = Random.Range(0f, 10f);
        ReportedNumOfSquares++;
        coolDownState += 1/spawnRate;
        if(coolDownState < 0f)
        {
            Debug.Log("Recursively doing thing");
            SpawnSquares(coolDownState);
        }
    }
}
