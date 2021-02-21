using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class laser_beam : MonoBehaviour
{
    public int max_reflect_count = 3;
    public float ray_width = 0.05f;
    public LineRenderer lineRenderer;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = max_reflect_count;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = ray_width;
        lineRenderer.endWidth = ray_width;
        lineRenderer.enabled = true;
    }


    void Update()
    {
        ShootLaser();
    }

    void ShootLaser(){
        Vector2 position = transform.position;
        Vector2 direction = transform.up;

        lineRenderer.SetPosition(0, position);
        RaycastHit2D hitInfo;

        for (int i = 1; i < max_reflect_count; ++i){
            hitInfo = Physics2D.Raycast(position, direction);
            // if the raycast doesn't hit anything, then draw a long line streching out of the screen
            if(hitInfo == false){
                lineRenderer.SetPosition(i, position + direction * 10f);
                break;
            }
            // if the raycast collides with objects other than bullets, then draw a line
            // from starting point to end point
            else if(!hitInfo.collider.gameObject.CompareTag("bullet")){
                lineRenderer.SetPosition(i, hitInfo.point);

                position = hitInfo.point + 0.01f * hitInfo.normal;
                direction = Vector2.Reflect(direction, hitInfo.normal);
            }
            // if the raycast hit bullets, do nothing

        }
    }
}
