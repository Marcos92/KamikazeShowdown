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

    private GameObject player, camera;

	// Use this for initialization
	void Start () 
    {
        newMap = true;
        map = new Room[10, 10];

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        camera = GameObject.Find("Main Camera");
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

            currentRoom = map[0, 0]; //Atribui a sala actual

            NavMeshBuilder.BuildNavMesh();

            newMap = false; //Evita a criação de mapas a cada update
            newRoom = true;
        }

        //Actualizar a posição da camera conforme a sala
        if (camera.transform.position.x < currentRoom.transform.position.x) camera.transform.position += Vector3.right * 20f * Time.deltaTime;
        if (camera.transform.position.x > currentRoom.transform.position.x) camera.transform.position -= Vector3.right * 20f * Time.deltaTime;
        if (camera.transform.position.z < currentRoom.transform.position.z) camera.transform.position += Vector3.forward * 20f * Time.deltaTime;
        if (camera.transform.position.z > currentRoom.transform.position.z) camera.transform.position -= Vector3.forward * 20f * Time.deltaTime;

        //Verificar se sala já foi derrotada e abrir portas
        if (currentRoom.clear)
        {
            //Abre porta da esquerda
            if ((int)GetCurrentRoomCoordinates().x - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y] != null && !map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y].clear)
            {
                map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y].OpenDoor(1);
                currentRoom.OpenDoor(0);
            }

            //Abre porta da direita
            if ((int)GetCurrentRoomCoordinates().x + 1 < 10 && map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y] != null && !map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y].clear)
            {
                map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y].OpenDoor(0);
                currentRoom.OpenDoor(1);
            }

            //Abre porta de cima
            if ((int)GetCurrentRoomCoordinates().y + 1 < 10 && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1] != null && !map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1].clear)
            {
                map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1].OpenDoor(3);
                currentRoom.OpenDoor(2);
            }

            //Abre porta de baixo
            if ((int)GetCurrentRoomCoordinates().y - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1] != null && !map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1].clear)
            {
                map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1].OpenDoor(2);
                currentRoom.OpenDoor(3);
            }
        }
        else if (newRoom)
        {
            //Fecha porta da direita da sala anterior
            if ((int)GetCurrentRoomCoordinates().x - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y] != null && map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y].clear)
                map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y].CloseDoor(1);

            //Fecha porta da esquerda da sala anterior
            if ((int)GetCurrentRoomCoordinates().x + 1 < 10 && map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y] != null && map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y].clear)
                map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y].CloseDoor(0);

            //Fecha porta de baixo da sala anterior
            if ((int)GetCurrentRoomCoordinates().y + 1 < 10 && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1] != null && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1].clear)
                map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1].CloseDoor(3);

            //Fecha porta de cima da sala anterior
            if ((int)GetCurrentRoomCoordinates().y - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1] != null && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1].clear)
                map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1].CloseDoor(2);

            //Fecha todas as portas
            currentRoom.CloseDoor(0);
            currentRoom.CloseDoor(1);
            currentRoom.CloseDoor(2);
            currentRoom.CloseDoor(3);

            //Activa todos os spawners
            currentRoom.spawners[0].gameObject.SetActive(true);
            currentRoom.spawners[1].gameObject.SetActive(true);
            currentRoom.spawners[2].gameObject.SetActive(true);
            currentRoom.spawners[3].gameObject.SetActive(true);

            if(currentRoom.AllDoorsClosed()) newRoom = false;
        }

        //Verificar em que sala está o jogador
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (map[i, j] != null
                    && player.transform.position.x < map[i, j].transform.position.x + map[i, j].room.GetComponent<Renderer>().bounds.size.x / 2
                    && player.transform.position.x > map[i, j].transform.position.x - map[i, j].room.GetComponent<Renderer>().bounds.size.x / 2
                    && player.transform.position.z < map[i, j].transform.position.z + map[i, j].room.GetComponent<Renderer>().bounds.size.z / 2
                    && player.transform.position.z > map[i, j].transform.position.z - map[i, j].room.GetComponent<Renderer>().bounds.size.z / 2)
                {
                    if (currentRoom != map[i, j])
                    {
                        newRoom = true;
                        currentRoom = map[i, j];
                    }
                }
            }
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
