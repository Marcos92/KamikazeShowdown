using UnityEngine;
using System.Collections;

[RequireComponent (typeof (NavMeshAgent))]
public class Enemy : LivingEntity {

    public enum State
    {
        Idle, Chasing, Attacking
    };
    State currentState;

    public enum Type
    {
        Melee, Roaming, Ranged, Boss1
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
    public Transform shootPosition;
    public float msBetweenShots, projectileSpeed;
    float nextShootTime;

    //Roaming 
    public LayerMask collisionMask;

    //Boss 1
    Vector3 movement;
    int direction;

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

        if (type == Type.Roaming)
        {
            int r = Random.Range(0, 4);
            if(r == 0) transform.forward = new Vector3(0.5f, 0, 0.5f);
            else if(r == 1) transform.forward = new Vector3(-0.5f, 0, 0.5f);
            else if (r == 2) transform.forward = new Vector3(0.5f, 0, -0.5f);
            else transform.forward = new Vector3(-0.5f, 0, -0.5f);
        }

        if (type == Type.Boss1)
        {
            int direction = Random.Range(0, 2);
            if (direction == 0) direction = -1;

            movement = Vector3.right * direction;
        }

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            transform.forward = (new Vector3(target.position.x, 0, target.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.onDeath += OntargetDeath;

            mycollisionRadious = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadious = target.GetComponent<CapsuleCollider>().radius;

            if(type == Type.Melee || type == Type.Roaming || type == Type.Ranged) StartCoroutine(UpdatePath());

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
        //Boss1
        if(type == Type.Boss1)
        {
            #region
            if (hasTarget)
            {
                //Point to target
                Vector3 dirToTarget = (new Vector3(target.position.x, 0, target.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToTarget), Time.deltaTime * 100f);

                //Shoot
                if (nextShootTime < Time.time)
                {
                    nextShootTime = Time.time + msBetweenShots / 1000f;

                    Projectile newProjectile = Instantiate(projectile, shootPosition.position, transform.rotation) as Projectile;
                    newProjectile.SetSpeed(projectileSpeed);
                }
            }

            //Move
            transform.position += movement * Time.deltaTime * pathFinder.speed;

            //Check walls
            Ray ray = new Ray(transform.position, movement);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3f, collisionMask, QueryTriggerInteraction.Collide))
            {
                movement = Quaternion.Euler(0, 90f, 0) * movement;
            }
            #endregion
        }
        //General
        else
        {
            #region
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
            #endregion
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
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dirToTarget), Time.deltaTime * 2f);

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
        if(type != Type.Roaming)
        {
            float refreshRate = 0.25f;

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
            float refreshRate = 0.1f;

            while(true)
            {
                Vector3 targetPosition = transform.position + transform.forward;
                if (!dead) pathFinder.SetDestination(targetPosition);

                Ray ray = new Ray(transform.position, transform.forward);
                //Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 4, transform.position.z), transform.forward, Color.cyan);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 6f, collisionMask, QueryTriggerInteraction.Collide))
                {
                    transform.forward = Vector3.Reflect(transform.forward, hit.normal);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
