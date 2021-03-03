using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private float baseSpeed;
    public float moveSpeed;
    public float runSpeed;
    public Animator anim;
    private bool playerMoving;
    private bool playerRunning;
    private Vector2 lastMove;
    private bool playerAttacking;
    private bool playerChopping;
    private Transform target;
    private Rigidbody2D rb;
    private Vector2 movement;
    private bool facingRight;
    public AudioSource walkSound;
    public AudioSource runSound;
    public GameObject playerName;

    public static PlayerController instance;
    private bool isRunning = false;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        baseSpeed = moveSpeed;
        //Get the components
        facingRight = true;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    

    }

    public void SetName(string n) {
    }



    void FixedUpdate()
    {
        if (isLocalPlayer) {
            rb.simulated = true;
        MoveKeys();

        if (walkSound.isPlaying && !playerMoving)
        {
            walkSound.Stop();

        }
           

        }


    }

    void GetMovementAxis() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    
    void MoveKeys() {

        GetMovementAxis();


        playerMoving = false;
        
        //Movement with Keys

        if (Input.GetAxisRaw("Horizontal") > 0.5f || Input.GetAxisRaw("Horizontal") < -0.5f)
        {
            if (!walkSound.isPlaying) {
                walkSound.Play();
          
            }

            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
            playerMoving = true;
            
            lastMove = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        }

        if (Input.GetAxisRaw("Vertical") > 0.5f || Input.GetAxisRaw("Vertical") < -0.5f)
        {
            if (!walkSound.isPlaying)
            {
                walkSound.Play();
                
            }

            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
            playerMoving = true;
            lastMove = new Vector2(0, Input.GetAxisRaw("Vertical"));
            
        }

        if (Input.GetButton("Run"))
        {
            if (walkSound.isPlaying) {
                walkSound.Stop();
            }

            if (!runSound.isPlaying && playerMoving)
            {
                if (Player.instance.GetStamina() > 10) {

                    playerRunning = true;

                    if (!isRunning) {
                    StartCoroutine(Drain());
                    }

                    moveSpeed = runSpeed;
                    runSound.Play();
                }
               
            }

            if (runSound.isPlaying && !playerMoving)
            {
                runSound.Stop();
                playerRunning = false;
            }
                
            
        }
        else
        {
            runSound.Stop();
            if (playerMoving) {

                if (!walkSound.isPlaying)
                {
                    walkSound.Play();
                }


            }
            moveSpeed = baseSpeed;
            playerRunning = false;
        }

        MoveAnimation();
        Flip();

    }

    IEnumerator Drain() {
        isRunning = true;
        while (playerRunning) {
        
            int currentStam = Player.instance.GetStamina();
            Player.instance.SetStamina(currentStam - 1);
            yield return new WaitForSeconds(1f);
        }
        isRunning = false;
    }

    void MoveAnimation() {
        //Animation for movement

        anim.SetFloat("MoveX", Input.GetAxisRaw("Horizontal"));

        anim.SetFloat("MoveY", Input.GetAxisRaw("Vertical"));

        anim.SetFloat("LastMoveX", lastMove.x);

        anim.SetFloat("LastMoveY", lastMove.y);

        anim.SetBool("PlayerMoving", playerMoving);

        anim.SetBool("PlayerRunning", playerRunning);
    }

    void Flip()
    {
        if (Input.GetAxisRaw("Horizontal") > 0 && !facingRight || Input.GetAxisRaw("Horizontal") < 0 && facingRight)
        {

            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            Vector3 namesPosOrig = playerName.transform.position;

            Vector3 namesPos = new Vector3(1,0.5f,0);


            scale.x *= -1;
            // namescale.x *= -1;
            transform.Rotate(new Vector3(0, 180, 0));
            //transform.localScale = scale;
            //  playerName.transform.localScale = namescale;

            playerName.transform.rotation = Quaternion.Euler(0.0f, transform.rotation.z * -1.0f, 0.0f);

            if (playerName.transform.rotation.y < 0)
            {
                playerName.transform.position = namesPos;
            }
            else {
                playerName.transform.position = namesPosOrig;

            }

        }
    }



}
