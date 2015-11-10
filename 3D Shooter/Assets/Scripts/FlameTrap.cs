using UnityEngine;
using System.Collections;

public class FlameTrap : MonoBehaviour {

    public Projectile flame;
    public bool rotate, alternated;
    public float rotationSpeed;
    public float msBetweenShots, shotSpeed, cooldown;
    float nextShootTime, nextCooldownTime;
    public bool on;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(rotate)
        {
            transform.Rotate(Vector3.up, rotationSpeed);
        }

        if (alternated)
        {
            if ((Time.time > nextCooldownTime) && on)
            {
                on = false;
                nextCooldownTime = Time.time + cooldown;
            }
            else if ((Time.time > nextCooldownTime) && !on)
            {
                on = true;
                nextCooldownTime = Time.time + cooldown;
            }
        }
        else if (!on) on = true;

        if ((Time.time > nextShootTime) && on)
        {
            nextShootTime = Time.time + msBetweenShots / 1000;

            Vector3 rot = transform.rotation.eulerAngles;
            int r = Random.Range(-15, 16);
            rot = new Vector3(rot.x, rot.y + r, rot.z);
            Projectile newProjectile = Instantiate(flame, transform.position + transform.forward * 2, Quaternion.Euler(rot)) as Projectile;
            newProjectile.lifeTime += Random.Range(-0.1f, 0.1f);
            newProjectile.SetSpeed(shotSpeed);
        }
	}
}
