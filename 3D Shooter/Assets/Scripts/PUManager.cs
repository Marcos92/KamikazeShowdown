using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PUManager : MonoBehaviour {

    public GameObject[] pickUpList;
    float nextSpawn;
    public float maxSpawnDelay, minSpawnDelay;
    Room currentRoom;

	// Use this for initialization
	void Start ()
    {
        nextSpawn = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay + 1);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //METER A FAZER SPAWN NA NAV MESH

        if (Time.time > nextSpawn && !currentRoom.clear)
        {
            nextSpawn = Time.time + Random.Range(minSpawnDelay, maxSpawnDelay + 1);

            float randomX = Random.Range(currentRoom.room.transform.position.x - currentRoom.room.GetComponent<Renderer>().bounds.size.x / 2.5f, currentRoom.room.transform.position.x + currentRoom.room.GetComponent<Renderer>().bounds.size.x / 2.5f);
            float randomZ = Random.Range(currentRoom.room.transform.position.z - currentRoom.room.GetComponent<Renderer>().bounds.size.z / 2.5f, currentRoom.room.transform.position.z + currentRoom.room.GetComponent<Renderer>().bounds.size.z / 2.5f);

            PickUp newPU = Instantiate(pickUpList[Random.Range(0, pickUpList.Length)], new Vector3(randomX, 0, randomZ), Quaternion.identity) as PickUp;
        }

        if (gameObject.GetComponent<LevelGenerator>().currentRoom != null) currentRoom = gameObject.GetComponent<LevelGenerator>().currentRoom;
	}
}
