using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
    public LayerMask collisionMask;
    float speed;
    public float damage = 1;
    public bool bouncing;

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
    }

	public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
	
	void Update () 
	{
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
        if(!bouncing) GameObject.Destroy(gameObject);
    }

    void OnHitObject(Collider c)
    {
        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeDamage(damage);
        }
        if(!bouncing) GameObject.Destroy(gameObject);
    }
}
