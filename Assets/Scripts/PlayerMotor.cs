using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    //
    private bool isRunning = false;

    // Animation
    private Animator anim;

    // Movement
    private CharacterController controller;
    private float jumpForce = 4.0f; //4.0f
    private float gravity = 12.0f;
    private float verticalVelocity;
    private int desiredLane = 1; // 0 = Left, 1 = Middle, 2 = Right

    // Speed Modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedInceaseLastTick;
    private float speedIncreaseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;

    private AudioManager audioManager;

    // Start is called before the first frame update
    private void Start()
    {
        speed = originalSpeed;
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!isRunning)
            return;

        if (Time.time - speedInceaseLastTick > speedIncreaseTime)
        {
            speedInceaseLastTick = Time.time;
            speed += speedIncreaseAmount;

            // Change the modifier text
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }

        // Gather the inputs on where we should be
        if (MobileInput.Instance.SwipeLeft)
            MoveLane(false);

        if (MobileInput.Instance.SwipeRight)
            MoveLane(true);
        /*
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLane(false);

        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveLane(true);*/

        // Calculate where we should be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0)
            targetPosition += Vector3.left * LANE_DISTANCE;
        if (desiredLane == 2)
            targetPosition += Vector3.right * LANE_DISTANCE;

        // Let's calculate our move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;   

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        // Calculate Y
        if (isGrounded) // If grounded
        {
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp)
            {
                // Jump
                audioManager.Play("jump");
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;

            } else if (MobileInput.Instance.SwipeDown)
            {
                // Slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        } else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            // Fast falling mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
                audioManager.Play("land");
            }
        }
        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        // Move the Pengu
        controller.Move(moveVector * Time.deltaTime);

        // Rotate the Pengu to the direction he is going
        Vector3 dir = controller.velocity;
        if (dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }

    }

    public void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z);
    }

    public void StopSliding()
    {
        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }

    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
        audioManager.Play("move");
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(
            new Vector3(controller.bounds.center.x,
                (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f,
                controller.bounds.center.z),
            Vector3.down);
        //Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return Physics.Raycast(groundRay, 0.2f + 0.1f);
    }

    public void StartRunning()
    {
        isRunning = true;
        anim.SetTrigger("StartRuning");
    }

    private void Crash()
    {
        audioManager.Play("crash");
        anim.SetTrigger("Death");
        isRunning = false;
        GameManager.Instance.OnDeath();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
         switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
             break;
        }
    }
}
