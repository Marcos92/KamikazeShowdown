using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PUManager : MonoBehaviour {

    public GameObject[] pickUpList;
    float nextSpawn;
    public float maxSpawnDelay, minSpawnDelay;
    public GameObject room;

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

            float randomX = Random.Range(room.transform.position.x - room.GetComponent<Renderer>().bounds.size.x / 2.5f, room.transform.position.x + room.GetComponent<Renderer>().bounds.size.x / 2.5f);
            float randomZ = Random.Range(room.transform.position.z - room.GetComponent<Renderer>().bounds.size.z / 2.5f, room.transform.position.z + room.GetComponent<Renderer>().bounds.size.z / 2.5f);

            PickUp newPU = Instantiate(pickUpList[Random.Range(0, pickUpList.Length)], new Vector3(randomX, 0, randomZ), Quaternion.identity) as PickUp;
        }
	}
}
