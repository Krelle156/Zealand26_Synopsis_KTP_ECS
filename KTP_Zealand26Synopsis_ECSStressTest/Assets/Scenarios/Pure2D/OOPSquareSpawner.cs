using UnityEngine;

public class OOPSquareSpawner : MonoBehaviour
{
    public SelfMovingSquare squarePrefab;
    public float spawnRate = 1f;
    private float cooldown;

    public int reportInterval = 1000;
    private int squareCount = 0;

    // Update is called once per frame
    void Update()
    {
        if (cooldown <= 0f)
        {
            SelfMovingSquare newSquare = Instantiate(squarePrefab, transform.position, Quaternion.identity);
            newSquare.direction = Random.insideUnitCircle.normalized;
            newSquare.speed = Random.Range(0f, 10f);
            cooldown = 1f / spawnRate;
            squareCount++;
            if (squareCount % reportInterval == 0)
            {
                Debug.Log($"Spawned square no. {squareCount}. The program is still running at {(int)(1 / Time.unscaledDeltaTime)} FPS.");
            }
        }
        else
        {
            cooldown -= Time.deltaTime;
        }
    }
}
