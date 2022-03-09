using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float dashTime;
    [SerializeField] private float crouchCooldown;
    [SerializeField] private float meleeForce;
    [SerializeField] private float meleeCooldown;
    [SerializeField] private int meleeDamage;
    [SerializeField] private float crouchTime;
    [SerializeField] private int maxHealth;
    [SerializeField] private SmallProjectile projectile;
    private int health;

    private float moveX;
    private bool isReadytoMelee = true;
    private bool isReadyToShoot = true;
    private bool isReadyToDash = true;
    private bool isReadyToCrouch = true;
    private Vector3 lookRight = Vector3.zero;
    private Vector3 lookLeft = Vector3.up * 180f;

    public delegate void playerTookDamageDel(int health);
    public static event playerTookDamageDel playerTookDamage;

    private int tempMeleeDamage;
    private Rigidbody2D body;
    private Animator anim;
    private new AudioSource audio;
    private CapsuleCollider2D colStand, colCrouch;
    private CapsuleCollider2D[] colArr;
    private BoxCollider2D meleeCol;

    private const string ANIM_IS_AIRBORNE = "isAirborne";
    private const string ANIM_IS_RUNNING = "isRunning";
    private const string ANIM_IS_WALKING = "isWalking";
    private const string ANIM_IS_DASHING = "isDashing";
    private const string ANIM_IS_CROUCHING = "isCrouching";
    private const string ANIM_IS_DEAD = "isDead";
    private const string ANIM_IS_SHOOTING = "isShooting";
    private const string ANIM_IS_MELEEING = "isMeleeing";
    private const float ANIM_SHOOT_LENGTH = 0.46f;
    private const string TAG_GROUND = "Terrain";
    private const string TAG_ENEMY = "Enemy";
    private const string TAG_SMALL_PROJ = "Small Projectile (Enemy)";
    private const KeyCode KEY_JUMP = KeyCode.Space;
    private const KeyCode KEY_CROUCH = KeyCode.LeftControl;
    private const KeyCode KEY_DASH = KeyCode.LeftShift;
    private const KeyCode KEY_SHOOT = KeyCode.Q;
    private const KeyCode KEY_MELEE = KeyCode.F;
    private const float DIST_SPAWN_PROJ = 0.25f;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        colArr = GetComponents<CapsuleCollider2D>();
        colStand = colArr[0];
        colCrouch = colArr[1];
        meleeCol = GetComponent<BoxCollider2D>();
        health = maxHealth;
        tempMeleeDamage = 0;
    }
    private void Update()
    {
        PlayerMove();
        PlayerJump();
        PlayerCrouchStart();
        PlayerDashStart();
        PlayerShoot();
        PlayerMelee();
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
        if (Input.GetKey(KEY_JUMP) && !anim.GetBool(ANIM_IS_AIRBORNE) && health > 0)
        {
            anim.SetBool(ANIM_IS_AIRBORNE, true);
            body.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }
    }
    private void PlayerCrouchStart()
    {    
        if (Input.GetKey(KEY_CROUCH) && isReadyToCrouch && !anim.GetBool(ANIM_IS_AIRBORNE) && !anim.GetBool(ANIM_IS_DASHING) && health > 0)
        {
            isReadyToCrouch = false;
            StartCoroutine(PlayerCrouch());
        }
    }

    private IEnumerator PlayerCrouch()
    {
        colStand.enabled = false;
        colCrouch.enabled = true;
        float tempSpeed = moveSpeed;
        moveSpeed = 0;
        body.velocity = new Vector2(0f, 0f);
        yield return new WaitForSeconds(crouchTime);
        colStand.enabled = true;
        colCrouch.enabled = false;
        moveSpeed = tempSpeed;
        StartCoroutine(ReadyToCrouch());
    }
    private IEnumerator ReadyToCrouch()
    {
        yield return new WaitForSeconds(crouchCooldown);
        isReadyToCrouch = true;
    }

    private void PlayerDashStart()
    {
        if (Input.GetKey(KEY_DASH) && isReadyToDash && (moveX == 1 || moveX == -1))
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

    private void PlayerMelee()
    {
        if (Input.GetKey(KEY_MELEE) && isReadytoMelee && health > 0 && !anim.GetBool(ANIM_IS_SHOOTING) && !anim.GetBool(ANIM_IS_DASHING))
        {
            tempMeleeDamage = meleeDamage;
            isReadytoMelee = false;
            body.velocity = new Vector2(meleeForce * moveX, body.velocity.y);
            anim.SetBool(ANIM_IS_MELEEING, true);
            meleeCol.enabled = true;
        }
    }

    private void EndMelee()
    {
        tempMeleeDamage = 0;
        anim.SetBool(ANIM_IS_MELEEING, false);
        meleeCol.enabled = false;
        StartCoroutine(ReadyToMelee(meleeCooldown));
    }

    private IEnumerator ReadyToMelee(float attackCooldown)
    {
        yield return new WaitForSeconds(attackCooldown);
        isReadytoMelee = true;
    }
    private void PlayerShoot()
    {
        if (Input.GetKey(KEY_SHOOT) && isReadyToShoot && health > 0 && !anim.GetBool(ANIM_IS_MELEEING) && !anim.GetBool(ANIM_IS_DASHING))
        {
            Vector3 spawnPos = gameObject.transform.position;
            if (transform.eulerAngles == lookRight)
            {
                spawnPos += Vector3.right * DIST_SPAWN_PROJ;
            }
            else if (transform.eulerAngles == lookLeft)
            {
                spawnPos += Vector3.left * DIST_SPAWN_PROJ;
            }
            SmallProjectile proj = Instantiate(projectile, spawnPos, gameObject.transform.rotation);
            isReadyToShoot = false;
            anim.SetBool(ANIM_IS_SHOOTING, true);
            StartCoroutine(EndShoot(ANIM_SHOOT_LENGTH, proj.GetAttackCooldown()));

        }
    }

    private IEnumerator EndShoot(float animDuration, float attackCooldown)
    {
        yield return new WaitForSeconds(animDuration);
        anim.SetBool(ANIM_IS_SHOOTING, false);
        StartCoroutine(ReadyToShoot(attackCooldown));
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
        if (moveX == 1 || moveX == -1)
        {
            anim.SetBool(ANIM_IS_RUNNING, true);
            anim.SetBool(ANIM_IS_WALKING, false);
        }
        else if(moveX > 0 || moveX < 0)
        {
            anim.SetBool(ANIM_IS_RUNNING, false);
            anim.SetBool(ANIM_IS_WALKING, true);
        }
        else
        {
            anim.SetBool(ANIM_IS_RUNNING, false);
            anim.SetBool(ANIM_IS_WALKING, false);
        }

        if (body.gravityScale == 0)
        {
            anim.SetBool(ANIM_IS_DASHING, true);
        } 
        else
        {
            anim.SetBool(ANIM_IS_DASHING, false);
        }

        if (colCrouch.enabled)
        {
            anim.SetBool(ANIM_IS_CROUCHING, true);
        } 
        else
        {
            anim.SetBool(ANIM_IS_CROUCHING, false);
        }
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag(TAG_GROUND))
        {
            anim.SetBool(ANIM_IS_AIRBORNE, false);
        }
        if (col.gameObject.CompareTag(TAG_ENEMY) && meleeCol.enabled == false)
        {
            TakeDamage(1);
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag(TAG_SMALL_PROJ) && meleeCol.enabled == false)
        {
            SmallProjectile proj = col.GetComponent<SmallProjectile>();
            TakeDamage(proj.GetDamage());
            //Knockback(1f);
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

    private void Knockback(float force)
    {
        if (transform.eulerAngles == lookRight)
        {
            body.AddForce(new Vector2(force, 0), ForceMode2D.Impulse);
        }
        else if (transform.eulerAngles == lookLeft)
        {
            body.AddForce(new Vector2(force, 0), ForceMode2D.Impulse);
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

    public int GetMeleeDamage()
    {
        return tempMeleeDamage;
    }
}
    