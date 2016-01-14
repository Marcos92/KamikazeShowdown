using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public int muzzleVelocity = 35;
    public bool spread, flame, shotgun, machinegun, infinite;
    public AudioSource audioSource;
    public string weaponName;

    [Range(0,1)]
    public float audioFade = .9f; // damping coefficient for the audio volume. .98 is a slow fade, .9 is a fast fade

    public int ammo;

    public float nextShootTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Shoot()
    {
        audioSource.volume = 1;
        if (Time.time > nextShootTime)
        {
            nextShootTime = Time.time + msBetweenShots / 1000;

            if (!flame)
                audioSource.Play();
            else if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            ammo--;

            if (flame)
            {
                Vector3 rot = muzzle.rotation.eulerAngles;
                int r = Random.Range(-15, 16);
                rot = new Vector3(rot.x, rot.y + r, rot.z);
                Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.Euler(rot)) as Projectile;
                newProjectile.lifeTime += Random.Range(-0.1f, 0.1f);
                newProjectile.SetSpeed(muzzleVelocity);

            }
            else
            {
                if (machinegun)
                {
                    Vector3 rot = muzzle.rotation.eulerAngles;
                    int r = Random.Range(-2, 3);
                    rot = new Vector3(rot.x, rot.y + r, rot.z);
                    Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.Euler(rot)) as Projectile;
                    newProjectile.SetSpeed(muzzleVelocity);
                }
                else
                {
                    Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
                    newProjectile.SetSpeed(muzzleVelocity);

                }
            }

            //Spread-shot
            if (spread)
            {
                Vector3 rotL = muzzle.rotation.eulerAngles;
                Vector3 rotR = rotL;

                rotL = new Vector3(rotL.x, rotL.y - 15, rotL.z);
                rotR = new Vector3(rotR.x, rotR.y + 15, rotR.z);

                Projectile newProjectileR = Instantiate(projectile, muzzle.position, Quaternion.Euler(rotR)) as Projectile;
                newProjectileR.SetSpeed(muzzleVelocity);
                Projectile newProjectileL = Instantiate(projectile, muzzle.position, Quaternion.Euler(rotL)) as Projectile;
                newProjectileL.SetSpeed(muzzleVelocity);
            }

            //Shotgun
            if(shotgun)
            {
                for(int i = 0; i < 14; i++)
                {
                    Vector3 rot = muzzle.rotation.eulerAngles;
                    int r = Random.Range(-15, 16);
                    int v = Random.Range(muzzleVelocity, muzzleVelocity + 10);
                    rot = new Vector3(rot.x, rot.y + r, rot.z);
                    Projectile newProjectile = Instantiate(projectile, muzzle.position, Quaternion.Euler(rot)) as Projectile;
                    newProjectile.SetSpeed(v);
                }
            }
        }            
    }

    public void FadeOut()
    {
        if (audioSource.volume > .01f)
        {
            audioSource.volume *= audioFade;
        }
        else
            audioSource.Stop();
    }
}
