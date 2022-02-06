using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallProjectile : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float playerAttackCooldown;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float timeBeforeDespawning;
    private const string PROJ_SMALL_PLAYER = "Small Projectile (Player)";
    private const string ENEMY = "Enemy";

    private Vector3 lookRight = Vector3.zero;
    private Vector3 lookLeft = Vector3.up * 180f;
    protected Rigidbody2D body;

    private void Awake()
    {
        StartCoroutine(Despawn());
        body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
    }

    protected void Move()
    {
        if (transform.eulerAngles == lookRight)
        {
            body.velocity = new Vector2(moveSpeed, body.velocity.y);
        }
        else if (transform.eulerAngles == lookLeft)
        {
            body.velocity = new Vector2(-moveSpeed, body.velocity.y);
        }
    }
    protected IEnumerator Despawn()
    {
        yield return new WaitForSeconds(timeBeforeDespawning);
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }

    public float GetAttackCooldown()
    {
        return playerAttackCooldown;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
    }
}
