using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject startRoomPrefab;
    public GameObject endRoomPrefab;
    public GameObject roomPrefab;
    public GameObject corridorPrefab;

    [Header("Grid Settings")]
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float roomOffset = 10f;

    [Header("Generation Settings")]
    public int totalRooms = 10;
    public int maxAdditionalInterconnections = 2;
    public float interconnectionProbability = 0.5f;
    public float loopProbability = 0.5f;
    public int maxLoops = 3;
    public bool unlimitedLoops = false;

    [Header("Expansion Bias (0-1)")]
    [Range(0f, 1f)] public float biasUp = 0.25f;
    [Range(0f, 1f)] public float biasDown = 0.25f;
    [Range(0f, 1f)] public float biasLeft = 0.25f;
    [Range(0f, 1f)] public float biasRight = 0.25f;

    [Header("Player and Enemies")]
    public Player player;
    public Enemy enemyPrefab;
    public int minEnemiesToSpawn = 1;
    public int maxEnemiesToSpawn = 3;
    public float enemySpawnOffset = 1f;
    public GameObject portalPrefab;

    private Room[,] roomGrid;
    private List<Room> spawnedRooms = new List<Room>();
    private int currentRoomCount = 0;
    private Room startRoom;
    private Room endRoom;

    void Start()
    {
        roomGrid = new Room[gridWidth, gridHeight];
        GenerateDungeon();
        player.transform.position = startRoom.transform.position;
        SpawnEnemiesInRooms();
        SpawnPortal();
    }

    void GenerateDungeon()
    {
        NormalizeBiases();

        // Step 1: Generate the Start Room
        int xStart = Random.Range(0, gridWidth);
        int yStart = Random.Range(0, gridHeight);

        Vector2 startPosition = new Vector2(xStart * roomOffset, yStart * roomOffset);
        GameObject startRoomGO = Instantiate(startRoomPrefab, startPosition, Quaternion.identity);
        startRoom = startRoomGO.GetComponent<Room>();
        startRoom.gridX = xStart;
        startRoom.gridY = yStart;
        startRoom.isStartRoom = true;

        roomGrid[xStart, yStart] = startRoom;
        spawnedRooms.Add(startRoom);
        currentRoomCount++;

        Room lastRoomPlaced = startRoom;

        while (currentRoomCount < totalRooms)
        {
            bool roomPlaced = false;

            List<Room> baseRoomsToTry = new List<Room>();

            if (currentRoomCount <= 2)
            {
                baseRoomsToTry.Add(lastRoomPlaced);
            }
            else
            {
                List<Room> possibleRooms = new List<Room>(spawnedRooms);
                possibleRooms.RemoveAll(r => r.isStartRoom);
                possibleRooms = ShuffleList(possibleRooms);

                baseRoomsToTry.AddRange(possibleRooms);
            }

            foreach (Room baseRoom in baseRoomsToTry)
            {
                int attempts = 0;
                while (attempts < 4)
                {
                    Direction direction = GetBiasedRandomDirection();
                    int xNew, yNew;
                    if (GetValidDirection(baseRoom.gridX, baseRoom.gridY, direction, out xNew, out yNew))
                    {
                        Room newRoom = InstantiateRoomAt(xNew, yNew);
                        ConnectRooms(baseRoom, newRoom, direction);
                        currentRoomCount++;
                        lastRoomPlaced = newRoom;

                        if (currentRoomCount == totalRooms)
                        {
                            TransformToEndRoom(newRoom);
                        }

                        roomPlaced = true;
                        break;
                    }
                    else
                    {
                        attempts++;
                    }
                }
                if (roomPlaced)
                {
                    break;
                }
            }

            if (!roomPlaced)
            {
                Debug.LogWarning("No valid adjacent cells found for any room. Cannot place more rooms.");
                break;
            }
        }

        AddAdditionalInterconnections();
        CreateLoops();
    }

    void NormalizeBiases()
    {
        float totalBias = biasUp + biasDown + biasLeft + biasRight;

        if (totalBias <= 0f)
        {
            biasUp = biasDown = biasLeft = biasRight = 0.25f;
            totalBias = 1f;
        }

        biasUp /= totalBias;
        biasDown /= totalBias;
        biasLeft /= totalBias;
        biasRight /= totalBias;
    }

    Direction GetBiasedRandomDirection()
    {
        float randomValue = Random.Range(0f, 1f);

        if (randomValue < biasUp)
            return Direction.Top;
        else if (randomValue < biasUp + biasDown)
            return Direction.Bottom;
        else if (randomValue < biasUp + biasDown + biasLeft)
            return Direction.Left;
        else
            return Direction.Right;
    }

    bool GetValidDirection(int x, int y, Direction direction, out int xNew, out int yNew)
    {
        xNew = x;
        yNew = y;

        switch (direction)
        {
            case Direction.Top:
                yNew += 1;
                break;
            case Direction.Bottom:
                yNew -= 1;
                break;
            case Direction.Left:
                xNew -= 1;
                break;
            case Direction.Right:
                xNew += 1;
                break;
        }

        if (xNew >= 0 && xNew < gridWidth && yNew >= 0 && yNew < gridHeight)
        {
            if (roomGrid[xNew, yNew] == null)
                return true;
        }
        return false;
    }

    Room InstantiateRoomAt(int x, int y)
    {
        Vector2 position = new Vector2(x * roomOffset, y * roomOffset);
        GameObject roomGO = Instantiate(roomPrefab, position, Quaternion.identity);
        Room room = roomGO.GetComponent<Room>();
        room.gridX = x;
        room.gridY = y;

        roomGrid[x, y] = room;
        spawnedRooms.Add(room);
        return room;
    }

    void ConnectRooms(Room roomA, Room roomB, Direction direction)
    {
        roomA.SetConnection(direction);
        roomB.SetConnection(GetOppositeDirection(direction));

        Vector2 corridorPosition = (roomA.transform.position + roomB.transform.position) / 2;
        GameObject corridorGO = Instantiate(corridorPrefab, corridorPosition, Quaternion.identity);
        Corridor corridor = corridorGO.GetComponent<Corridor>();

        if (direction == Direction.Top || direction == Direction.Bottom)
            corridor.SetOrientation(true);
        else
            corridor.SetOrientation(false);
    }

    Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top: return Direction.Bottom;
            case Direction.Bottom: return Direction.Top;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: return Direction.Top;
        }
    }

    void TransformToEndRoom(Room room)
    {
        Vector2 position = room.transform.position;
        Destroy(room.gameObject);

        GameObject endRoomGO = Instantiate(endRoomPrefab, position, Quaternion.identity);
        endRoom = endRoomGO.GetComponent<Room>();
        endRoom.gridX = room.gridX;
        endRoom.gridY = room.gridY;
        endRoom.isEndRoom = true;

        // Copy connections
        endRoom.isConnectedTop = room.isConnectedTop;
        endRoom.isConnectedBottom = room.isConnectedBottom;
        endRoom.isConnectedLeft = room.isConnectedLeft;
        endRoom.isConnectedRight = room.isConnectedRight;

        // Set doors according to connections
        if (endRoom.isConnectedTop)
            endRoom.SetConnection(Direction.Top);
        if (endRoom.isConnectedBottom)
            endRoom.SetConnection(Direction.Bottom);
        if (endRoom.isConnectedLeft)
            endRoom.SetConnection(Direction.Left);
        if (endRoom.isConnectedRight)
            endRoom.SetConnection(Direction.Right);

        roomGrid[room.gridX, room.gridY] = endRoom;
        spawnedRooms.Remove(room);
        spawnedRooms.Add(endRoom);
    }

    void AddAdditionalInterconnections()
    {
        foreach (Room room in spawnedRooms)
        {
            if (room.isStartRoom || room.isEndRoom)
                continue;

            int addedConnections = room.GetConnectionCount();

            List<Direction> directions = new List<Direction> {
                Direction.Top, Direction.Bottom, Direction.Left, Direction.Right
            };

            directions = ShuffleList(directions);

            foreach (Direction direction in directions)
            {
                if (addedConnections >= maxAdditionalInterconnections)
                    break;

                int xNew = room.gridX;
                int yNew = room.gridY;

                switch (direction)
                {
                    case Direction.Top: yNew += 1; break;
                    case Direction.Bottom: yNew -= 1; break;
                    case Direction.Left: xNew -= 1; break;
                    case Direction.Right: xNew += 1; break;
                }

                if (xNew >= 0 && xNew < gridWidth && yNew >= 0 && yNew < gridHeight)
                {
                    Room adjacentRoom = roomGrid[xNew, yNew];
                    if (adjacentRoom != null && !AreRoomsConnected(room, adjacentRoom))
                    {
                        if (room.GetConnectionCount() >= maxAdditionalInterconnections)
                            break;

                        if (adjacentRoom.GetConnectionCount() >= maxAdditionalInterconnections)
                            continue;

                        if (Random.value < interconnectionProbability)
                        {
                            ConnectRooms(room, adjacentRoom, direction);
                            addedConnections++;
                        }
                    }
                }
            }
        }
    }

    bool AreRoomsConnected(Room roomA, Room roomB)
    {
        if (roomA.gridX == roomB.gridX)
        {
            if (roomA.gridY + 1 == roomB.gridY)
                return roomA.isConnectedTop && roomB.isConnectedBottom;
            if (roomA.gridY - 1 == roomB.gridY)
                return roomA.isConnectedBottom && roomB.isConnectedTop;
        }
        else if (roomA.gridY == roomB.gridY)
        {
            if (roomA.gridX + 1 == roomB.gridX)
                return roomA.isConnectedRight && roomB.isConnectedLeft;
            if (roomA.gridX - 1 == roomB.gridX)
                return roomA.isConnectedLeft && roomB.isConnectedRight;
        }
        return false;
    }

    void CreateLoops()
    {
        List<Room[]> potentialLoops = new List<Room[]>();

        for (int x = 0; x < gridWidth - 1; x++)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                Room roomA = roomGrid[x, y];
                Room roomB = roomGrid[x + 1, y];
                Room roomC = roomGrid[x, y + 1];
                Room roomD = roomGrid[x + 1, y + 1];

                if (roomA != null && roomB != null && roomC != null && roomD != null)
                {
                    potentialLoops.Add(new Room[] { roomA, roomB, roomC, roomD });
                }
            }
        }

        int loopsCreated = 0;
        foreach (Room[] loopRooms in potentialLoops)
        {
            if (!unlimitedLoops && loopsCreated >= maxLoops)
                break;

            if (Random.value < loopProbability)
            {
                if (!AreRoomsConnected(loopRooms[0], loopRooms[1]))
                {
                    if (CanAddConnection(loopRooms[0], loopRooms[1]))
                        ConnectRooms(loopRooms[0], loopRooms[1], Direction.Right);
                }

                if (!AreRoomsConnected(loopRooms[1], loopRooms[3]))
                {
                    if (CanAddConnection(loopRooms[1], loopRooms[3]))
                        ConnectRooms(loopRooms[1], loopRooms[3], Direction.Top);
                }

                if (!AreRoomsConnected(loopRooms[3], loopRooms[2]))
                {
                    if (CanAddConnection(loopRooms[3], loopRooms[2]))
                        ConnectRooms(loopRooms[3], loopRooms[2], Direction.Left);
                }

                if (!AreRoomsConnected(loopRooms[2], loopRooms[0]))
                {
                    if (CanAddConnection(loopRooms[2], loopRooms[0]))
                        ConnectRooms(loopRooms[2], loopRooms[0], Direction.Bottom);
                }

                loopsCreated++;
            }
        }
    }

    bool CanAddConnection(Room roomA, Room roomB)
    {
        if (roomA.isStartRoom || roomA.isEndRoom || roomB.isStartRoom || roomB.isEndRoom)
            return false;

        if (roomA.GetConnectionCount() >= maxAdditionalInterconnections)
            return false;

        if (roomB.GetConnectionCount() >= maxAdditionalInterconnections)
            return false;

        return true;
    }

    List<T> ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    void SpawnEnemiesInRooms()
    {
        foreach (Room room in spawnedRooms)
        {
            if (!room.isStartRoom && !room.isEndRoom)
            {
                room.SpawnEnemies(enemyPrefab, minEnemiesToSpawn, maxEnemiesToSpawn, enemySpawnOffset);
            }
        }
    }

    void SpawnPortal()
    {
        GameObject portal = Instantiate(portalPrefab, endRoom.transform);
        portal.transform.localScale = new Vector3(
            portalPrefab.transform.localScale.x / endRoom.transform.localScale.x,
            portalPrefab.transform.localScale.y / endRoom.transform.localScale.y,
            portalPrefab.transform.localScale.z / endRoom.transform.localScale.z
        );

        foreach (Enemy enemy in endRoom.spawnedEnemies)
        {
            Destroy(enemy.gameObject);
        }
        endRoom.spawnedEnemies.Clear();
    }
}
