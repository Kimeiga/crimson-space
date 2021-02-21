using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun_script : MonoBehaviour
{
    public bullet_pool bullet_pool_object;
    public GameObject laser_pointer;
    public bool continuous_shooting = true;
    public float shooting_interval = 0.1f;
    public float sniper_zoom_out_value = 20f;
    public float sniper_zoom_speed = 10f;

    private Camera main_camera;
    private float original_orthographicSize;
    private bool is_shooting = false;
    private float shooting_time = 0.0f;
    private bool is_sniper_mode = false;
    private int default_weapon_choice = 1;
    // Start is called before the first frame update
    void Start()
    {
        main_camera = Camera.main;
        original_orthographicSize = main_camera.orthographicSize;
        laser_pointer.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // handle shooting
        ShootingHandler();

        // handle change weapon
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            default_weapon_choice = 1;
            laser_pointer.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            default_weapon_choice = 2;
            laser_pointer.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            default_weapon_choice = 3;
            laser_pointer.SetActive(true);
        }

        // enter/exit broaden sniper view
        if (default_weapon_choice == 3 && Input.GetKeyDown(KeyCode.LeftShift)){
            is_sniper_mode = true;
        }
        else if (default_weapon_choice != 3 || Input.GetKeyUp(KeyCode.LeftShift)){
            is_sniper_mode = false;
        }
        SniperMode();
    }

    void ShootingHandler(){
        if (continuous_shooting == true){
            if (Input.GetMouseButtonDown(0))
            {
                is_shooting = true;
            }
            else if (Input.GetMouseButtonUp(0)){
                is_shooting = false;
            }
            continuous_shoot();
        }
        else{
            if (Input.GetMouseButtonDown(0))
            {
                discrete_shoot();
            }
        }
    }
    void discrete_shoot()
    {
        bullet_pool_object.spawnBulletFromPool(default_weapon_choice, transform);
    }

    void continuous_shoot()
    {
        if (is_shooting == true && Time.time >= shooting_time){
            shooting_time = Time.time + shooting_interval;
            bullet_pool_object.spawnBulletFromPool(default_weapon_choice, transform);
        }
    }

    void SniperMode(){
        if(is_sniper_mode == true){
            Mathf.Clamp(main_camera.orthographicSize, original_orthographicSize, sniper_zoom_out_value);
            main_camera.orthographicSize = Mathf.Lerp(main_camera.orthographicSize, sniper_zoom_out_value, Time.deltaTime * sniper_zoom_speed);
        }
        else{
            Mathf.Clamp(main_camera.orthographicSize, original_orthographicSize, sniper_zoom_out_value);
            main_camera.orthographicSize = Mathf.Lerp(main_camera.orthographicSize, original_orthographicSize, Time.deltaTime * sniper_zoom_speed);
        }
    }
}
