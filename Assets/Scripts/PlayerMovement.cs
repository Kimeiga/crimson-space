using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 15.0f;
    public float dashSpeed = 30.0f;
    public float dashIFrameDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float dashOpacity = 0.5f;
    public Rigidbody2D rb;
    public Camera cam;
    public SpriteRenderer sprite;

    Vector2 movement;
    Vector2 mousePos;
    Vector2 dashDirection;

    private float dashCDTime = 0.0f;
    private float dashIFrameTime = 0.0f;
    private bool isDashing; // well we're all dashing, but not like that

    void Start() {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        cam = Camera.main;
    }
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        if(movement.magnitude > 1)
            movement.Normalize();

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        if (movement != Vector2.zero && Time.time >= dashCDTime && (Input.GetKeyDown(KeyCode.Space)))
        {
            dashCDTime = Time.time + dashCooldown;
            dashIFrameTime = Time.time + dashIFrameDuration;
            dashDirection = movement;
            isDashing = true;
        }

        if (isDashing)
        {
            sprite.color = new Color(1f, 1f, 1f, dashOpacity);
        }
        else
        {
            sprite.color = new Color(1f, 1f, 1f, 1f);
        }

    }

    void FixedUpdate()
    {
        if (Time.fixedTime >= dashIFrameTime)
        {
            isDashing = false;
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
        }

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;


    }
}
