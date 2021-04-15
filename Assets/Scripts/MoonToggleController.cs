using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonToggleController : MonoBehaviour
{
    [Header("Target MoonBeam")]
    public GameObject target;

    [Header("Sprites")]
    public Sprite activeSprite;
    public Sprite inactiveSprite;

    // Components
    private SpriteRenderer _spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.sprite = inactiveSprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Barrel"))
        {
            if (target.activeSelf)
            {
                target.SetActive(false);
                _spriteRenderer.sprite = inactiveSprite;
            }
            else
            {
                target.SetActive(true);
                _spriteRenderer.sprite = activeSprite;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
