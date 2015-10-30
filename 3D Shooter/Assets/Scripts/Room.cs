using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour 
{
    public bool clear;

    public GameObject room;
    public Spawner topSpawner, bottomSpawner, leftSpawner, rightSpawner;

	// Use this for initialization
	void Start () 
    {
        if (topSpawner != null) topSpawner.transform.position = room.transform.position + new Vector3(0, 0, room.GetComponent<Renderer>().bounds.size.z / 2 - 5);
        else
        {
            topSpawner = new Spawner();
            topSpawner.allEnemiesDefeated = true;
        }

        if (bottomSpawner != null) bottomSpawner.transform.position = room.transform.position + new Vector3(0, 0, -room.GetComponent<Renderer>().bounds.size.z / 2 + 5);
        else
        {
            bottomSpawner = new Spawner();
            bottomSpawner.allEnemiesDefeated = true;
        }

        if (leftSpawner != null) leftSpawner.transform.position = room.transform.position + new Vector3(-room.GetComponent<Renderer>().bounds.size.x / 2 + 5, 0, 0);
        else
        {
            leftSpawner = new Spawner();
            leftSpawner.allEnemiesDefeated = true;
        }

        if (rightSpawner != null) rightSpawner.transform.position = room.transform.position + new Vector3(room.GetComponent<Renderer>().bounds.size.x / 2 - 5, 0, 0);
        else
        {
            rightSpawner = new Spawner();
            rightSpawner.allEnemiesDefeated = true;
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        //The room is clear after all enemies from all spawn points are defeated
        if (topSpawner.allEnemiesDefeated && bottomSpawner.allEnemiesDefeated && leftSpawner.allEnemiesDefeated && rightSpawner.allEnemiesDefeated)
            clear = true;

        if (clear) Debug.Log("YOU WIN");
	}
}
