using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartController : MonoBehaviour
{

    [Header("Car Physics")]
    public Rigidbody theRB;
    public float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 180f, gravityForce = 10f, dragOnGround = 3f;
    private float speedInput, turnInput;


    [Header("Grounded Values")]
    private bool grounded;
    public LayerMask whatIsGround;
    public float groundRayLength = .5f;
    public Transform groundRayPoint;


    [Header("Wheels")]
    public Transform leftFrontWheel,rightFrontWheel, backWheel;
    public float maxWheelTurn = 25f;
    public float wheelForwardMax;
    public ParticleSystem[] dustTrail;
    public float maxEmission = 25f;
    private float emissionRate;

    void Start()
    {
        theRB.transform.parent = null;

    }

    private void Update()
    {
        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }


        //turns the cars model with the car
        turnInput = Input.GetAxis("Horizontal");
        if (grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }

        //rotates the wheels left and right
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), leftFrontWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);

        
        transform.position = theRB.transform.position;
    }

    private void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;


        //Checks the distance from the transform "ray point" to the ground and determines whether its on the ground to apply force down
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            //changes the normal to whatever the rotation of the slope you're hitting
            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        emissionRate = 0f;

        if (grounded)
        {
            theRB.drag = dragOnGround;
            if (Mathf.Abs(speedInput) > 0)
            {
                theRB.AddForce(transform.forward * -speedInput);

                emissionRate = maxEmission;
            }
        }
        else // pushes car downwards
        {
            theRB.drag = 0.1f;
            theRB.AddForce(Vector3.up * -gravityForce * 100f);
        }


        //Controlling Amount of PArticles
        foreach(ParticleSystem part in dustTrail)
        {
            var emissionModule = part.emission;
            emissionModule.rateOverTime = emissionRate;
        }
    }
}
