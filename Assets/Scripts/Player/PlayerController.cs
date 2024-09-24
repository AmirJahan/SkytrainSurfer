using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    static PlayerController Instance;

    [SerializeField] private AnimationInputs animation;
    
    public static PlayerController GetInstance()
    {
        if (!Instance)
        {
            Instance = FindObjectOfType<PlayerController>();
        }

        return Instance;
    }


    [Header("Hop Settings")]
    [SerializeField, Tooltip("How farfg can the player move left and right")]
    private float hopIncrement = 25f;

    [SerializeField, Tooltip("How long the player takes to hop left or right")]
    private float hopSpeed = 2f;


    [Header("Jump Settings")]
    [SerializeField, Tooltip("How many player heights the player can jump")]
    float jumpHeight = 2f;

    [SerializeField, Tooltip("How long the player takes to jump")]
    float jumpSpeed = 2f;

    [SerializeField, Tooltip("How long the player can stay in the air")]
    float jumpHangTime = 0.5f;

    [SerializeField, Tooltip("Whether or not the player has jumped")]
    bool hasJumped = false;

    private bool fastFall = false;

    [SerializeField, Tooltip("The amount of health the currently player has")]
    float currentHealth = 100f;

    [SerializeField, Tooltip("The amount of health the  player can have at most")]
    float maxtHealth = 100f;

    [SerializeField, Tooltip("The amount of damage an obstacle does to the player")]
    float obstacleDamage = 10f;


    [Header("Slide settings")]

    [SerializeField, Tooltip("Slide duration")]
    private float slideDuration = 0.5f;

    [SerializeField, Tooltip("Slide transition time")]
    private float slideTransitionTime = 0.25f;

    [SerializeField, Tooltip("How fast the character falls when sliding in air")]
    float fastFallMultiplier = 150f;

    public bool isSliding = false;


    [Header("Controller Values")]
    [SerializeField, Tooltip("The lane the player is currently in")]
    private int lane = 0;


    [SerializeField, Tooltip("Whether or not the player is currently in the middle of an action")]
    private bool pauseInput = false;

    [SerializeField, Tooltip("Whether or not the player is alive")]
    bool isAlive = true;

    private Rigidbody rb;
    private PlayerInput input;
    private CapsuleCollider col;
    
    [Header("Effects")]
    [SerializeField, Tooltip("The effect for when the player jumps")] GameObject jumpEffectPrefab;
    [SerializeField, Tooltip("The effect for when the player moves")] GameObject moveEffectPrefab;
    
    VisualEffect jumpEffect;
    private VisualEffect moveEffect;

    
    private void OnValidate()
    {
        // Add
        if (!rb)
        {
            if (!(rb = GetComponent<Rigidbody>()))
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }

            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;

        }

        if (!input)
        {
            if (!(input = GetComponent<PlayerInput>()))
            {
                input = gameObject.AddComponent<PlayerInput>();
            }

        }
    }



    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<CapsuleCollider>();
        // the player should have full health at the start
        currentHealth = maxtHealth;
        
        VisualEffect je = Instantiate(jumpEffectPrefab, transform).GetComponent<VisualEffect>();
        VisualEffect me = Instantiate(moveEffectPrefab, transform).GetComponent<VisualEffect>();
        if (je)
        {
            jumpEffect = je;
            jumpEffect.resetSeedOnPlay = true;
        }
        else
        {
            Debug.Log("No jump effect prefab assigned to the player controller");
        }
        
        if (me)
        {
            moveEffect = me;
            moveEffect.resetSeedOnPlay = true;
        }
        else
        {
            Debug.Log("No move effect prefab assigned to the player controller");
        }
    }



    private void Update()
    {
        if (isAlive)
        {
            // don't accept input if the player is in the middle of an action
            if (!pauseInput)
            {
                Vector2 moveDir = input.MoveDir;

                int xIn = Mathf.RoundToInt(moveDir.x);
                if (xIn != 0)
                {
                    StartCoroutine(HopToSide(xIn));
                }

                int yIn = Mathf.RoundToInt(moveDir.y);

                if (yIn > 0)
                {
                    StartCoroutine(Jump());
                }

                if (yIn < 0)
                {
                    StartCoroutine(Slide());

                }

            }


        }
    }



    // Causes the player to hop left or right
    // NOTE it is restricted from moving forwards or backwards. Change it if it is needed above
    IEnumerator HopToSide(int direction)
    {
        // Don't allow the player to hop if they are at the edge of the screen
        if ((lane == -1 && direction == -1) || (lane == 1 && direction == 1) || pauseInput)
            yield break;
        
        animation.PlayAction(AnimationInputs.ActionType.SideJump);
        
        pauseInput = true;
        lane += direction;
        if (AudioManager.Instance)
            AudioManager.Instance.PlaySFX("Slide");

        // The target hop position
        float targetX = transform.position.z + (hopIncrement * direction);

        int startedLayer = lane;
        float startedY = transform.position.y;
        // The destination of the player
        Vector3 dest = new Vector3(transform.position.x, transform.position.y, targetX);

        // Lerp the palyer to it's new position
        while (Vector3.Distance(transform.position, dest) > 0.1f)
        {
            if (lane != startedLayer || transform.position.y != startedLayer)
            {
                startedY = transform.position.y;
                dest = new Vector3(transform.position.x, transform.position.y, targetX);
            }

            // move the player the next step
            float newPosition = Mathf.Lerp(transform.position.z, targetX, hopSpeed);
            rb.MovePosition(new Vector3(transform.position.x, transform.position.y, newPosition));

            yield return new WaitForFixedUpdate();
        }

        pauseInput = false;
    }



    IEnumerator Jump()
    {
        if (jumpEffect)
        {
            jumpEffect.SetVector3("WorldPos", transform.position);
            jumpEffect.Play();
        }
        else 
            Debug.Log("No jump effect assigned on player controller");
        
        // exits if alerady jumping
        if (hasJumped)
        {
            yield break;
        }
        
        animation.PlayAction(AnimationInputs.ActionType.Jump);
        animation.UpdateGrounded(false);

        // set controlling values
        hasJumped = true;
        fastFall = false;
        rb.useGravity = false;

        if (AudioManager.Instance)
            AudioManager.Instance.PlaySFX("Jump");


        
        MeshRenderer Mesh = GetComponent<MeshRenderer>();
        float targetJumpLocation = transform.position.y + (jumpHeight * Mesh.bounds.size.y);

        // the jump destination world point
        Vector3 dest = new Vector3(transform.position.x, targetJumpLocation, transform.position.z);

        int startedLayer = lane;

        while (Vector3.Distance(transform.position, dest) > 0.1f)
        {
            if (fastFall)
            {
                rb.useGravity = true;
                break;
            }

            if (lane != startedLayer)
            {
                dest = new Vector3(transform.position.x, targetJumpLocation, transform.position.z);
            }

            yield return new WaitForFixedUpdate();

            // Move the player over to the next step
            float newPosition = Mathf.Lerp(transform.position.y, targetJumpLocation, jumpSpeed);
            rb.MovePosition(new Vector3(transform.position.x, newPosition, transform.position.z));
        }

        // how long to hang in air
        if (!fastFall)
            yield return new WaitForSeconds(jumpHangTime);

        rb.useGravity = true;
    }


    IEnumerator Slide()
    {
        animation.PlayAction(AnimationInputs.ActionType.Roll);
        
        if (AudioManager.Instance)
            AudioManager.Instance.PlaySFX("Slide");
        rb.AddForce(Vector3.down * fastFallMultiplier);
        fastFall = true;

        Vector3 dest = new Vector3(transform.position.x, transform.position.y - (col.height / 4), transform.position.z);

        int startedLayer = lane;
        while (Vector3.Distance(transform.position, dest) > 0.1f)
        {
            if (lane != startedLayer)
            {
                dest = new Vector3(transform.position.x, transform.position.y - (col.height / 4), transform.position.z);
            }

            if (hasJumped)
            {
                break;
            }

            transform.position = Vector3.Lerp(transform.position, dest, slideTransitionTime * Time.deltaTime);
        }

        //col.height /= 4;
        isSliding = true;
        yield return new WaitForSeconds(slideDuration);
        isSliding = false;
        //col.height *= 4;
        
        dest = new Vector3(transform.position.x, transform.position.y + (col.height / 4), transform.position.z);
        startedLayer = lane;
        while (Vector3.Distance(transform.position, dest) > 0.1f)
        {
            if (lane != startedLayer)
            {
                dest = new Vector3(transform.position.x, transform.position.y + (col.height / 4), transform.position.z);
            }
            if (hasJumped)
            {
                break;
            }

            transform.position = Vector3.Lerp(transform.position, dest, slideTransitionTime * Time.deltaTime);
        }

        if (!hasJumped)
            rb.AddForce(Vector3.up);

        fastFall = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // destrpy tje obstacle
            Destroy(gameObject);
            SceneManager.LoadScene("GameOver");
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        hasJumped = false;
        pauseInput = false;
        fastFall = false;

        animation.UpdateGrounded(true);
    }


    public bool GetAlive()
    {
        return isAlive;
    }
}
