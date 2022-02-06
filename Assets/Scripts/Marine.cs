using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class Marine : Enemy
{
    protected const float BURST_GAP_MIN = 0.08f;
    protected const float BURST_GAP_MAX = 0.13f;

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
        Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
        yield return new WaitForSeconds(Random.Range(BURST_GAP_MIN, BURST_GAP_MAX));
        Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
        yield return new WaitForSeconds(Random.Range(BURST_GAP_MIN, BURST_GAP_MAX));
        Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation);
        StartCoroutine(AttackReady());
    }
}
