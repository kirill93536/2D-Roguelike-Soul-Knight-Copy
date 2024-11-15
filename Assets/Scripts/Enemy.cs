using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ProjectilePooler))]
public class Enemy : Entity
{
    public EnemyStats stats;
    public bool playerInRoom;
    public Player player;

    private ProjectilePooler projectilePooler;
    private bool canAttack;

    private void Awake()
    {
        hp = stats.hp;
        mp = stats.mp;
    }

    private void Start()
    {
        canAttack = true;
        player = FindObjectOfType<Player>();
        projectilePooler = GetComponent<ProjectilePooler>();

        // Subscribe to onDie event
        onDie.AddListener(OnEnemyDeath);
    }

    private void Update()
    {
        if (canAttack && playerInRoom)
        {
            Attack();
            canAttack = false;
            StartCoroutine(AttackDelay());
        }
    }

    public void Attack()
    {
        // Get a projectile from the pooler
        GameObject projectileObject = projectilePooler.GetPooledObject();
        if (projectileObject != null)
        {
            projectileObject.SetActive(true);
            // Set the projectile's position to the enemy's position
            projectileObject.transform.position = transform.position;

            // Calculate the direction towards the player
            Vector2 direction = (player.transform.position - transform.position).normalized;

            Projectile projectile = projectileObject.GetComponent<Projectile>();

            // Launch the projectile towards the player
            projectile.Launch(direction, stats.attackSpeed, 25f, stats.attackDamage, projectile.playerLayer);
        }
    }

    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(stats.attackDelay);
        canAttack = true;
    }

    // Method called when enemy dies
    private void OnEnemyDeath()
    {
        // Add 12 mana to the player

        int manaToAdd = Random.Range(1, 5);
        player.AddMana(manaToAdd);
    }
}
