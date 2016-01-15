using UnityEngine;
using System.Collections;

public class LivingEntity : MonoBehaviour, IDamageable {

    public float startingHealth;
    public float health;
    protected bool dead;

    public event System.Action onDeath;

    public GameObject hitParticle;
    public GameObject enemyDeathParticle;

    public AudioClip death;
    AudioSource audioSource;

    protected virtual void Start()
    {
        health = startingHealth;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

	public void TakeHit(float damage, RaycastHit hit)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        Quaternion back = new Quaternion(transform.rotation.x, -transform.rotation.y, transform.rotation.z, 0);
        Destroy(Instantiate(hitParticle, gameObject.transform.position, back) as GameObject, 2);
        health -= damage;
        if (health <= 0 && !dead)
        {
            if (gameObject.tag == "Enemy")
            {
                audioSource.clip = death;
                audioSource.Play();
                Destroy(Instantiate(enemyDeathParticle, gameObject.transform.position, Quaternion.FromToRotation(transform.position, transform.position - Camera.main.ScreenToViewportPoint(Input.mousePosition))), 2);
            }
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
