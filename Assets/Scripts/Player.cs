using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D rigidbody2;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            float jumpVelocity = 30f;
            rigidbody2.velocity = Vector2.up * jumpVelocity;

        }
        
    }
}
