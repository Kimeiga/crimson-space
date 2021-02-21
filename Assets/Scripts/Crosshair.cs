using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    private Vector3 mouseCoords;
    public float mouseSensitivity = 0.1f;

    public GameObject crosshair;

    public bool cursorVisible = false;
    
    
    // Start is called before the first frame update
    void Start()
    {
        if (!cursorVisible)
        {
            Cursor.visible = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        // TODO:
        // press escape to start the pause menu,
        // this can unlock the cursor
        
        
        mouseCoords = Input.mousePosition;
        mouseCoords = Camera.main.ScreenToWorldPoint(mouseCoords);
        mouseCoords.z = 1;
        crosshair.transform.position = mouseCoords;
    }
}
