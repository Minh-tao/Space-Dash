using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float dashForce;
    [SerializeField]
    private float dashCooldown;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private int maxHealth;
    private int health;
    [SerializeField]
    private SmallProjectile projectile;

    private float moveX;
    private bool isReadyToShoot = true;
    private bool isReadyToDash = true;
    private Vector3 lookRight = Vector3.zero;
    private Vector3 lookLeft = Vector3.up * 180f;

    public delegate void playerTookDamageDel(int health);
    public static event playerTookDamageDel playerTookDamage;

    private Rigidbody2D body;
    private Animator anim;
    private new AudioSource audio;

    private const string ANIM_IS_AIRBORNE = "isAirborne";
    private const string ANIM_IS_RUNNING = "isRunning";
    private const string ANIM_IS_DASHING = "isDashing";
    private const string ANIM_IS_DEAD = "isDead";
    private const string TAG_GROUND = "Terrain";
    private const string TAG_ENEMY = "Enemy";
    private const string TAG_SMALL_PROJ = "Small Projectile (Enemy)";

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        health = maxHealth;
    }
    private void Update()
    {
        PlayerMove();
        PlayerJump();
        PlayerDashStart();
        PlayerShoot();
        Animate();
    }

    private void PlayerMove()
    {
        moveX = Input.GetAxis("Horizontal");    
        transform.position += new Vector3(moveX, 0f, 0f) * Time.deltaTime * moveSpeed;
        Rotate();
    }

    private void PlayerJump()
    {
        if (Input.GetKey(KeyCode.Space) && !anim.GetBool(ANIM_IS_AIRBORNE) && health > 0)
        {
            anim.SetBool(ANIM_IS_AIRBORNE, true);
            body.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }

    private void PlayerDashStart()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isReadyToDash && (moveX == 1 || moveX == -1))
        {
            isReadyToDash = false;
            StartCoroutine(PlayerDash());
        }
    }
    private IEnumerator PlayerDash()
    {
        body.velocity = new Vector2(dashForce * moveX, 0f);
        body.gravityScale = 0;
        Rotate();
        yield return new WaitForSeconds(dashTime);
        body.gravityScale = 1;
        StartCoroutine(ReadyToDash());
    }

    private IEnumerator ReadyToDash()
    {
        yield return new WaitForSeconds(dashCooldown);
        isReadyToDash = true;
    }

    private void PlayerShoot()
    {
        if (Input.GetKey(KeyCode.F) && isReadyToShoot && health > 0)
        {
            SmallProjectile proj = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
            isReadyToShoot = false;
            StartCoroutine(ReadyToShoot(proj.GetAttackCooldown()));
        }
    }

    private IEnumerator ReadyToShoot(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown);
        isReadyToShoot = true;
    }

    private void Rotate()
    {
        if (moveX > 0)
        {
            transform.eulerAngles = lookRight;
        }
        else if (moveX < 0)
        {
            transform.eulerAngles = lookLeft;
        }
    }

    private void Animate()
    {
        if (moveX > 0 || moveX < 0)
        {
            anim.SetBool(ANIM_IS_RUNNING, true);
        }
        else
        {
            anim.SetBool(ANIM_IS_RUNNING, false);
        }

        if (body.gravityScale == 0)
        {
            anim.SetBool(ANIM_IS_DASHING, true);
        } 
        else
        {
            anim.SetBool(ANIM_IS_DASHING, false);
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(TAG_GROUND))
        {
            anim.SetBool(ANIM_IS_AIRBORNE, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(TAG_SMALL_PROJ))
        {
            SmallProjectile proj = col.GetComponent<SmallProjectile>();
            TakeDamage(proj.GetDamage());
        }

        if (col.gameObject.CompareTag(TAG_ENEMY))
        {
            Enemy proj = col.GetComponent<Enemy>();
            TakeDamage(proj.GetContactDamage());
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
        if (playerTookDamage != null)
        {
            playerTookDamage(health);
        }
    }

    private void Freeze()
    {
        moveSpeed = 0;
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }


    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
    