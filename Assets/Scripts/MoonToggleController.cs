using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonToggleController : MonoBehaviour
{
    [Header("Target MoonBeam")]
    public GameObject target;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Barrel"))
        {
            if (target.activeSelf)
            {
                target.SetActive(false);
            }
            else
            {
                target.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
