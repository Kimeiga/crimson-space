using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_pool : MonoBehaviour
{
    public bullet_pool instance;
    public float bullet_force = 20f;
    [System.Serializable]
    public class Pool
    {
        public int name;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<int, Queue<GameObject>> poolDictionary;

    private void Start()
    {
        poolDictionary = new Dictionary<int, Queue<GameObject>>();

        // create pools for different bullets, and add them to the poolDictionary
        foreach (Pool pool in pools)
        {
            Queue<GameObject> bulletPool = new Queue<GameObject>();
            
            // spawn "size" many bullets and add them into the bulletPool
            for (int i = 0; i < pool.size; ++i)
            {
                GameObject bullet = Instantiate(pool.prefab);
                bullet.SetActive(false);
                bullet.transform.SetParent(instance.transform);
                bulletPool.Enqueue(bullet);
            }
            
            poolDictionary.Add(pool.name, bulletPool);
        }
    }

    public void spawnBulletFromPool(int name, Transform transform)
    {
        if (!poolDictionary.ContainsKey(name))
        {
            Debug.Log("No bullet category called" + name + "was set up in the bullet pool, check the inspector.");
            return;
        }
        
        // pull out the bullet from poolDictionary
        GameObject new_bullet = poolDictionary[name].Dequeue();
        new_bullet.GetComponent<bullet_behavior>().reset_bullet_damage();
        
        // spawn the bullet at corresponding place
        new_bullet.SetActive(true);
        new_bullet.transform.position = transform.position;
        new_bullet.transform.rotation = transform.rotation;
        
        // give the bullet a starting force
        Rigidbody2D rb = new_bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(transform.up * bullet_force, ForceMode2D.Impulse);
        
        // put the bullet back to poolDictionary in order for the next spawn
        poolDictionary[name].Enqueue(new_bullet);
    }
}
