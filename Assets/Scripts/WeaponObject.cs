using UnityEngine;

public enum WeaponType
{
    Melee,
    Ranged
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Custom/Create New Weapon")]
public class WeaponObject : ScriptableObject
{
    public string weaponName;
    public WeaponType type;
    public float attackRange = 1f;
    public int attackDamage = 10;
    public float attackDelay = 0.5f;
    public Projectile projectilePrefab;
    public float attackSpeed = 10f;
    public int shootCost = 2;
}
