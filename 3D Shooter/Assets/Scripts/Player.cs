using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

    public float moveSpeed = 5;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    float nextDodgeTime;
    public int msBetweenDodges = 500;
    public float dodgeVelocityMultiplier = 1.5f;
    bool dodging;

    public int combo;
    public int score;

	protected override void Start ()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        combo = 1;
	}
	
	void Update ()
    {
        // Movement Input
	    Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        if (!dodging) controller.Move(moveVelocity);
        else controller.Move(moveVelocity * dodgeVelocityMultiplier);

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
        if (!dodging && Input.GetMouseButtonDown(1) && moveVelocity != Vector3.zero)
        {
            dodging = true;
            nextDodgeTime = Time.time + msBetweenDodges / 1000f;
        }
        else if (dodging && Time.time > nextDodgeTime)
        {
            dodging = false;
        }

        if (dodging) skinMaterial.color = Color.green;
        else skinMaterial.color = originalColor;
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
