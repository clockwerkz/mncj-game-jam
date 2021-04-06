using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: How do you keep the momentum for in/out moon beams?
// TODO: Should transforming be instant or have a time component?
// TODO: Should inputs be locked during pending transform and/or transforming?
// TODO: Seprate horizontal speed for pirate/bird?
// TODO: Transition(s) animations
// TODO: Bird cannot interact with physical objects (platforms and barrels etc.)

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Settings to Tweak for Gameplay Feel")]
    public float moveSpeedPirateHori;
    public float moveSpeedBirdHori;
    public float moveSpeedBirdVert;
    public float jumpForce;
    public float normalGravity;
    public float flightGravity;
    public float intoMoonDelaySeconds;
    public float outOfMoonDelaySeconds;

    [Space(10.0f)]
    [Header("Ground Detection Settings")]
    public Transform feet;
    public LayerMask groundLayer;

    // Components
    private Rigidbody2D _rigidBody2d;
    private Collider2D _collider2d;
    private SpriteRenderer _spriteRenderer;

    // Movement inputs
    private float _mx;
    private float _my;

    // Keep track of whether the player is inside a moonbeam
    private bool _inMoonLight = false;

    // Counters for going in/out of moonbeams
    private float _intoMoonCounter;
    private float _outOfMoonCounter;

    // Particles for going in/out of moonbeams
    private ParticleSystem _transformationParticles;
    private ParticleSystem.EmissionModule _transformationEmission;

    // Keep track of Player transformations
    private enum PlayerTransform
    {
        Pirate,
        Bird
    }
    private PlayerTransform _playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2d = GetComponent<Rigidbody2D>();
        _collider2d = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _transformationParticles = GetComponentInChildren<ParticleSystem>();
        _transformationEmission = _transformationParticles.emission;

        // Set gravity and player is pirate
        _rigidBody2d.gravityScale = normalGravity;
        _playerTransform = PlayerTransform.Pirate;
    }

    private void FixedUpdate()
    {
        _mx = Input.GetAxisRaw("Horizontal");
        _my = Input.GetAxisRaw("Vertical");
    }

    // Update is called once per frame
    void Update()
    {
        // Update moonbeam counters
        if (_intoMoonCounter > 0)
        {
            _intoMoonCounter -= Time.deltaTime;
        }
        if (_outOfMoonCounter > 0)
        {
            _outOfMoonCounter -= Time.deltaTime;
        }

        // Configure transformation particles
        if (_playerTransform == PlayerTransform.Bird && !_inMoonLight)
        {
            _transformationEmission.rateOverTime = 50f;
        }
        else
        {
            _transformationEmission.rateOverTime = 0f;
        }

        // Maybe transform
        MaybeTransform();

        // Calculate horizontal velocity
        float moveSpeedHori = _playerTransform == PlayerTransform.Pirate ? moveSpeedPirateHori : moveSpeedBirdHori;
        Vector2 movementHori = new Vector2(_mx * moveSpeedHori, _rigidBody2d.velocity.y);
        _rigidBody2d.velocity = movementHori;

        // Calculate vertical movement (fly if bird and jump if pirate)
        if (_playerTransform == PlayerTransform.Bird)
        {
            Vector2 movementVert = new Vector2(_rigidBody2d.velocity.x, _my * moveSpeedBirdVert);
            _rigidBody2d.velocity = movementVert;
        }
        else if (_playerTransform == PlayerTransform.Pirate && Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
        }
    }

    bool IsGrounded()
    {
        Collider2D groundCheck = Physics2D.OverlapCircle(feet.position, 0.2f, groundLayer);
        return groundCheck != null;
    }

    void Jump()
    {
        Vector2 movement = new Vector2(_rigidBody2d.velocity.x, jumpForce);
        _rigidBody2d.velocity = movement;
    }

    void MaybeTransform()
    {
        // Stayed in moonbeam long enough to become a bird
        if (_intoMoonCounter <= 0 && _playerTransform == PlayerTransform.Pirate && _inMoonLight)
        {
            _playerTransform = PlayerTransform.Bird;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0.5f);
            _rigidBody2d.gravityScale = flightGravity;

            // Collisions off
            gameObject.layer = 9;

            // Stop moving
            _rigidBody2d.velocity = new Vector2(_rigidBody2d.velocity.x, 0);
        }

        // Stayed out of moonbeam long enough to become a pirate
        if (_outOfMoonCounter <= 0 && _playerTransform == PlayerTransform.Bird && !_inMoonLight)
        {
            _playerTransform = PlayerTransform.Pirate;
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1.0f);
            _rigidBody2d.gravityScale = normalGravity;

            // Collisions on
            gameObject.layer = 8;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "MoonBeam")
        {
            _inMoonLight = true;
            _intoMoonCounter = intoMoonDelaySeconds;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "MoonBeam")
        {
            _inMoonLight = false;
            _outOfMoonCounter = outOfMoonDelaySeconds;
        }
    }
}
