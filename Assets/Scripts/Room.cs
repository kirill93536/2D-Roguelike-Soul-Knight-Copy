using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
    // Grid position of the room
    public int gridX;
    public int gridY;

    // Flags to indicate connections
    public bool isConnectedTop = false;
    public bool isConnectedBottom = false;
    public bool isConnectedLeft = false;
    public bool isConnectedRight = false;

    // Room type
    public bool isStartRoom = false;
    public bool isEndRoom = false;

    // Doors
    public GameObject topDoor;
    public GameObject bottomDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;

    // Enemies in the room
    public List<Enemy> spawnedEnemies = new List<Enemy>();

    // New variables for coins range
    public int minCoins = 10;
    public int maxCoins = 50;

    // Reference to the player
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void SetConnection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Top:
                isConnectedTop = true;
                if (topDoor != null) topDoor.SetActive(false);
                break;
            case Direction.Bottom:
                isConnectedBottom = true;
                if (bottomDoor != null) bottomDoor.SetActive(false);
                break;
            case Direction.Left:
                isConnectedLeft = true;
                if (leftDoor != null) leftDoor.SetActive(false);
                break;
            case Direction.Right:
                isConnectedRight = true;
                if (rightDoor != null) rightDoor.SetActive(false);
                break;
        }
    }

    public int GetConnectionCount()
    {
        int count = 0;
        if (isConnectedTop) count++;
        if (isConnectedBottom) count++;
        if (isConnectedLeft) count++;
        if (isConnectedRight) count++;
        return count;
    }

    public void SpawnEnemies(Enemy enemyPrefab, int minEnemies, int maxEnemies, float offset)
    {
        int numEnemies = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            Enemy newEnemy = Instantiate(enemyPrefab, transform);
            newEnemy.transform.position = GetRandomPosition(offset);
            newEnemy.transform.localScale = new Vector3(
                enemyPrefab.transform.localScale.x / transform.localScale.x,
                enemyPrefab.transform.localScale.y / transform.localScale.y,
                enemyPrefab.transform.localScale.z / transform.localScale.z
            );
            spawnedEnemies.Add(newEnemy);
            newEnemy.onDie.AddListener(() =>
            {
                spawnedEnemies.Remove(newEnemy);
                TriggerDoorsCheck();

                // Additional check if this is the end room
                if (spawnedEnemies.Count == 0 && isEndRoom)
                {
                    CompleteLevel();
                }
            });
        }
    }

    public void StartRoomStage()
    {
        if (spawnedEnemies.Count > 0)
        {
            LockAllDoors();
            StartEnemyAttack();
        }
        else
        {
            // If no enemies, directly complete the level if it's the end room
            if (isEndRoom)
            {
                CompleteLevel();
            }
        }
    }

    private void StartEnemyAttack()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            enemy.playerInRoom = true;
        }
    }

    private void LockAllDoors()
    {
        StartCoroutine(LockAllDoorsDelayed());
    }

    private IEnumerator LockAllDoorsDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        SetAllDoors(true);
    }

    public void TriggerDoorsCheck()
    {
        if (spawnedEnemies.Count == 0)
        {
            UnlockPreviousDoors();

            // If this is the end room, complete the level
            if (isEndRoom)
            {
                CompleteLevel();
            }
        }
    }

    public void SetAllDoors(bool state)
    {
        if (topDoor != null) topDoor.SetActive(state);
        if (bottomDoor != null) bottomDoor.SetActive(state);
        if (leftDoor != null) leftDoor.SetActive(state);
        if (rightDoor != null) rightDoor.SetActive(state);
    }

    public void UnlockPreviousDoors()
    {
        if (isConnectedTop && topDoor != null) topDoor.SetActive(false);
        if (isConnectedBottom && bottomDoor != null) bottomDoor.SetActive(false);
        if (isConnectedLeft && leftDoor != null) leftDoor.SetActive(false);
        if (isConnectedRight && rightDoor != null) rightDoor.SetActive(false);
    }

    private Vector2 GetRandomPosition(float offset)
    {
        Collider2D roomCollider = GetComponent<Collider2D>();

        float deltaX = roomCollider.bounds.extents.x;
        float deltaY = roomCollider.bounds.extents.y;
        float minX = transform.position.x - deltaX + offset;
        float maxX = transform.position.x + deltaX - offset;
        float minY = transform.position.y - deltaY + offset;
        float maxY = transform.position.y + deltaY - offset;

        Vector2 randomPos = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

        foreach (Enemy enemy in spawnedEnemies)
        {
            if (Vector2.Distance(randomPos, enemy.transform.position) < 2f)
            {
                return GetRandomPosition(offset);
            }
        }

        return randomPos;
    }

    // New method to complete the level
    private void CompleteLevel()
    {
        // Add 20 mana to the player
        player.AddMana(20);

        // Generate random coins within the specified range
        int coinsAwarded = Random.Range(minCoins, maxCoins + 1);
        player.AddCoins(coinsAwarded);

        // Optionally, you can display a level completion UI or trigger other events here
        Debug.Log("Level Completed! Mana +20 and Coins +" + coinsAwarded);
    }
}
