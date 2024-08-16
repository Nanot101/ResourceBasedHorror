using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private float velocityY;
    private float velocityX;
    private Vector2 velocity;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        PlayerMove();
    }
    private void PlayerMove()
    {
        velocityX = Input.GetAxisRaw("Horizontal");
        velocityY = Input.GetAxisRaw("Vertical");

        velocity = new Vector2(velocityX, velocityY);
        velocity *= speed;

        rb.velocity = velocity;
    }
}
