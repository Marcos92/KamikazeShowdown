using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

    public GameObject[] rooms;
    GameObject[,] map;
    bool newMap;

	// Use this for initialization
	void Start () 
    {
        newMap = true;
        map = new GameObject[10, 10];
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(newMap)
        {
            int x = 0, y = 0;
            map[x, y] = rooms[0];

            for (int i = 0; i < 5; i++)
            {
                int randomPath = Random.Range(0, 2);
                int randomRoom = Random.Range(0, rooms.Length);

                if (randomPath == 0 && x < 9) x++;
                else if (y < 9) y++;

                map[x, y] = rooms[randomRoom];
            }

            for (int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if (map[i, j] != null)
                    {
                        GameObject newRoom = Instantiate(map[i, j], new Vector3(i * map[i, j].transform.FindChild("Plane").GetComponent<Renderer>().bounds.size.x, -1, j * map[i, j].transform.FindChild("Plane").GetComponent<Renderer>().bounds.size.z), Quaternion.identity) as GameObject;
                    }
                }
            }

            newMap = false;
        }
	}
}
