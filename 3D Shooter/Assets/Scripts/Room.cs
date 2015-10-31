using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour 
{
    public bool clear;

    public GameObject room;
    public Spawner topSpawner, bottomSpawner, leftSpawner, rightSpawner;
    Spawner[] spawners = new Spawner[4];

	// Use this for initialization
	void Start () 
    {
        if (topSpawner != null)
        {
            Spawner ts = Instantiate(topSpawner, room.transform.position + new Vector3(0, 0, room.GetComponent<Renderer>().bounds.size.z / 2 - 5), Quaternion.identity) as Spawner;
            spawners[0] = ts;
        }

        if (bottomSpawner != null)
        {
            Spawner bs = Instantiate(bottomSpawner, room.transform.position + new Vector3(0, 0, -room.GetComponent<Renderer>().bounds.size.z / 2 + 5), Quaternion.identity) as Spawner;
            spawners[1] = bs;
        }

        if (leftSpawner != null)
        {
            Spawner ls = Instantiate(leftSpawner, room.transform.position + new Vector3(-room.GetComponent<Renderer>().bounds.size.z / 2 + 5, 0, 0), Quaternion.identity) as Spawner;
            spawners[2] = ls;
        }

        if (rightSpawner != null)
        {
            Spawner rs = Instantiate(rightSpawner, room.transform.position + new Vector3(room.GetComponent<Renderer>().bounds.size.z / 2 - 5, 0, 0), Quaternion.identity) as Spawner;
            spawners[3] = rs;
        }
    }
	
	// Update is called once per frame
	void Update () 
    {
        //The room is clear after all enemies from all spawn points are defeated
        if ((topSpawner == null || spawners[0].allEnemiesDefeated)
            && (bottomSpawner == null || spawners[1].allEnemiesDefeated)
            && (leftSpawner == null || spawners[2].allEnemiesDefeated)
            && (rightSpawner == null || spawners[3].allEnemiesDefeated))
        {
            clear = true;
        }

        if (clear) Debug.Log("YOU WIN"); //TEMP
	}
}
