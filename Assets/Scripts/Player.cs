using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private Rigidbody2D playerBody;
    [SerializeField] private CapsuleCollider2D playerCollider2D;
    [SerializeField] private LayerMask platformLayerMask;

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
            playerBody.velocity = Vector2.up * jumpVelocity;
        }
        
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit2d = Physics2D.CapsuleCast(playerCollider2D.bounds.center, playerCollider2D.bounds.size, 0f, 0f, Vector2.down * .1f);
        return raycastHit2d.collider != null;
    }

}
