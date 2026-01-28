using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D capsuleCollider2d;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask platformLayerMask;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 inputVector = new Vector2(0, 0);

        if (Input.GetKey(KeyCode.D))
        {
            inputVector.x += 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputVector.x -= 1;
        }

        Vector3 moveDir = new Vector3(inputVector.x, 0f, 0f);
        transform.position += moveDir * Time.deltaTime * moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            float jumpVelocity = 30f;
            playerBody.velocity = Vector2.up * jumpVelocity;
        }
    }

    private bool IsGrounded()
    {
        float extraHeight = 0.01f;
        RaycastHit2D raycastHit = Physics2D.CapsuleCast(capsuleCollider2d.bounds.center, capsuleCollider2d.bounds.size, capsuleCollider2d.direction, 0f, Vector2.down, extraHeight, platformLayerMask);
        return raycastHit.collider != null;
    }
        
    
}
