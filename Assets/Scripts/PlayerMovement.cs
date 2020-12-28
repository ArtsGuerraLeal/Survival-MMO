using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    Vector2 playerInput;
    Vector3 targetPos;
    Rigidbody2D rb;
    public float moveSpeed = 2;
    private bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0)) {
            SetTargetPos();
        }

        if (isMoving) {
            MoveMouse();
        }

        MoveKeys();

        
    }

    void MoveKeys() {
        playerInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = playerInput.normalized * moveSpeed;
    }

    void MoveMouse()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (transform.position == targetPos) {
            isMoving = false;
        }

        if (Input.anyKeyDown)
        {
            isMoving = false;
            targetPos = transform.position;
        }
    }

    void SetTargetPos() {
        targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPos.z = transform.position.z;

        isMoving = true;
    }
}
