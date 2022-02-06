using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform player;
    private Vector3 newPos;
    [SerializeField]
    private float minX, maxX, minY, maxY;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!player)
            return;

        newPos = transform.position;
        newPos.x = player.position.x;
        newPos.y = player.position.y;

        if (newPos.x > maxX)
        {
            newPos.x = maxX;
        }
        if (newPos.x < minX)
        {
            newPos.x = minX;
        }
        if (newPos.y > maxY)
        {
            newPos.y = maxY;
        }
        if (newPos.y < minY)
        {
            newPos.y = minY;
        }
        transform.position = newPos;
    }
}
