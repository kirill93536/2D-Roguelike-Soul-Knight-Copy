using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    public float gunRotationSpeed = 5f;

    public Weapon currentWeapon; // The weapon currently equipped by the player

    public LayerMask enemyLayer; // Layer mask for detecting enemies

    private Transform target; // Target of range attacks
    private Room currentRoom;
    private bool canMeleeAttack = true; // Flag to check if melee attack is available
    private bool canRangeAttack = true; // Flag to check if range attack is available
    private bool hasEnteredRoom = false; // Flag to check if player entered room
    private Player player;

    [SerializeField] private Animator animator; // Animator component for playing animations
    [SerializeField] private AudioSource audioSource; // AudioSource component for playing sounds

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        /*if (Input.GetButton("Fire2") && canMeleeAttack && currentWeapon != null)
        {

            MeleeAttack();
            canMeleeAttack = false;
            StartCoroutine(MeleeAttackDelay());
        }*/
        if (Input.GetButton("Fire1") && canRangeAttack && currentWeapon != null && player.mp > 0)
        {
            RangeAttack();
            canRangeAttack = false;
            StartCoroutine(RangeAttackDelay());
        }
    }

    void MeleeAttack()
    {
        int damage = currentWeapon.weapon.attackDamage;
        if (!hasEnteredRoom)
        {
            damage *= 0;
        }
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, currentWeapon.weapon.attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(damage);
        }
    }

    void RangeAttack()
    {
        int damage = currentWeapon.weapon.attackDamage;
        player.ReduceMP(currentWeapon.weapon.shootCost);
        if (currentRoom == null)
        {
            damage *= 0;
        }

        // Get the direction the player is facing
        float direction = Mathf.Sign(transform.localScale.x);

        // Find the closest enemy
        target = FindNearestEnemyRoom();

        if (target != null)
        {
            // Check if the nearest enemy is in the direction the player is facing


            Vector2 projectileDirection = target.position - currentWeapon.shootPoint.position;
            float angle = CalculateAngle(projectileDirection);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            currentWeapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            GameObject projectileObject = ObjectPooler.SharedInstance.GetPooledObject(currentWeapon.weapon.projectilePrefab.tag);
            if (projectileObject != null)
            {
                projectileObject.transform.position = currentWeapon.shootPoint.position;
                projectileObject.transform.rotation = rotation;
                projectileObject.SetActive(true);

                Projectile projectile = projectileObject.GetComponent<Projectile>();
                projectile.Launch(projectileDirection.normalized, currentWeapon.weapon.attackSpeed, currentWeapon.weapon.attackRange, damage, enemyLayer);
            }
        }
        else
        {
            // If there is no enemy in the correct direction, continue shooting in the previous direction
            Vector2 shootDirection = currentWeapon.shootPoint.transform.position - currentWeapon.transform.position;
            float angle = CalculateAngle(shootDirection);
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            currentWeapon.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            GameObject projectileObject = ObjectPooler.SharedInstance.GetPooledObject(currentWeapon.weapon.projectilePrefab.tag);
            if (projectileObject != null)
            {
                projectileObject.transform.position = currentWeapon.shootPoint.position;
                projectileObject.transform.rotation = rotation;
                projectileObject.SetActive(true);

                Projectile projectile = projectileObject.GetComponent<Projectile>();
                projectile.Launch(shootDirection.normalized, currentWeapon.weapon.attackSpeed, currentWeapon.weapon.attackRange, damage, enemyLayer);
            }
        }
    }

    private Transform FindNearestEnemyRoom()
    {
        if (currentRoom == null)
        {
            return FindClosestEnemy(transform.position);
        }

        List<Enemy> enemiesInRoom = currentRoom.spawnedEnemies;
        if (currentRoom.spawnedEnemies.Count == 0)
        {
            return null;
        }
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;
        foreach (Enemy enemy in enemiesInRoom)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                nearestEnemy = enemy.transform;
                minDistance = distance;
            }
        }

        return nearestEnemy;
    }

    private Transform FindClosestEnemyInDirection(Vector2 origin, float direction)
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(origin, currentWeapon.weapon.attackRange, enemyLayer);

        float closestEnemyDistance = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            if (enemyCollider.gameObject.activeSelf)
            {
                Transform enemyTransform = enemyCollider.transform;
                float distanceToEnemy = Vector2.Distance(origin, enemyTransform.position);

                // Check if the enemy is in the correct direction
                if (Mathf.Sign(enemyTransform.position.x - origin.x) != direction)
                {
                    continue;
                }

                if (distanceToEnemy < closestEnemyDistance)
                {
                    closestEnemyDistance = distanceToEnemy;
                    closestEnemy = enemyTransform;
                }
            }
        }

        return closestEnemy;
    }

    float CalculateAngle(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += 90;
        return angle;
    }

    IEnumerator MeleeAttackDelay()
    {
        yield return new WaitForSeconds(currentWeapon.weapon.attackDelay);
        canMeleeAttack = true;
    }

    IEnumerator RangeAttackDelay()
    {
        yield return new WaitForSeconds(currentWeapon.weapon.attackDelay);
        canRangeAttack = true;
    }

    Transform FindClosestEnemy(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        Transform closestEnemy = null;
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, currentWeapon.weapon.attackRange, enemyLayer);

        foreach (Collider2D enemy in enemies)
        {
            float distance = Vector2.Distance(position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy.transform;
            }
        }

        return closestEnemy;
    }

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Room>(out Room room))
        {
            currentRoom = room;
            hasEnteredRoom = true;
            room.StartRoomStage();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Room>(out Room room))
        {
            hasEnteredRoom = false;
            currentRoom = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (currentWeapon != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, currentWeapon.weapon.attackRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, currentWeapon.weapon.attackRange);

            if (target != null)
            {
                Debug.DrawRay(transform.position, target.position - transform.position, Color.red);
            }
        }
    }
}
