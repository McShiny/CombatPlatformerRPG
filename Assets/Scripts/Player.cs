using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D playerBody;
    public float speed = 5f;
    private Vector2 movement;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        float input = Input.GetAxisRaw("Horizontal");
        movement.x = input * speed * Time.deltaTime;
        transform.Translate(movement);

        if (input != 0)
        {
        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Sign(input) * Mathf.Abs(localScale.x);
        transform.localScale = localScale;
        } 
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float jumpVelocity = 30f;
            playerBody.velocity = Vector2.up * jumpVelocity;

        }

        
    }
}
