using UnityEngine;
using System.Collections;
using UnityEditor;

public class LevelGenerator : MonoBehaviour {

    public Room[] normalRooms;
    public Room[] bossRooms;
    Room[,] map;

    [HideInInspector]
    public Room currentRoom;
    bool newMap, newRoom;
    int nRooms; //Número de salas
    int maxSize = 25;
    int level = 1;

    private GameObject player, cam;

    float camSpeed = 50f;

	// Use this for initialization
	void Start () 
    {
        newMap = true;
        map = new Room[maxSize, maxSize];

        player = GameObject.FindGameObjectsWithTag("Player")[0];
        cam = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if(newMap)
        {
            nRooms = level * 10;

            int x = maxSize / 2, y = maxSize / 2;
            map[x, y] = normalRooms[0]; //Primeira sala

            //Gera a disposição e tipo de salas
            for (int i = 0; i < nRooms + 1; i++)
            {
                int randomPath, newX, newY, randomRoom;

                newX = x;
                newY = y;

                if ((x <= 1 || map[x - 1, y] != null) && (x >= maxSize - 2 || map[x + 1, y] != null)) randomPath = 1; //Não pode expandir na horizontal
                else if ((y <= 1 || map[x, y - 1] != null) && (y >= maxSize - 2 || map[x, y + 1] != null)) randomPath = 0; //Não pode expandir na vertical
                else randomPath = Random.Range(0, 2); //Escolhe se expande na vertical ou horizontal

                if(randomPath == 0)
                {
                    if (x <= 1 || map[x - 1, y] != null) x++; //Não pode criar mais salas à esquerda
                    else if (x >= maxSize - 2 || map[x + 1, y] != null) x--; //Não pode criar mais salas à direita
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
                    if (y <= 1 || map[x, y - 1] != null) y++; //Não pode criar mais salas em baixo
                    else if (y >= maxSize - 2 || map[x, y + 1] != null) y--; //Não pode criar mais salas em cima
                    else 
                    {
                        while (newY == y)
                        {
                            newY = Random.Range(y - 1, y + 2); //Pode ir para qualquer um dos lados
                        }
                        y = newY;
                    }
                }

                if (i == nRooms) map[x, y] = bossRooms[level - 1]; //Sala do boss
                else
                {
                    randomRoom = Random.Range(0, normalRooms.Length); //Escolhe a sala a ser gerada
                    map[x, y] = normalRooms[randomRoom];
                }
            }

            //Instancia todas as salas na sua posição respectiva
            for (int i = 0; i < maxSize; i++)
            {
                for(int j = 0; j < maxSize; j++)
                {
                    if (map[i, j] != null)
                    {
                        Room r = Instantiate(map[i, j], new Vector3(i * map[i, j].room.GetComponent<Renderer>().bounds.size.x, -1, j * map[i, j].room.GetComponent<Renderer>().bounds.size.z), Quaternion.identity) as Room;
                        map[i, j] = r;
                    }
                }
            }

            currentRoom = map[maxSize/2, maxSize/2]; //Atribui a sala actual

            //Posiciona a camara e o jogador na primeira sala
            player.transform.position = new Vector3(maxSize/2 * map[maxSize/2, maxSize/2].room.GetComponent<Renderer>().bounds.size.x, 1, maxSize/2 * map[maxSize/2, maxSize/2].room.GetComponent<Renderer>().bounds.size.z);
            cam.transform.position = new Vector3(maxSize/2 * map[maxSize/2, maxSize/2].room.GetComponent<Renderer>().bounds.size.x, cam.transform.position.y, maxSize/2 * map[maxSize/2, maxSize/2].room.GetComponent<Renderer>().bounds.size.z - 12);

            NavMeshBuilder.BuildNavMesh();

            newMap = false; //Evita a criação de mapas a cada update
            newRoom = true;
        }

        //Actualizar a posição da camera conforme a sala
        if (cam.transform.position.x < currentRoom.transform.position.x) cam.transform.position += Vector3.right * camSpeed * Time.deltaTime;
        if (cam.transform.position.x > currentRoom.transform.position.x) cam.transform.position -= Vector3.right * camSpeed * Time.deltaTime;
        if (cam.transform.position.z < currentRoom.transform.position.z - 12) cam.transform.position += Vector3.forward * camSpeed * Time.deltaTime;
        if (cam.transform.position.z > currentRoom.transform.position.z - 12) cam.transform.position -= Vector3.forward * camSpeed * Time.deltaTime;

        //Verificar se sala já foi derrotada e abrir portas
        if (currentRoom.clear)
        {
            //Abre porta da esquerda
            if ((int)GetCurrentRoomCoordinates().x - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y] != null)
            {
                map[(int)GetCurrentRoomCoordinates().x - 1, (int)GetCurrentRoomCoordinates().y].OpenDoor(1);
                currentRoom.OpenDoor(0);
            }

            //Abre porta da direita
            if ((int)GetCurrentRoomCoordinates().x + 1 < maxSize && map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y] != null)
            {
                map[(int)GetCurrentRoomCoordinates().x + 1, (int)GetCurrentRoomCoordinates().y].OpenDoor(0);
                currentRoom.OpenDoor(1);
            }

            //Abre porta de cima
            if ((int)GetCurrentRoomCoordinates().y + 1 < maxSize && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1] != null)
            {
                map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y + 1].OpenDoor(3);
                currentRoom.OpenDoor(2);
            }

            //Abre porta de baixo
            if ((int)GetCurrentRoomCoordinates().y - 1 >= 0 && map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1] != null)
            {
                map[(int)GetCurrentRoomCoordinates().x, (int)GetCurrentRoomCoordinates().y - 1].OpenDoor(2);
                currentRoom.OpenDoor(3);
            }

            if (currentRoom.boss)
            {
                ResetMap(); //Reinicia o mapa
                //level++; //Próximo nível
                player.GetComponent<Player>().health = player.GetComponent<Player>().startingHealth; //HP do jogador fica a 100%
            }
        }
        else if (newRoom)
        {
            //Fecha todas as portas
            currentRoom.CloseDoor(0);
            currentRoom.CloseDoor(1);
            currentRoom.CloseDoor(2);
            currentRoom.CloseDoor(3);

            if (currentRoom.AllDoorsClosed())
            {
                //Activa todos os spawners
                if (currentRoom.spawners[0] != null) currentRoom.spawners[0].gameObject.SetActive(true);
                if (currentRoom.spawners[1] != null) currentRoom.spawners[1].gameObject.SetActive(true);
                if (currentRoom.spawners[2] != null) currentRoom.spawners[2].gameObject.SetActive(true);
                if (currentRoom.spawners[3] != null) currentRoom.spawners[3].gameObject.SetActive(true);
                newRoom = false;
            }
        }

        //Verificar em que sala está o jogador
        if (player != null)
        {
            for (int i = 0; i < maxSize; i++)
            {
                for (int j = 0; j < maxSize; j++)
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
	}

    private Vector2 GetCurrentRoomCoordinates()
    {
        for (int i = 0; i < maxSize; i++ )
        {
            for (int j = 0; j < maxSize; j++)
            {
                if (map[i,j] != null && currentRoom == map[i, j]) return new Vector2(i, j);
            }
        }
        return Vector2.zero;
    }

    private void ResetMap()
    {
        //Reset map array
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                if (map[i, j] != null)
                {
                    GameObject obj = GameObject.Find(map[i, j].gameObject.name);
                    foreach (Transform child in obj.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    Destroy(obj);
                    map[i, j] = null;
                }
            }
        }
        newMap = true;
    }
}
