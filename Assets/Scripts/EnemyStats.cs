using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "Custom/Create new Enemy")]
public class EnemyStats : ScriptableObject
{
    public int hp = 100;
    public int mp = 50;
    public float attackSpeed = 1.5f;
    public int attackDamage = 10;
    public Projectile projectilePrefab;
    public float attackDelay = 0.5f;
}
