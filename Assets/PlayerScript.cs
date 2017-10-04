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


    public Color color;
    public Renderer rend;

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
        rend.material.color = color;
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
        moveDirection.x = Input.GetAxis("Horizontal") * speed;
        moveDirection.z = Input.GetAxis("Vertical") * speed;

        if (Input.GetButtonDown("Jump"))
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
        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
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
    }
}
