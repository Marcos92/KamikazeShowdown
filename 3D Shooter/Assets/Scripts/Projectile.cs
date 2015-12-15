using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
    public LayerMask collisionMask;
    float speed;
    public float damage = 1;
    public bool bouncing;
    public bool explosive;
    public float explosionRadius;
    public bool missile;
    float timeBetweenSearches = 0.25f, nextSearchTime;
    Vector3 targetPosition;
    Transform closestTarget;

    public float lifeTime = 2;
    float skinWidth = 0.1f;

    public Color trailColor;

    private AudioSource audioSource;

    void Start()
    {
        Destroy(gameObject, lifeTime);
        audioSource = GetComponent<AudioSource>();

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if (initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0]);
        }
        if (GetComponent<TrailRenderer>() != null)
        {
            GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
        }

        if (missile) nextSearchTime = Time.time + timeBetweenSearches;
    }

	public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
	
	void Update () 
	{
        if(missile)
        {
            if(Time.time > nextSearchTime)
            {
                nextSearchTime = Time.time + timeBetweenSearches;

                closestTarget = null;

                foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
                {
                    if (closestTarget == null) closestTarget = enemy.transform;
                    else if (Vector3.Distance(closestTarget.position, transform.position) > Vector3.Distance(enemy.transform.position, transform.position)) closestTarget = enemy.transform;
                }

                if (closestTarget != null) targetPosition = closestTarget.position;
            }

            if (closestTarget != null)
            {
                Vector3 dirToTarget = (targetPosition - transform.position).normalized;
                Quaternion rotation = Quaternion.LookRotation(dirToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.25f);
            }
        }

        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
	}

    void CheckCollisions(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit);
            if (bouncing)
            {
                transform.forward = Vector3.Reflect(transform.forward, hit.normal);
                audioSource.Play();
            }
        }
    }
    void OnHitObject(RaycastHit hit)
    {
        IDamageable damageableObject = hit.collider.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hit);
        }
        if (explosive) Explosion();
        if(!bouncing) GameObject.Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        if (explosive) Explosion();
        if(!bouncing) GameObject.Destroy(gameObject);
    }

    void Explosion()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].gameObject.GetComponent<LivingEntity>() != null) 
                hitColliders[i].gameObject.GetComponent<LivingEntity>().TakeDamage(damage / 2);
        }
    }
}
