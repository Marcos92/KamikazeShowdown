using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Rigidbody))]
public class PlayerController : MonoBehaviour {

    Vector3 velocity;
    Rigidbody myRigidbody;

	void Start () {
        myRigidbody = GetComponent<Rigidbody>();
	}

    public void Move(Vector3 _velocity)
    {
        if (_velocity.magnitude > 1)
        {
            _velocity.Normalize();
        }
        velocity = _velocity;
    }

    public void LookAt(Vector3 lookAtPoint)
    {
        Vector3 heightCorrectedPoint = new Vector3(lookAtPoint.x, transform.position.y, lookAtPoint.z);
        transform.LookAt(heightCorrectedPoint);
    }

    void FixedUpdate()
    {
        myRigidbody.MovePosition(myRigidbody.position + velocity * Time.deltaTime);
    }
}
