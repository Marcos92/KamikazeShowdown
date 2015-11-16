using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State
    {
        Idle, Chasing, Attacking
    };
    State currentState;

    public enum Type
    {
        Melee, Roaming, Ranged
    };
    public Type type;

    NavMeshAgent pathFinder;
    Transform target;
    LivingEntity targetEntity;

    public float attackDistThreshold = .5f;
    float timeBetweenAttacks = 1;
    public float meleeDamage = 1;

    float nextAttackTime;

    //Ranged enemies
    public Projectile projectile;
    public float msBetweenShots, projectileSpeed;
    float nextShootTime;

    //Roaming 
    public LayerMask collisionMask;

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

        transform.forward = new Vector3(0.5f, 0, 0.5f);

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
            if(type == Type.Melee)
            {
                if (percent >= 0.5 && !hasAppliedDamage)
                {
                    hasAppliedDamage = true;
                    targetEntity.TakeDamage(meleeDamage);
                }
                percent += Time.deltaTime * attackSpeed;
                float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
            }
            else if (type == Type.Ranged)
            {
                dirToTarget = (target.position - transform.position).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToTarget), Time.deltaTime * 5f);

                if (Time.time > nextShootTime)
                {
                    nextShootTime = Time.time + msBetweenShots / 1000;
                    Projectile newProjectile = Instantiate(projectile, transform.position, transform.rotation) as Projectile;
                    newProjectile.SetSpeed(projectileSpeed);
                }
                percent += Time.deltaTime * 0.5f;
            }

            yield return null;
        }

        skinMaterial.color = originalColor;
        currentState = State.Chasing;
        pathFinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {
        float refreshRate = 0.25f;

        if(type != Type.Roaming)
        {
            while (hasTarget)
            {
                if (currentState == State.Chasing)
                {
                    Vector3 dirToTarget = (target.position - transform.position).normalized;
                    Vector3 targetPosition = target.position - dirToTarget * (mycollisionRadious + targetCollisionRadious + attackDistThreshold / 2);
                    if (!dead)
                    {
                        pathFinder.SetDestination(targetPosition);
                    }
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            while(true)
            {
                Vector3 targetPosition = transform.position + transform.forward;
                if (!dead) pathFinder.SetDestination(targetPosition);

                Ray ray = new Ray(transform.position, transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 10f, collisionMask, QueryTriggerInteraction.Collide))
                {
                    transform.Rotate(Vector3.up, (float)Math.PI / 2.0f);
                }

                Debug.Log(hit.collider);

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
