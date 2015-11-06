using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour {

    public float speedX, speedZ;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerStay(Collider c)
    {
        if(c.gameObject.tag == "Player" || c.gameObject.tag == "Enemy")
        {
            c.gameObject.transform.position += new Vector3(speedX, 0, speedZ);
        }
    }
}
