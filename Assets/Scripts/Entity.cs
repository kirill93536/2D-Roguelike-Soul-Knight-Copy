using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public int hp;
    public int stamina;
    public int mp;

    [HideInInspector] public UnityEvent onDie;

    public virtual void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0)
        {
            onDie.Invoke();
            Die();
        }
    }

    public virtual void ReduceMP(int mana)
    {
        mp -= mana;
        if(mp <= 0)
        {
            mp = 0;
        }
    }

    public virtual void Heal(int amount)
    {
        hp += amount;
    }

    public virtual void Die()
    {
        // Destroy this object
        gameObject.SetActive(false);
    }
}
