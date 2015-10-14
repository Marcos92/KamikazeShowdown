using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State
    {
        Idle, Chasing, Attacking
    };
    State currentState;

    NavMeshAgent pathFinder;
    Transform target;
    LivingEntity targetEntity;

    float attackDistThreshold = .5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;

    // To get space after reaching player
    float mycollisionRadious;
    float targetCollisionRadious;

    bool hasTarget;

    public int score;
    
	protected override void Start () 
    {
        base.Start();
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.onDeath += OntargetDeath;

            mycollisionRadious = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadious = target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());

            onDeath += ScoreIncrease;
        }
	}

    void OntargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    void ScoreIncrease()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().combo++;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().score += score * GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().combo;
    }
	
	void Update ()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistToTarget < Mathf.Pow(attackDistThreshold + mycollisionRadious + targetCollisionRadious, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        } 
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathFinder.enabled = false;

        Vector3 originalPosition = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - dirToTarget * (mycollisionRadious + (targetCollisionRadious/2));

        float percent = 0; // 0 to 1
        float attackSpeed = 3;

        skinMaterial.color = Color.red;
        bool hasAppliedDamage = false;

        while (percent <= 1)
        {
            if (percent >= 0.5 && !hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
                GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().combo = 1;
            }
            percent += Time.deltaTime * attackSpeed;
            //parabola
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - dirToTarget * (mycollisionRadious + targetCollisionRadious + attackDistThreshold/2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
