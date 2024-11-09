using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int roomCount;
    public float roomOffset;
    public float corridorOffset;
    public Room roomPrefab;
    public GameObject corridorPrefab;
    public Vector2 spawnPosition;
    public Player player;

    public Enemy enemyPrefab;
    public float offset;
    public int minEnemiesToSpawn;
    public int maxEnemiesToSpawn;

    public GameObject portalPrefab;

    public List<Room> rooms;
    public List<GameObject> spawnedCorridors = new List<GameObject>();
    public Vector2[] directions = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
    public HashSet<Vector2> usedPositions = new HashSet<Vector2>();

    private void Start()
    {
        CreateDungeon();
        ConnectRooms(rooms, roomOffset);
        TurnOffDoors(rooms);
        CorridorSpawn(rooms, corridorOffset, corridorPrefab);
        player.transform.position = rooms[0].transform.position;
        SpawnEnemiesInRooms();
        SpawnPortal();
    }

    public void ConnectRooms(List<Room> allRooms, float offset)
    {
        for (int i = 0; i < allRooms.Count; i++)
        {
            for (int j = 0; j < allRooms.Count; j++)
            {
                if (i != j)
                {
                    Room room1 = allRooms[i];
                    Room room2 = allRooms[j];
                    Vector2 room1Pos = room1.currentPosition;
                    Vector2 room2Pos = room2.currentPosition;

                    float distance = Vector2.Distance(room1Pos, room2Pos);
                    if (distance == offset)
                    {
                        if (room2Pos.x > room1Pos.x)
                        {
                            room1.right = room2;
                            room2.left = room1;
                        }
                        else if (room2Pos.x < room1Pos.x)
                        {
                            room1.left = room2;
                            room2.right = room1;
                        }
                        else if (room2Pos.y > room1Pos.y)
                        {
                            room1.top = room2;
                            room2.bottom = room1;
                        }
                        else if (room2Pos.y < room1Pos.y)
                        {
                            room1.bottom = room2;
                            room2.top = room1;
                        }
                    }
                }
            }
        }
    }

    public void TurnOffDoors(List<Room> allRooms)
    {
        for (int i = 0; i < allRooms.Count; i++)
        {
            Room room = allRooms[i];
            if (room.top != null)
            {
                room.topDoor.SetActive(false);
            }
            if (room.bottom != null)
            {
                room.bottomDoor.SetActive(false);
            }
            if (room.left != null)
            {
                room.leftDoor.SetActive(false);
            }
            if (room.right != null)
            {
                room.rightDoor.SetActive(false);
            }
        }
    }

    public void CorridorSpawn(List<Room> rooms, float offset, GameObject corridorPrefab)
    {
        List<Vector2> corridorPositions = new List<Vector2>();

        foreach (Room currentRoom in rooms)
        {
            if (currentRoom.right != null)
            {
                Vector2 corridorPos = currentRoom.currentPosition + new Vector2(offset, 0);
                if (!corridorPositions.Contains(corridorPos))
                {
                    corridorPositions.Add(corridorPos);
                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    GameObject corridor = Instantiate(corridorPrefab, corridorPos, rotation);
                    corridor.transform.parent = currentRoom.transform;
                }
            }
            if (currentRoom.left != null)
            {
                Vector2 corridorPos = currentRoom.currentPosition + new Vector2(-offset, 0);
                if (!corridorPositions.Contains(corridorPos))
                {
                    corridorPositions.Add(corridorPos);
                    Quaternion rotation = Quaternion.Euler(0, 0, 0);
                    GameObject corridor = Instantiate(corridorPrefab, corridorPos, rotation);
                    corridor.transform.parent = currentRoom.transform;
                }
            }
            if (currentRoom.top != null)
            {
                Vector2 corridorPos = currentRoom.currentPosition + new Vector2(0, offset);
                if (!corridorPositions.Contains(corridorPos))
                {
                    corridorPositions.Add(corridorPos);
                    Quaternion rotation = Quaternion.Euler(0, 0, 90);
                    GameObject corridor = Instantiate(corridorPrefab, corridorPos, rotation);
                    corridor.transform.parent = currentRoom.transform;
                }
            }
            if (currentRoom.bottom != null)
            {
                Vector2 corridorPos = currentRoom.currentPosition + new Vector2(0, -offset);
                if (!corridorPositions.Contains(corridorPos))
                {
                    corridorPositions.Add(corridorPos);
                    Quaternion rotation = Quaternion.Euler(0, 0, 90);
                    GameObject corridor = Instantiate(corridorPrefab, corridorPos, rotation);
                    corridor.transform.parent = currentRoom.transform;
                }
            }
        }
    }


    public void CreateDungeon()
    {
        rooms = new List<Room>();

        GameObject dungeonHolder = new GameObject("Dungeon");
        dungeonHolder.transform.position = Vector2.zero;

        // Create initial room
        Room initialRoom = Instantiate(roomPrefab);
        initialRoom.SetRoomPositions(Vector2.zero);
        initialRoom.transform.parent = dungeonHolder.transform;
        usedPositions.Add(initialRoom.currentPosition);
        rooms.Add(initialRoom);

        // Generate remaining rooms
        for (int i = 1; i < roomCount; i++)
        {
            Room previousRoom = rooms[i - 1];
            Vector2 nextPosition = previousRoom.currentPosition + directions[Random.Range(0, directions.Length)] * roomOffset;

            if (!usedPositions.Contains(nextPosition))
            {
                Room newRoom = Instantiate(roomPrefab);
                newRoom.name += i.ToString();
                newRoom.SetRoomPositions(nextPosition);
                newRoom.transform.parent = dungeonHolder.transform;

                // Set references to other rooms
                if (nextPosition.y > previousRoom.currentPosition.y)
                {
                    newRoom.bottom = previousRoom;
                    previousRoom.top = newRoom;
                }
                else if (nextPosition.y < previousRoom.currentPosition.y)
                {
                    newRoom.top = previousRoom;
                    previousRoom.bottom = newRoom;
                }
                else if (nextPosition.x > previousRoom.currentPosition.x)
                {
                    newRoom.left = previousRoom;
                    previousRoom.right = newRoom;
                }
                else if (nextPosition.x < previousRoom.currentPosition.x)
                {
                    newRoom.right = previousRoom;
                    previousRoom.left = newRoom;
                }

                usedPositions.Add(nextPosition);
                rooms.Add(newRoom);
            }
            else
            {
                i--;
            }
        }
    }

    public void SpawnEnemiesInRooms()
    {
        if (rooms.Count > 1)
        {
            for (int i = 1; i < rooms.Count; i++) // Start at index 1 to skip the first room
            {
                Room room = rooms[i];
                room.SpawnEnemies(enemyPrefab, minEnemiesToSpawn, maxEnemiesToSpawn, offset);
            }
        }
    }

    public void SpawnPortal()
    {
        Room room = rooms[rooms.Count - 1];

        GameObject portal = Instantiate(portalPrefab, room.transform);
        portal.transform.localScale = new Vector3(portalPrefab.transform.localScale.x / rooms[rooms.Count - 1].transform.localScale.x,
                                           portalPrefab.transform.localScale.y / rooms[rooms.Count - 1].transform.localScale.y,
                                           portalPrefab.transform.localScale.z / rooms[rooms.Count - 1].transform.localScale.z);
        
        for(int i = 0; i < room.spawnedEnemies.Count; i++)
        {
            Destroy(room.spawnedEnemies[i].gameObject);
        }
        room.spawnedEnemies.Clear();
    }
}