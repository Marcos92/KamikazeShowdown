using UnityEngine;
using System.Collections;

public class CameraLookAt : MonoBehaviour {

    public float sensitivityX = 15F;
    public float sensitivityY = 15F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    float rotationX = 0F;
    float rotationY = 0F;
    float translateX = 0f;
    float translateY = 0f;
    float translateZ = 0f;
    Quaternion originalRotation;
    Vector3 originalPosition;

    public Transform player;

    void Start()
    {
        originalRotation = transform.localRotation;
        originalPosition = transform.position;
    }
    void Update()
    {
        // Read the mouse input axis
        rotationX += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime * 0.05f;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime * 0.1f;

        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);
        transform.localRotation = originalRotation * xQuaternion * yQuaternion;

        Mathf.Clamp(rotationX, -10f, 10);
    }
}
