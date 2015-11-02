using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelGenerator : MonoBehaviour {

    public Room[] allRooms;
    Room[,] map;

    [HideInInspector]
    public Room currentRoom;
    bool newMap, newRoom;
    public int nRooms; //Número de salas 

	// Use this for initialization
	void Start () 
    {
        newMap = true;
        newRoom = false;
        map = new Room[10, 10];
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(newMap)
        {
            int x = 0, y = 0;
            map[x, y] = allRooms[0]; //Primeira sala

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

                randomRoom = Random.Range(0, allRooms.Length); //Escolhe a sala a ser gerada
                map[x, y] = allRooms[randomRoom];
            }

            //Instancia todas as salas na sua posição respectiva
            for (int i = 0; i < 10; i++)
            {
                for(int j = 0; j < 10; j++)
                {
                    if (map[i, j] != null)
                    {
                        Room r = Instantiate(map[i, j], new Vector3(i * map[i, j].room.GetComponent<Renderer>().bounds.size.x, -1, j * map[i, j].room.GetComponent<Renderer>().bounds.size.z), Quaternion.identity) as Room;
                        map[i, j] = r;
                    }
                }
            }

            currentRoom = map[0, 0]; //Atribui o nível actual

            NavMeshBuilder.BuildNavMesh();
            
            newMap = false; //Evita a criação de mapas a cada update
            newRoom = true;
        }

        //Iniciar spawners da sala actual
        if(newRoom)
        {
            for (int k = 0; k < 4; k++)
            {
                currentRoom.spawners[k].gameObject.SetActive(true);
                newRoom = false;
            }
        }

        //Verificar se sala já foi derrotada e abrir portas
        if(currentRoom.clear)
        {
            //Abre porta da esquerda
            if ((int)GetCurrentRoomCoordinates().x - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y] != null)
                currentRoom.OpenDoor(0);

            //Abre porta da direita
            if ((int)GetCurrentRoomCoordinates().x + 1 < 10 && map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y] != null)
                currentRoom.OpenDoor(1);

            //Abre porta de cima
            if ((int)GetCurrentRoomCoordinates().y + 1 < 10 && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1] != null)
                currentRoom.OpenDoor(2);

            //Abre porta de baixo
            if ((int)GetCurrentRoomCoordinates().y - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1] != null)
                currentRoom.OpenDoor(3);
        }
	}

    private Vector2 GetCurrentRoomCoordinates()
    {
        for (int i = 0; i < 10; i++ )
        {
            for (int j = 0; j < 10; j++)
            {
                if (map[i,j] != null && currentRoom == map[i, j]) return new Vector2(i, j);
            }
        }

        return Vector2.zero;
    }
}
