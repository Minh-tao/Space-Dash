using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Marine : Enemy
{
    protected const float BURST_GAP = 0.1f;
    protected const int BURST_COUNT = 3;
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (readyToAttack)
        {
            StartCoroutine(Attack());
            readyToAttack = false;
        }
    }
    protected override IEnumerator Attack()
    {
        for (int i = 1; i <= BURST_COUNT; i++)
        {
            Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
            yield return new WaitForSeconds(BURST_GAP);
        }
        StartCoroutine(AttackReady());
    }
}
