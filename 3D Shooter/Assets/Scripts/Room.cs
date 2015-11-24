using UnityEngine;
using System.Collections;

public class Room : MonoBehaviour 
{
    [HideInInspector]
    public bool clear;

    public GameObject room;
    public Spawner topSpawner, bottomSpawner, leftSpawner, rightSpawner;
    public GameObject door;

    [HideInInspector]
    public Spawner[] spawners = new Spawner[4];
    GameObject[] doors = new GameObject[4];

    public bool boss;

	// Use this for initialization
	void Start () 
    {
        if (topSpawner != null)
        {
            spawners[0] = Instantiate(topSpawner, room.transform.position + new Vector3(0, 0, room.GetComponent<Renderer>().bounds.size.z / 2 - 2), Quaternion.identity) as Spawner;
            spawners[0].gameObject.SetActive(false);
        }

        if (bottomSpawner != null)
        {
            spawners[1] = Instantiate(bottomSpawner, room.transform.position + new Vector3(0, 0, -room.GetComponent<Renderer>().bounds.size.z / 2 + 2), Quaternion.identity) as Spawner;
            spawners[1].gameObject.SetActive(false);
        }

        if (leftSpawner != null)
        {
            spawners[2] = Instantiate(leftSpawner, room.transform.position + new Vector3(-room.GetComponent<Renderer>().bounds.size.z / 2 + 2, 0, 0), Quaternion.identity) as Spawner;
            spawners[2].gameObject.SetActive(false);
        }

        if (rightSpawner != null)
        {
            spawners[3] = Instantiate(rightSpawner, room.transform.position + new Vector3(room.GetComponent<Renderer>().bounds.size.z / 2 - 2, 0, 0), Quaternion.identity) as Spawner;
            spawners[3].gameObject.SetActive(false);
        }

        doors[0] = Instantiate(door, room.transform.position + new Vector3(-32, 4, 0), Quaternion.identity) as GameObject; //Porta da esquerda
        doors[1] = Instantiate(door, room.transform.position + new Vector3(32, 4, 0), Quaternion.identity) as GameObject; //Porta da direita
        doors[2] = Instantiate(door, room.transform.position + new Vector3(0, 4, 18), Quaternion.identity) as GameObject; //Porta de cima
        doors[2].transform.Rotate(0, 90, 0);
        doors[3] = Instantiate(door, room.transform.position + new Vector3(0, 4, -18), Quaternion.identity) as GameObject; //Porta de baixo
        doors[3].transform.Rotate(0, 90, 0);
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
	}

    public void OpenDoor(int d)
    {
        if (doors[d].transform.position.y < 8) 
            doors[d].transform.position += Vector3.up * 2.5f * Time.deltaTime;
    }

    public void CloseDoor(int d)
    {
        if (doors[d].transform.position.y > 4)
            doors[d].transform.position -= Vector3.up * 2.5f * Time.deltaTime;
    }

    public bool AllDoorsClosed()
    {
        if (doors[0].transform.position.y <= 4
            && doors[1].transform.position.y <= 4
            && doors[2].transform.position.y <= 4
            && doors[3].transform.position.y <= 4)
        {
            return true;
        }
        else return false;
    }
}
