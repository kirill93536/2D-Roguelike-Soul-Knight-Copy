using UnityEngine;

public abstract class Pickupable : MonoBehaviour
{
    public virtual void Pickup(Transform handlePoint)
    {
        transform.SetParent(handlePoint);
        transform.position = handlePoint.position;
        transform.rotation = handlePoint.rotation;
        gameObject.SetActive(true);
    }
    public virtual void Drop(CombatSystem combatSystem)
    {
        transform.parent = null;
        combatSystem.currentWeapon = null;
    }
}