using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//norte es forward, este es right
public enum door { north, east, west, south };

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    [Header("Algorithm Settings")]
    public int maxNumberOfRooms;
    private int numberOfRooms;

    [Range(0, 100)]
    public int chanceOfSpawningRoomAtDoor;
    public bool generateCrossingPaths;
    [Range(0, 10)]
    public float chanceOfSpawningSecretRoom;
    public int maxNumberOfSecretRooms;
    private int numberOfSecretRooms;

    [Header("Room Settings")]
    public GameObject firstRoom;
    public GameObject roomPrefab;
    public Vector2 sizeOfRoom;

    [Header("Additional Configuration")]
    public GameObject portalPrefab;
    public Transform temporalAssets;

    private List<RoomController> leftToDo;
    private List<RoomController> currentLevel;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        leftToDo = new List<RoomController>();
        currentLevel = new List<RoomController>();
    }

    void Start()
    {


    }

    public void Clear()
    {
        foreach (Transform item in temporalAssets)
        {
            Destroy(item.gameObject);
        }
        foreach (Transform item in EnemiesInfo.instance.enemyParent)
        {
            Destroy(item.gameObject);
        }
        if (currentLevel.Count > 0)
        {
            foreach (var item in currentLevel)
            {
                Destroy(item.gameObject);
            }
        }
        leftToDo = new List<RoomController>();
        currentLevel = new List<RoomController>();
    }

    public void Generate()
    {
        //limpio la escena
        Clear();
        RoomController firstRoomScript = firstRoom.GetComponent<RoomController>();
        //Genero las primeras 4 habitaciones
        for (int i = 0; i < 4; i++)
        {
            Vector3 offset = Offset(i) / 2f;

            GameObject aux = Instantiate(roomPrefab, firstRoomScript.directions[i].transform.position + offset, Quaternion.identity, this.transform);
            RoomController auxScript = aux.GetComponent<RoomController>();
            auxScript.position = UpdatePosition(i);
            auxScript.ToggleDoor(SwapDirection(i), false);
            leftToDo.Add(auxScript);
            currentLevel.Add(auxScript);
        }
        numberOfRooms = 5;

        //genero hasta llegar a 20 habitaciones
        while (numberOfRooms < maxNumberOfRooms)
        {

            //el algoritmo dice que hay un 33% de posibilidades de spawnear habitacion, esto puede hacer que a veces
            //todos los puntos de generacion se agoten y no se llegue a 20 habitaciones
            if (leftToDo.Count <= 0)
            {
                leftToDo.AddRange(currentLevel);//esto tambien mete las habitaciones interiores, puede funcionar mal con generateCrossingPaths
                // Debug.Log("añadiendo todas las habitaciones de nuevo");
                // break;
            }
            int random = Random.Range(0, leftToDo.Count);

            //elijo un punto de generacion aleatorio

            RoomController currentRoom = leftToDo[random];

            for (int i = 0; i < 4; i++)
            {
                //si hay una puerta cerrada
                if (currentRoom.doors[i].activeInHierarchy)
                {
                    //hay una posibilidad de que se abra
                    if (Random.Range(0, 101) < chanceOfSpawningRoomAtDoor)
                    {
                        Vector2Int nextPos = currentRoom.position + UpdatePosition(i);
                        //compruebo si ya hay habitacion aqui, si la opcion esta marcada, genera una puerta y conecta los caminos
                        if (CheckIfAlreadyBuilt(nextPos))
                        {
                            // Debug.Log("there is a room there already");

                            if (generateCrossingPaths)
                            {
                                // Debug.Log("nueva puerta");
                                currentRoom.ToggleDoor(i, false);
                                LocateRoom(nextPos).ToggleDoor(SwapDirection(i), false);
                            }
                            continue;
                        }

                        Vector3 offset = Offset(i) / 2f;
                        GameObject aux = Instantiate(roomPrefab, currentRoom.directions[i].transform.position + offset, Quaternion.identity, this.transform);
                        RoomController generatedRoom = aux.GetComponent<RoomController>();
                        //guardo su posicion para poder usarla mas adelante para saber si la habitacion ya existe
                        generatedRoom.position = currentRoom.position + UpdatePosition(i);

                        //si la habitacion es secreta dejamos las puertas cerradas y la volvemos secreta
                        if (Random.Range(0, 100f) < chanceOfSpawningSecretRoom &&
                             (numberOfRooms != maxNumberOfRooms - 1) &&
                                 numberOfSecretRooms < maxNumberOfSecretRooms)
                        {
                            generatedRoom.TurnSecret();
                            //dejamos las puertas cerradas pero las volvemos destruibles
                            currentRoom.ToggleDoor(i, true, true);

                            //no la añado a los puntos de generacion porque es una sala aislada que no debe tener puertas abiertas
                        }
                        else
                        {

                            //abrimos puerta de la que ya estaba generada
                            currentRoom.ToggleDoor(i, false);
                            //abrimos la contraria de la nueva
                            generatedRoom.ToggleDoor(SwapDirection(i), false);
                            //añado la nueva habitacion a los posibles puntos de generacion
                            leftToDo.Add(generatedRoom);
                        }

                        //añado la habitacion a una lista global 
                        currentLevel.Add(generatedRoom);

                        //habitacion del boss
                        if (numberOfRooms == maxNumberOfRooms - 1)
                        {
                            generatedRoom.TurnBossRoom();
                            //vaya vaya, usando goto
                            goto getOut;
                            // break;
                        }
                        numberOfRooms++;
                    }
                }
            }
            //este punto de generacion se ha agotado
            leftToDo.Remove(currentRoom);
        }
    getOut:
        // Debug.Log("Done");
        //hay que generar el navmesh en runtime debido a que el suelo es procedural y no se puede hacer con obstaculos
        //tiene que haber un delay para que genere el navmesh con todo actualizado, hay cosas que el motor realiza mas tarde asi que hay que esperar al siguiente frame
        StartCoroutine(delay());
    }

    IEnumerator delay()
    {
        yield return null;
        this.GetComponent<UnityEngine.AI.NavMeshSurface>().BuildNavMesh();
    }

    private Vector3 Offset(int iterator)
    {
        switch (iterator)
        {
            case 0:
                return Vector3.forward * sizeOfRoom.y;

            case 1:
                return Vector3.right * sizeOfRoom.x;
            case 2:
                return -Vector3.right * sizeOfRoom.x;
            case 3:
                return -Vector3.forward * sizeOfRoom.y;

            default:
                return Vector3.zero;
        }
    }

    public bool CheckIfAlreadyBuilt(Vector2Int position)
    {
        bool result = false;
        foreach (var item in currentLevel)
        {
            if (item.position == position)
            {
                result = true;
            }
        }
        return result;
    }

    public RoomController LocateRoom(Vector2Int position)
    {
        foreach (var item in currentLevel)
        {
            if (item.position == position)
            {
                return item;
            }
        }
        return null;
    }

    public Vector2Int UpdatePosition(int iterator)
    {
        switch (iterator)
        {
            case 3:
                return Vector2Int.right;

            case 2:
                return Vector2Int.up;
            case 1:
                return Vector2Int.down;
            case 0:
                return Vector2Int.left;

            default:
                return Vector2Int.zero;
        }
    }
    //esta funcion es para que devuelva el contador contrario al que le mando. Por ejemplo, si spawneo una habitacion en el norte (i=0),
    //quiero abrir su puerta sur, por lo que espero que me devuelva 3
    private int SwapDirection(int iterator)
    {
        return 3 - iterator;
    }
}
