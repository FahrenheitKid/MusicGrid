using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;
using DG.Tweening;

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


    public int powerUpNeeded;
    public int SlowPower;
    public int SingleJumpPower;
    public int musicPower;

    public bool slowed;
    public float slowed_timer;
    public float slowed_time;

    public bool single_jumped;
    public float singleJumped_timer;
    public float singleJumped_time;


    public GameObject otherPlayer;
    public JukeboxScript Juke_Ref;

    public GridMakerScript grid;

    public bool checkDeath = false;
    public float deathTimer = 0.0f;
    public float deathIsComingTime;
    public int levelFromHighestToTriggerDeath;

    public InterfaceManager interfaceManager;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        rend.material.color = color;
        if (levelFromHighestToTriggerDeath > grid.level_gap_limit)
            levelFromHighestToTriggerDeath = grid.level_gap_limit - 1;

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

        handleTimers();
        handleMovement();


        if (checkDeath)
        {
            if (deathTimer <= 0)
            {
                if (isPlayer1)
                    GameOver(0);
                else
                    GameOver(1);
                //gameover routine;
            }

            deathTimer -= Time.deltaTime;
        }

        

    }

    public void handleTimers()
    {
        if(slowed)
        {
            //speed = (float)speed * 0.5f;

            slowed_time -= Time.deltaTime;
            if(slowed_time <= 0)
            {
                slowed = false;
                speed *= 2;
            }

        }

        if(single_jumped)
        {

            singleJumped_time -= Time.deltaTime;
            if (singleJumped_time <= 0)
            {
                single_jumped = false;
            }
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
                if (!controller.isGrounded && dJumpCounter < nrOfAlowedDJumps && !single_jumped)
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
                if (!controller.isGrounded && dJumpCounter < nrOfAlowedDJumps && !single_jumped)
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

       
        if (musicPower >= powerUpNeeded)
        {
            print("test " + musicPower + " - " + powerUpNeeded);
            if (!Juke_Ref.isMusicPower)
            {
                musicPower = 0;
                SlowPower = 0;
                SingleJumpPower = 0;

                if (isPlayer1)
                    interfaceManager.RemoveAll(0);
                else
                    interfaceManager.RemoveAll(1);


                print("aplayou");

                //DOTween.To(,0.7,)

                int luck = Random.Range(0, 2);
                if (luck % 2 == 0)
                {
                    float val1 = Juke_Ref.GetComponent<AudioSource>().pitch - Juke_Ref.pitch_modifier;
                    float val2 = Juke_Ref.GetComponent<AudioProcessor>().gThresh + Juke_Ref.gThresh_modifier;
                    DOTween.To(() => Juke_Ref.GetComponent<AudioSource>().pitch, x => Juke_Ref.GetComponent<AudioSource>().pitch = x, val1, 2);
                    DOTween.To(() => Juke_Ref.GetComponent<AudioProcessor>().gThresh, x => Juke_Ref.GetComponent<AudioProcessor>().gThresh = x, val2, 2);

                }
                else
                {
                    float val1 = Juke_Ref.GetComponent<AudioSource>().pitch + Juke_Ref.pitch_modifier;
                    float val2 = Juke_Ref.GetComponent<AudioProcessor>().gThresh - Juke_Ref.gThresh_modifier;
                    DOTween.To(() => Juke_Ref.GetComponent<AudioSource>().pitch, x => Juke_Ref.GetComponent<AudioSource>().pitch = x, val1, 2);
                    DOTween.To(() => Juke_Ref.GetComponent<AudioProcessor>().gThresh, x => Juke_Ref.GetComponent<AudioProcessor>().gThresh = x, val2, 2);
                }


                Juke_Ref.isMusicPower = true;
                Juke_Ref.musicPower_time = Juke_Ref.musicPower_timer;
            }
            else
            {
                musicPower = 2;
            }
        }

        if (SlowPower >= powerUpNeeded)
        {
            musicPower = 0;
            SlowPower = 0;
            SingleJumpPower = 0;

            if (isPlayer1)
                interfaceManager.RemoveAll(0);
            else
                interfaceManager.RemoveAll(1);

            PlayerScript ps = otherPlayer.GetComponent<PlayerScript>();
            ps.slowed = true;
            ps.slowed_time = ps.slowed_timer;
            ps.speed /= 2;

        }

        if (SingleJumpPower >= powerUpNeeded)
        {
            musicPower = 0;
            SlowPower = 0;
            SingleJumpPower = 0;

            if (isPlayer1)
                interfaceManager.RemoveAll(0);
            else
                interfaceManager.RemoveAll(1);

            PlayerScript ps = otherPlayer.GetComponent<PlayerScript>();
            ps.single_jumped = true;
            ps.singleJumped_time = ps.singleJumped_timer;


        }

    }

    void GameOver(int player)
    {
        //Time.timeScale = 0.0f;
        interfaceManager.GameOver(player);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.CompareTag("EndWorld"))
        {
            if (isPlayer1)
                GameOver(0);
            else
                GameOver(1);
        }

        if (hit.transform.CompareTag("Block"))
        {
            Vector3 collisionPoint = hit.normal;
            float dotUp = Vector3.Dot(-collisionPoint, Vector3.down);
            float dotLeft = Vector3.Dot(-collisionPoint, Vector3.left);
            if (dotUp > 0.99f)
            {
                if (hit.gameObject.GetComponent<GridBlockScript>().level < grid.highest_level - levelFromHighestToTriggerDeath)
                {
                    if (!checkDeath)
                        deathTimer = deathIsComingTime;
                    checkDeath = true;
                    rend.material.color = Color.red;
                }
                else
                {
                    rend.material.color = Color.white;
                    checkDeath = false;
                    deathTimer = deathIsComingTime;
                }


                int g = -1; // index do bloco
                for (int i = 0; i < grid.grid_List.Count; i++)
                {
                    if (hit.gameObject == grid.grid_List[i]) g = i;

                }

                

                if (isPlayer1)
                {
                    if (g >= 0)
                        grid.lastBlockHitP1 = g;

                }
                else
                {
                    if (g >= 0)
                        grid.lastBlockHitP2 = g;
                }
                
            }
        }


        if (hit.transform.CompareTag("PowerUp"))
        {
            PowerUpScript p = hit.transform.gameObject.GetComponent<PowerUpScript>();

            if (p.isMusicPower)
            {
                musicPower++;
                if (isPlayer1)
                    interfaceManager.addPower(2, 0, musicPower-1);
                else
                    interfaceManager.addPower(2, 1, musicPower-1);
            }
            else if (p.isSlowPower)
            {
                SlowPower++;
                if (isPlayer1)
                    interfaceManager.addPower(0, 0, SlowPower-1);
                else
                    interfaceManager.addPower(0, 1, SlowPower-1);
            }
            else if (p.isSingleJumpPower)
            {
                SingleJumpPower++;
                if (isPlayer1)
                    interfaceManager.addPower(1, 0, SingleJumpPower-1);
                else
                    interfaceManager.addPower(1, 1, SingleJumpPower-1);
            }

            applyPower();
            grid.powerUps.Remove(hit.gameObject);
            Destroy(hit.gameObject);

        }
    }
}
