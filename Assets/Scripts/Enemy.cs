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
    protected int contactDamage;
    [SerializeField]
    protected int health;
    [SerializeField]
    protected float moveSpeed;
    [SerializeField]
    protected GameObject projectile;

    protected bool readyToAttack = true;
    protected Rigidbody2D body;
    protected Animator anim;

    private const string ANIM_IS_DEAD = "isDead";
    private const string TAG_SMALL_PROJ = "Small Projectile (Player)";

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected abstract IEnumerator Attack();

    protected IEnumerator AttackReady()
    {
        yield return new WaitForSeconds(Random.Range(attackCooldownMin, attackCooldownMax));
        readyToAttack = true;
    }

    public int GetContactDamage()
    {
        return contactDamage;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(TAG_SMALL_PROJ))
        {
            SmallProjectile proj = col.GetComponent<SmallProjectile>();
            TakeDamage(proj.GetDamage());
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
