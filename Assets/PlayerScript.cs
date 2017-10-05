using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class PlayerScript : MonoBehaviour
{



    // Moving fields
    [SerializeField] // This will make the variable below appear in the inspector
    float speed = 6;
    [SerializeField]
    float jumpSpeed = 8;
    [SerializeField]
    float gravity = 20;
    Vector3 moveDirection = Vector3.zero;
    CharacterController controller;
    //bool isJumping; // "controller.isGrounded" can be used instead
    [SerializeField]
    int nrOfAlowedDJumps = 1; // New vairable
    int dJumpCounter = 0;     // New variable

    public bool isPlayer1;

    public Color color;
    public Renderer rend;

    public int SlowPower;
    public int SingleJumpPower;
    public int musicPower;
    BeatObserver beatObserver;
    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        beatObserver = GetComponent<BeatObserver>();
        rend.material.color = color;
        //Select the instance of AudioProcessor and pass a reference
        //to this object
        //AudioProcessor processor = FindObjectOfType<AudioProcessor>();
        //processor.onBeat.AddListener(onOnbeatDetected);
        //processor.onSpectrum.AddListener(onSpectrum);
    }

    void onOnbeatDetected()
    {
        Color lastcolor = color;
        color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        //rend.material.color = color;
        Debug.Log("Beat!!!");

    }

    void Update()
    {
        handleMovement();

        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            onOnbeatDetected();
        }

    }

    public void handleMovement()
    {

        if (transform.position.x <= -0.2)
        {
            Vector3 newpos = transform.position;
            newpos.x = -0.2f;
            transform.position = newpos;
        }

        if (transform.position.x >= 6.5)
        {
            Vector3 newpos = transform.position;
            newpos.x = 6.5f;
            transform.position = newpos;
        }

        if (transform.position.z <= -0.2f)
        {
            Vector3 newpos = transform.position;
            newpos.z = -0.2f;
            transform.position = newpos;
        }

        if (transform.position.z >= 6.5)
        {
            Vector3 newpos = transform.position;
            newpos.z = 6.5f;
            transform.position = newpos;
        }


        if (isPlayer1)
        {
            moveDirection.x = Input.GetAxis("P1_Horizontal") * speed;
            moveDirection.z = Input.GetAxis("P1_Vertical") * speed;

            if (Input.GetButtonDown("P1_Jump"))
            {
                if (controller.isGrounded)
                {
                    moveDirection.y = jumpSpeed;
                    dJumpCounter = 0;
                }
                if (!controller.isGrounded && dJumpCounter < nrOfAlowedDJumps)
                {
                    moveDirection.y = jumpSpeed;
                    dJumpCounter++;
                }
            }

        }
        
        else
        {
            moveDirection.x = Input.GetAxis("P2_Horizontal") * speed;
            moveDirection.z = Input.GetAxis("P2_Vertical") * speed;

            if(Input.GetButtonDown("P2_Jump"))
            {
                if (controller.isGrounded)
                {
                    moveDirection.y = jumpSpeed;
                    dJumpCounter = 0;
                }
                if (!controller.isGrounded && dJumpCounter < nrOfAlowedDJumps)
                {
                    moveDirection.y = jumpSpeed;
                    dJumpCounter++;
                }
            }
        }
        

       

      
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void applyPower()
    {

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("Wall"))
        {
            Vector3 collisionPoint = hit.normal;
            float dotRight = Vector3.Dot(-collisionPoint, Vector3.right);
            float dotLeft = Vector3.Dot(-collisionPoint, Vector3.left);
            if (dotRight > 0.99f)
            {
                Debug.Log("Right");
            }
            if (dotLeft > 0.99f)
            {
                Debug.Log("Left");
            }
        }


        if (hit.transform.CompareTag("PowerUp"))
        {
            PowerUpScript p = hit.transform.gameObject.GetComponent<PowerUpScript>();

            if (p.isMusicPower)
                musicPower++;
            else if (p.isSlowPower)
                SlowPower++;
            else if (p.isMusicPower)
                musicPower++;

            applyPower();
            Destroy(hit.gameObject);

        }
    }
}
