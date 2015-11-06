using UnityEngine;
using System.Collections;

public class Saw : MonoBehaviour {

    public float speed;
    public float bounds;
    int direction;
    Transform saw;

	// Use this for initialization
	void Start ()
    {
        saw = gameObject.transform.GetChild(0);
        direction = -1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        saw.position += new Vector3(speed, 0, 0) * direction;

        if (saw.localPosition.x <= -bounds || saw.localPosition.x >= bounds) direction *= -1;
	}
}
