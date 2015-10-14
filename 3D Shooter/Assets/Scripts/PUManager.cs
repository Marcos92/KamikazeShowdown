using UnityEngine;
using System.Collections;

public class PUManager : MonoBehaviour {

    public GameObject spreadPU;
    float nextSpawn;
    public float maxSpawnDelay, minSpawnDelay;

	// Use this for initialization
	void Start ()
    {
        nextSpawn = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay + 1);
    }
	
	// Update is called once per frame
	void Update ()
    {
	    if(Time.time > nextSpawn)
        {
            nextSpawn = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay + 1);
            PickUp newPU = Instantiate(spreadPU, Vector3.zero, Quaternion.identity) as PickUp;
        }
	}
}
