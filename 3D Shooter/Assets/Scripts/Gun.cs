using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public Transform muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public int muzzleVelocity = 35;
    public bool spread, flame, shotgun, machinegun, infinite;

    public int ammo;

    float nextShootTime;

    //Laser
    #region
    public bool laser = false;
    //public float laserWidth = 0.5f;
    //public float noise = 1.0f;
    //public float maxLength = 50.0f;
    //public Color color = Color.red;
    //LineRenderer lineRenderer;
    //int length;
    //Vector3[] position;
    ////Cache any transforms here
    //Transform myTransform;
    //Transform endEffectTransform;
    ////The particle system, in this case sparks which will be created by the Laser
    //public ParticleSystem endEffect;
    //Vector3 offset;
    #endregion

    //Laser
    #region
    //void Start()
    //{
    //    if (laser)
    //    {
    //        lineRenderer = GetComponent<LineRenderer>();
    //        lineRenderer.SetWidth(laserWidth, laserWidth);
    //        myTransform = transform;
    //        offset = new Vector3(0, 0, 0);
    //        endEffect = GetComponentInChildren<ParticleSystem>();
    //        if (endEffect) endEffectTransform = endEffect.transform;
    //    }
    //}

    //void RenderLaser()
    //{
    //    //Shoot our laserbeam forwards!
    //    UpdateLength();

    //    lineRenderer.SetColors(color, color);
    //    //Move through the Array
    //    for (int i = 0; i < length; i++)
    //    {
    //        //Set the position here to the current location and project it in the forward direction of the object it is attached to
    //        offset.x = myTransform.position.x + i * myTransform.forward.x + Random.Range(-noise, noise);
    //        offset.z = i * myTransform.forward.z + Random.Range(-noise, noise) + myTransform.position.z;
    //        offset.y = myTransform.position.y;
    //        position[i] = offset;
    //        position[0] = myTransform.position;

    //        lineRenderer.SetPosition(i, position[i]);
    //    }
    //}

    //void UpdateLength()
    //{
    //    //Raycast from the location of the cube forwards
    //    RaycastHit[] hit;
    //    hit = Physics.RaycastAll(myTransform.position, myTransform.forward, maxLength);
    //    int i = 0;
    //    while (i < hit.Length)
    //    {
    //        //Check to make sure we aren't hitting triggers but colliders
    //        if (!hit[i].collider.isTrigger)
    //        {
    //            length = (int)Mathf.Round(hit[i].distance) + 2;
    //            position = new Vector3[length];
    //            //Move our End Effect particle system to the hit point and start playing it
    //            if (endEffect)
    //            {
    //                endEffectTransform.position = hit[i].point;
    //                if (!endEffect.isPlaying)
    //                    endEffect.Play();
    //            }
    //            lineRenderer.SetVertexCount(length);
    //            return;
    //        }
    //        i++;
    //    }
    //    //If we're not hitting anything, don't play the particle effects
    //    if (endEffect)
    //    {
    //        if (endEffect.isPlaying)
    //            endEffect.Stop();
    //    }
    //    length = (int)maxLength;
    //    position = new Vector3[length];
    //    lineRenderer.SetVertexCount(length);


    //}
    #endregion

    public void Shoot()
    {
        if (Time.time > nextShootTime)
        {
            nextShootTime = Time.time + msBetweenShots / 1000;
            ammo--;

            if (laser)
            {
                //RenderLaser();
            }
            else if (flame)
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
}
