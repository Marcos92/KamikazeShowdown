using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelGenerator : MonoBehaviour {

    public GameObject[] rooms;
    GameObject[,] map;
    [HideInInspector]
    public GameObject currentRoom;
    bool newMap, newRoom;
    int nRooms;

	// Use this for initialization
	void Start () 
    {
        newMap = true;
        newRoom = true;
        map = new GameObject[10, 10];
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(newMap)
        {
            int x = 0, y = 0;
            map[x, y] = rooms[0]; //Primeira sala
            currentRoom = rooms[0]; //Atribui o nível actual
            nRooms = 10; //Número de salas no nível

            //Gera a disposição e tipo de salas
            for (int i = 0; i < nRooms; i++)
            {
                int randomPath, newX, newY, randomRoom;

                newX = x;
                newY = y;

                if ((x <= 0 || map[x - 1, y] != null) && (x >= 9 || map[x + 1, y] != null)) randomPath = 1; //Não pode expandir na horizontal
                else if ((y <= 0 || map[x, y - 1] != null) && (y >= 9 || map[x, y + 1] != null)) randomPath = 0; //Não pode expandir na vertical
                else randomPath = Random.Range(0, 2); //Escolhe se expande na vertical ou horizontal

                if(randomPath == 0)
                {
                    if (x <= 0 || map[x - 1, y] != null) x++; //Não pode criar mais salas à esquerda
                    else if (x >= 9 || map[x + 1, y] != null) x--; //Não pode criar mais salas à direita
                    else
                    {
                        while (newX == x)
                        {
                            newX = Random.Range(x - 1, x + 2); //Pode ir para qualquer um dos lados
                        }
                        x = newX;
                    }
                }
                else
                {
                    if (y <= 0 || map[x, y - 1] != null) y++; //Não pode criar mais salas em baixo
                    else if (y >= 9 || map[x, y + 1] != null) y--; //Não pode criar mais salas em cima
                    else 
                    {
                        while (newY == y)
                        {
                            newY = Random.Range(y - 1, y + 2); //Pode ir para qualquer um dos lados
                        }
                        y = newY;
                    }
                }

                randomRoom = Random.Range(0, rooms.Length); //Escolhe a sala a ser gerada
                map[x, y] = rooms[randomRoom];
            }

            //Instancia todas as salas na sua posição respectiva
            for (int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if (map[i, j] != null)
                    {
                        GameObject r = Instantiate(map[i, j], new Vector3(i * map[i, j].transform.FindChild("Plane").GetComponent<Renderer>().bounds.size.x, -1, j * map[i, j].transform.FindChild("Plane").GetComponent<Renderer>().bounds.size.z), Quaternion.identity) as GameObject;
                    }
                }
            }

            newMap = false; //Evita a criação de mapas a cada update

            if (newRoom)
            {
                //Mudar currentRoom
                NavMeshBuilder.BuildNavMesh();
                newRoom = false;
            }
        }
	}
}
