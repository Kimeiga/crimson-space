using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerPos;
    public float cameraDist = 3.5f;
    public float smoothTime = 0.2f;

    private Vector3 mousePos;
    private Vector3 target;
    private Vector3 refVel;
    private float zStart;


    void Start()
    {
        target = playerPos.position;
        //target = Vector3.zero;
        zStart = transform.position.z; 
    }

    void FixedUpdate()
    {
        mousePos = CaptureMousePos(); //find out where the mouse is
        target = UpdateTargetPos(); //find out where the camera ought to be
        UpdateCameraPosition(); //smoothly move the camera closer to it's target location
    }
    Vector3 CaptureMousePos()
    {
        Vector2 ret = Camera.main.ScreenToViewportPoint(Input.mousePosition); //raw mouse pos
        ret *= 2;
        ret -= Vector2.one; //set (0,0) of mouse to middle of screen
        float max = 0.9f;
        if (Mathf.Abs(ret.x) > max || Mathf.Abs(ret.y) > max)
        {
            ret = ret.normalized; //helps smooth near edges of screen
        }
        return ret;
    }
    Vector3 UpdateTargetPos()
    {
        Vector3 mouseOffset = mousePos * cameraDist; //mult mouse vector by distance scalar 
        Vector3 ret = playerPos.position + mouseOffset; //find position as it relates to the player
        //Vector3 ret = mouseOffset; //find position as it relates to the player
        ret.z = zStart; //make sure camera stays at same Z coord
        return ret;
    }

    void UpdateCameraPosition()
    {
        Vector3 tempPos;
        tempPos = Vector3.SmoothDamp(transform.position, target, ref refVel, smoothTime); //smoothly move towards the target
        transform.position = tempPos; //update the position
    }

}
