using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    [SerializeField]
    private int damage;
    [SerializeField]
    private float playerAttackCooldown;
    private const string PROJ_SMALL_PLAYER = "Small Projectile (Player)";
    private const string ENEMY = "Enemy";

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
