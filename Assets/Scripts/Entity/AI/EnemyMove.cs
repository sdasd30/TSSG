using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public int playerSpeed = 10;
    public bool facingRight = true;
    public int jumpPower = 1250;

    private AIInputParentClass m_aibase;

    // Start is called before the first frame update
    void Start()
    {
        m_aibase = GetComponent<AIInputParentClass>();

    }
    // Update is called once per frame
    void Update()
    {
        
        InputPacket p = new InputPacket();
        if (m_aibase != null)
            p = m_aibase.AITemplate();
        MovePlayer(p);
    }

    void MovePlayer(InputPacket ip)
    {
        //CONTROL
        float moveX = ip.movementInput.x;
        if (ip.jump)
        {
            Jump();
        }
        //ANIMATION
        //PLAYER DIRECTION
        if (moveX > 0.0f && facingRight == false)
        {
            FlipPlayer();
        }
        else if (moveX < 0.0f && facingRight == true)
        {
            FlipPlayer();
        }
        //PHYSICS
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(moveX * playerSpeed * ScaledTime.deltaTime, gameObject.GetComponent<Rigidbody2D>().velocity.y);

    }

    void Jump()
    {
        GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpPower * ScaledTime.deltaTime);
    }

    void FlipPlayer()
    {
        facingRight = !facingRight;
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
