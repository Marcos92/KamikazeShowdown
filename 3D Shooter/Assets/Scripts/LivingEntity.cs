using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHealth;
    protected float health;
    protected bool dead;

    public Material skinMaterial;
    public Color originalColor;

    public event System.Action onDeath;

    public GameObject deathParticle;

    protected virtual void Start()
    {
        health = startingHealth;
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;
    }

	public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        Quaternion back = new Quaternion(transform.rotation.x, -transform.rotation.y, transform.rotation.z, 0);
        Destroy(Instantiate(deathParticle, gameObject.transform.position, back) as GameObject, 2);
        health -= damage;
        if (health <= 0 && !dead)
        {
            
            Die();
        }
    }

    protected void Die()
    {
        dead = true;
        if (onDeath != null) onDeath();
        GameObject.Destroy(gameObject);
    }

    IEnumerator ChangeColor()
    {
        yield return new WaitForSeconds(10);
    }
}
