using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3;
    public float jumpForce = 20;
    public Transform feet;
    public LayerMask moonLight;

    private Rigidbody2D _rigidBody2d;
    private float _mx;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        _mx = Input.GetAxisRaw("Horizontal");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = new Vector2(_mx * moveSpeed, _rigidBody2d.velocity.y);
        _rigidBody2d.velocity = movement;
    }
}
