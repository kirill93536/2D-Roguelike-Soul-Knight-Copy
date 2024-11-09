using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePooler : MonoBehaviour
{
    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;

    public GameObject holder;

    private void Awake()
    {
        amountToPool = 40;
        holder = new GameObject("Object Pool");

        pooledObjects = new List<GameObject>();

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false);
            obj.transform.SetParent(holder.transform);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].tag == tag)
            {
                return pooledObjects[i];
            }
        }

        GameObject obj = Instantiate(objectToPool);
        obj.SetActive(false);
        obj.transform.SetParent(holder.transform);
        pooledObjects.Add(obj);

        return obj;
    }
}
