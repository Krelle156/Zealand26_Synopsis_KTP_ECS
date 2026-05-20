using UnityEngine;

public class SelfMovingSquare : MonoBehaviour
{
    public float speed;
    public Vector2 direction;
    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
    }
}
