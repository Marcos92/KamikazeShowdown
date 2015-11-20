using UnityEngine;
using System.Collections;

public class Boss1 : LivingEntity 
{
    //Find player
    Transform target;
    LivingEntity targetEntity;
    bool hasTarget;

    //Shooting
    public Projectile projectile;
    public float msBetweenShots, projectileSpeed;
    float nextShootTime;

    public int score;

	// Use this for initialization
	void Start () 
    {
        base.Start();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.onDeath += OntargetDeath;

            onDeath += ScoreIncrease;
        }
	}

    void OntargetDeath()
    {
        hasTarget = false;
    }

    void ScoreIncrease()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().combo++;
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().score += score * GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().combo;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (hasTarget)
        {
            //Point to target

            //Shoot
        }

        //Move
	}
}
