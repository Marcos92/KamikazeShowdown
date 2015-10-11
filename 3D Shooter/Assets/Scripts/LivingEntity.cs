using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHealth;
    protected float health;
    protected bool dead;

    Material skinMaterial;
    Color originalColor;

    public event System.Action onDeath;

    protected virtual void Start()
    {
        health = startingHealth;
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;
    }

	public void TakeHit(float damage, RaycastHit hit)
    {
        // do some stuff here with hit var
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
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
