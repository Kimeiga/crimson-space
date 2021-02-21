using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_behavior : MonoBehaviour
{
    public bool is_bouncy = false;
    public int max_hit_count = 5;
    public int public_damage = 10;

    private int hit_counter = 0;
    private int private_damage = 0;
    void OnCollisionEnter2D(Collision2D collision2D)
    {
        // ignore collision if both tagged "bullet"
        if (collision2D.gameObject.CompareTag("bullet"))
        {
            Physics2D.IgnoreCollision(collision2D.collider, collision2D.otherCollider);
            Debug.Log(collision2D.collider);
            Debug.Log(collision2D.otherCollider);
        }
        // increase the hit count and set damage value after colliding more than once
        if (!collision2D.gameObject.CompareTag("bullet"))
        {
            ++hit_counter;
            if (hit_counter > 0)
            {
                private_damage = public_damage;
            }

            if (hit_counter > max_hit_count)
            {
                this.gameObject.SetActive(false);
            }
        }
    }

    // gets called every time when the bullet gets reused in the pool
    public void reset_bullet_damage()
    {
        hit_counter = 0;
        private_damage = 0;
    }

    void Awake()
    {
        if (is_bouncy == false)
        {
            max_hit_count = 0;
            private_damage = public_damage;
        }
    }
}
