using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

abstract public class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float attackCooldownMin;
    [SerializeField]
    protected float attackCooldownMax;
    [SerializeField]
    protected int health;
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected GameObject projectile;

    protected bool readyToAttack = true;
    protected Rigidbody2D body;
    protected Animator anim;

    protected Vector3 lookRight = Vector3.zero;
    protected Vector3 lookLeft = Vector3.up * 180f;
    private const string ANIM_IS_DEAD = "isDead";
    private const string TAG_SMALL_PROJ = "Small Projectile (Player)";
    private const string TAG_PLAYER = "Player";

    protected abstract IEnumerator Attack();

    protected IEnumerator AttackReady()
    {
        yield return new WaitForSeconds(Random.Range(attackCooldownMin, attackCooldownMax));
        readyToAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(TAG_SMALL_PROJ))
        {
            SmallProjectile proj = col.GetComponent<SmallProjectile>();
            TakeDamage(proj.GetDamage());
        }
        if (col.gameObject.CompareTag(TAG_PLAYER))
        {
            Player playerMelee = col.GetComponent<Player>();
            TakeDamage(playerMelee.GetMeleeDamage());
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            anim.SetBool(ANIM_IS_DEAD, true);
        }
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
