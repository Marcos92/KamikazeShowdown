using UnityEngine;
using System.Collections;

public class Saw : MonoBehaviour {

    public float speed;
    public float bounds;
    public float damage;
    public bool vertical;
    int direction;

	// Use this for initialization
	void Start ()
    {
        direction = Random.Range(-1,1);
        if (direction == 0) direction = 1;
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.localPosition += new Vector3(speed, 0, 0) * direction;
        if (transform.localPosition.x <= -bounds || transform.localPosition.x >= bounds) direction *= -1;

        transform.Rotate(0, -10f, 0);
	}

    void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.tag == "Player")
        {
            c.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
        }
    }
}
