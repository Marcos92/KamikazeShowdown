using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

    public float moveSpeed = 5;

    Camera viewCamera;
    PlayerController controller;
    public GunController gunController;

    float nextDodgeTime;
    public int msBetweenDodges = 500;
    public float dodgeVelocityMultiplier = 1.5f;
    bool dodging;
    bool canDodge;
    int i = 0;

    [HideInInspector]
    public int combo;
    [HideInInspector]
    public int score;

    float currentHealth;

	protected override void Start ()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        combo = 1;
        canDodge = true;
        dodging = false;
	}
	
	void Update ()
    {
        // Movement Input
	    Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        if (!dodging) controller.Move(moveVelocity);
        else if (dodging)
        {
            i++;
            controller.Move(moveInput.normalized * 40f);//controller.Move(moveVelocity * dodgeVelocityMultiplier);
        }
        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
        }

        // Weapon input
        if (!dodging && Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }

        //Dodge
        if (canDodge && !dodging && Input.GetMouseButtonDown(1) && moveVelocity != Vector3.zero)
        {
            dodging = true;
            canDodge = false;
            nextDodgeTime = Time.time + msBetweenDodges / 1000f;
        }
        else if (dodging && i > 5)
        {
            dodging = false;
            i = 0;
        }
        else if (!canDodge && Time.time > nextDodgeTime)
        {
            canDodge = true;
        }

        //if (dodging) skinMaterial.color = Color.green;
        //else skinMaterial.color = originalColor;

        //Ammo
        if (!gunController.equippedGun.infinite && gunController.equippedGun.ammo <= 0)
            gunController.EquipGun(gunController.startingGun);

        //Combo 
        if(health < currentHealth) combo = 1;

        currentHealth = health;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PickUp")
        {
            gunController.EquipGun(c.gameObject.GetComponent<PickUp>().gun);
            Destroy(c.gameObject);
        }
    }
}
