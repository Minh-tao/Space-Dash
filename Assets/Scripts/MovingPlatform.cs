using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private const float MOVE_SPEED = 1.5f;
    private const float MOVE_DIST = 4f;
    private Vector3 endpointLeft, endpointRight;

    protected Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        endpointLeft = transform.position + MOVE_DIST * Vector3.left;
        endpointRight = transform.position + MOVE_DIST * Vector3.right;
        body.velocity = new Vector2(MOVE_SPEED, 0);
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (transform.position[0] >= endpointRight[0])
        {
            body.velocity = new Vector2(-MOVE_SPEED, body.velocity.y);
        }
        else if (transform.position[0] <= endpointLeft[0])
        {
            body.velocity = new Vector2(MOVE_SPEED, body.velocity.y);
        }
    }
}

