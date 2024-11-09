using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private float pickupRange = 1f; // How far the player can pick up items
    [SerializeField] private LayerMask itemLayer; // The layer that items are on
    private List<Pickupable> inventory = new List<Pickupable>(); // The list of items that the player has

    private CombatSystem combatSystem;
    public Transform handlePoint;

    private void Start()
    {
        combatSystem = GetComponent<CombatSystem>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Find the closest item within pickup range
            Pickupable pickupable = FindClosestPickupable();
            if (pickupable != null)
            {
                // Check if the pickupable is a weapon
                Weapon weapon = pickupable?.GetComponent<Weapon>();
                if (weapon != null)
                {
                    // Set the weapon in the CombatSystem
                    combatSystem.SetWeapon(weapon);
                }

                // Add the pickupable to the inventory
                inventory.Add(pickupable);
                pickupable.Pickup(handlePoint);
            }
        }

        // Check for input to drop items
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Drop the last pickupable in the inventory
            if (inventory.Count > 0)
            {
                Pickupable pickupable = inventory[inventory.Count - 1];
                inventory.RemoveAt(inventory.Count - 1);
                CombatSystem combatSystem = GetComponent<CombatSystem>();
                pickupable.Drop(combatSystem);
            }
        }
    }

    Pickupable FindClosestPickupable()
    {
        Collider2D[] hitPickupables = Physics2D.OverlapCircleAll(transform.position, pickupRange, itemLayer);

        float closestDistance = Mathf.Infinity;
        Pickupable closestPickupable = null;

        foreach (Collider2D hitPickupable in hitPickupables)
        {
            float distance = Vector2.Distance(transform.position, hitPickupable.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestPickupable = hitPickupable.GetComponent<Pickupable>();
            }
        }

        return closestPickupable;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
