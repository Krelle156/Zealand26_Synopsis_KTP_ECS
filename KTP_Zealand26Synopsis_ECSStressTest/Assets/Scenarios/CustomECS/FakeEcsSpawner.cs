using UnityEngine;

public class FakeEcsSpawner : MonoBehaviour
{
    public FakeEcsManager manager;

    public SquareFakeEcsComponent squarePrefab;
    public float spawnRate = 1f;
    private float coolDown;

    private int ReportedNumOfSquares = 0;

    private void Update()
    {
        coolDown -= Time.deltaTime;
        if (coolDown <= 0f)
        {
            SpawnSquare();

            SharedUIController instance = SharedUIController.Instance;
            if(instance != null)
            {
                instance.ReportedNumOfSquares = ReportedNumOfSquares;
            }
        }
    }
    
    private void SpawnSquare()
    {
        SquareFakeEcsComponent newSquare = Instantiate(squarePrefab, transform.position, Quaternion.identity);
        newSquare.MoveDirection = Random.insideUnitCircle.normalized;
        newSquare.Speed = Random.Range(0f, 10f);
        manager.squares.Add(newSquare);

        ReportedNumOfSquares++;
        coolDown += 1f / spawnRate;
        if(coolDown <= 0f)
        {
            SpawnSquare();
        }
    }
}
