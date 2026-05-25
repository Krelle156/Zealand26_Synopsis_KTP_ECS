using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class FakeEcsManager : MonoBehaviour
{
    public List<SquareFakeEcsComponent> squares = new List<SquareFakeEcsComponent>();

    public void Update()
    {
        foreach (SquareFakeEcsComponent square in squares)
        {
            Vector2 direction = square.MoveDirection;
            float speed = square.Speed;
            square.transform.position += new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
        }
    }
}
