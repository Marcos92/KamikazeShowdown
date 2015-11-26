using UnityEngine;
using System.Collections;

[RequireComponent (typeof (PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

    public float moveSpeed = 5;
    Vector3 moveVelocity;

    Camera viewCamera;
    PlayerController controller;
    public GunController gunController;

    float nextDodgeTime;
    public int msBetweenDodges = 500;
    public float dodgeSpeed = 1.5f;
    bool dodging;
    bool canDodge;
    int i = 0;

    [HideInInspector]
    public int combo;
    [HideInInspector]
    public int score;

    float currentHealth;

    // Animation stuff
    Animator anim;
    Transform cam;
    Vector3 camForward;
    Vector3 move;
    float forwardAmount;
    float turnAmount;

	protected override void Start ()
    {
        base.Start();
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        combo = 1;
        canDodge = true;
        dodging = false;

        anim = GetComponentInChildren<Animator>();
        cam = Camera.main.transform;
	}
	
	void Update ()
    {
        // Movement Input
	    Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        camForward = Vector3.Scale(cam.up, new Vector3(1, 0, 1)).normalized;
        move = moveInput.y * camForward + moveInput.x * cam.right;

        if (move.magnitude > 1)
        {
            move.Normalize();
        }

        moveVelocity = moveInput.normalized * moveSpeed;

        if (!dodging)
        {
            controller.Move(moveVelocity);

            ConvertMoveInput();
            UpdateAnimator();
        }
        else if (dodging)
        {
            i++;
            controller.Move(moveInput.normalized * dodgeSpeed);
        }

        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
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

        //Ammo
        if (!gunController.equippedGun.infinite && gunController.equippedGun.ammo <= 0)
            gunController.EquipGun(gunController.startingGun);

        //Combo 
        if(health < currentHealth) combo = 1;

        currentHealth = health;
    }

    void ConvertMoveInput()
    {
        Vector3 localMove = transform.InverseTransformDirection(moveVelocity);
        turnAmount = localMove.x;
        forwardAmount = localMove.z;
    }
    void UpdateAnimator()
    {
        anim.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "PickUp")
        {
            gunController.EquipGun(c.gameObject.GetComponent<PickUp>().gun);
            Destroy(c.gameObject);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Wall")
        {
            dodging = false;
            canDodge = false;
        }
    }
}
