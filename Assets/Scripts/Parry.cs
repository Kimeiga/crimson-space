using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry : MonoBehaviour
{
    public ParticleSystem ps;
    public float parryCooldown = 0.6f;

    private float parryCDTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        ps.Pause();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(1) && Time.time >= parryCDTimer)
        {
            ps.Play();
            parryCDTimer = Time.time + parryCooldown;
        }
        
    }
}
