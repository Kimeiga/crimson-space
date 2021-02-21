using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SpriteMovement : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    public Sprite idleSprite;
    public Sprite forwardSprite;
    public Sprite backwardSprite;
    public Sprite upSprite;
    public Sprite downSprite;

    private bool facingRight;
    private Vector3 originalLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        originalLocalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") > 0.5f)
        {
            spriteRenderer.sprite = upSprite;
        }
        else if (Input.GetAxisRaw("Vertical") < -0.5f)
        {
            spriteRenderer.sprite = downSprite;
        }
        else if (Input.GetAxisRaw("Horizontal") > 0.5f)
        {
            
            spriteRenderer.sprite = facingRight ? forwardSprite : backwardSprite;
        }
        else if (Input.GetAxisRaw("Horizontal") < -0.5f)
        {
            spriteRenderer.sprite = facingRight ? backwardSprite: forwardSprite;
        }
        else
        {
            spriteRenderer.sprite = idleSprite;
        }
        
        
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
        var playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);
        if(mousePosition.x < playerScreenPoint.x)
        {
            facingRight = false;
            transform.localScale = new Vector3(-originalLocalScale.x, originalLocalScale.y, originalLocalScale.z);
        } else
        {
            facingRight = true;
            transform.localScale = new Vector3(originalLocalScale.x, originalLocalScale.y, originalLocalScale.z);
        }
    }
}
