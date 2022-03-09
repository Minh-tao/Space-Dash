using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Crawler : Enemy
{
    private const float MOVE_DIST = 2.5f;
    private Vector3 endpointLeft, endpointRight;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        endpointLeft = transform.position + MOVE_DIST * Vector3.left;
        endpointRight = transform.position + MOVE_DIST * Vector3.right;
        if (transform.eulerAngles == lookLeft)
        {
            body.velocity = new Vector2(-moveSpeed, 0);
        } else if (transform.eulerAngles == lookRight)
        {
            body.velocity = new Vector2(moveSpeed, 0);
        }



    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (transform.position[0] >= endpointRight[0])
        {
            body.velocity = new Vector2(-moveSpeed, body.velocity.y);
            transform.eulerAngles = lookLeft;
        }
        else if (transform.position[0] <= endpointLeft[0])
        {
            body.velocity = new Vector2(moveSpeed, body.velocity.y);
            transform.eulerAngles = lookRight;
        }
    }

    protected override IEnumerator Attack()
    {
        yield return new WaitForSeconds(0);
    }
}
