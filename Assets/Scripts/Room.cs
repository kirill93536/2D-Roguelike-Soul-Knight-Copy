using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2 currentPosition;
    public Room top;
    public Room bottom;
    public Room left;
    public Room right;
    public GameObject topDoor;
    public GameObject bottomDoor;
    public GameObject leftDoor;
    public GameObject rightDoor;

    public List<Enemy> spawnedEnemies = new List<Enemy>();

    public void SetRoomPositions(Vector2 position)
    {
        currentPosition = position;
        transform.position = currentPosition;
    }

    public void SetDoors(bool top, bool bottom, bool left, bool right)
    {
        topDoor.SetActive(top);
        bottomDoor.SetActive(bottom);
        leftDoor.SetActive(left);
        rightDoor.SetActive(right);
    }

    public void UnlockPreviousDoors()
    {
        if (top != null)
        {
            topDoor.SetActive(false);
        }
        if (bottom != null)
        {
            bottomDoor.SetActive(false);
        }
        if (left != null)
        {
            leftDoor.SetActive(false);
        }
        if (right != null)
        {
            rightDoor.SetActive(false);
        }
    }

    public void StartRoomStage()
    {
        if (spawnedEnemies.Count > 0)
        {
            LockAllDoors();
            StartEnemyAttack();

        }
    }

    private void StartEnemyAttack()
    {
        for(int i = 0; i < spawnedEnemies.Count; i++)
        {
            spawnedEnemies[i].playerInRoom = true;
        }
    }

    private void LockAllDoors()
    {
        StartCoroutine(LockAllDoorsDelayed());
    }

    private IEnumerator LockAllDoorsDelayed()
    {
        yield return new WaitForSeconds(0.25f);
        SetDoors(true, true, true, true);
    }

    public void SpawnEnemies(Enemy enemyPrefab, int minEnemies, int maxEnemies, float offset)
    {
        int numEnemies = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < numEnemies; i++)
        {
            //enemyPrefab.transform.localScale / transform.localScale
            Enemy newEnemy = Instantiate(enemyPrefab, transform);
            newEnemy.transform.position = GetRandomPosition(offset);
            newEnemy.transform.localScale = new Vector3(enemyPrefab.transform.localScale.x / transform.localScale.x,
                                                        enemyPrefab.transform.localScale.y / transform.localScale.y,
                                                        enemyPrefab.transform.localScale.z / transform.localScale.z);
            spawnedEnemies.Add(newEnemy);
            newEnemy.onDie.AddListener(() =>
            {
                spawnedEnemies.Remove(newEnemy);
                TriggerDoorsCheck();
            });
        }
    }

    public void TriggerDoorsCheck()
    {
        if(spawnedEnemies.Count == 0)
        {
            UnlockPreviousDoors();
        }
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

        // Check if the position is too close to another enemy
        foreach (Enemy enemy in spawnedEnemies)
        {
            if (Vector2.Distance(randomPos, enemy.transform.position) < 2f)
            {
                // If too close, generate a new position
                return GetRandomPosition(offset);
            }
        }

        return randomPos;
    }

}
