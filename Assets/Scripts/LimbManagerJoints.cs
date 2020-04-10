using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbManagerJoints : MonoBehaviour
{
    // Array of all the joints we'll be using
    // Make this public so that we can decide which joints we want to include in the inspector
    public Rigidbody[] joints;
    public GameObject throwingHand;
    public bool useGravity; // Should the limbs be using gravity in their rigidbodies?
    private Transform[] fingers; // The transforms of the throwing hand's fingers
    private GameObject ball; // Reference for the ball we're throwing
    private Vector3[] torqueInputs; // Array of inputs for the torque
    private int releaseFrameInput; // The frame at which the ball should be released
    private int currentFrame; // Counted up until we reach releaseFrameInput

    private bool released = false;


    // Start is called before the first frame update
    void Start()
    {
        // Get the ball's transform
        ball = transform.Find("Ball").gameObject;
        ball.transform.SetParent(null);

        // Get the fingers
        fingers = throwingHand.GetComponentsInChildren<Transform>();

        currentFrame = 0;

        // Immediately prepare each joint to be manipulated
        foreach(Rigidbody joint in joints)
        {
            joint.isKinematic = false;
            if(!useGravity)
            {
                joint.useGravity = false;
            }
        }
    }


    // This opens the hand to release the ball
    private void Release()
    {
        // Open the hand
        foreach (Transform finger in fingers)
        {
            if (finger != throwingHand.transform)
            {
                finger.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
        }

        // Release the ball
        Destroy(ball.GetComponent<FixedJoint>());
    }

    void FixedUpdate()
    {
        // Is it time to throw the ball?
        if (currentFrame < releaseFrameInput)
        {
            // It is not time to throw the ball.
            // We'll add more torque to our limbs instead.
            for (int i = 0; i < joints.Length; i++)
            {
                joints[i].AddRelativeTorque(torqueInputs[i]);
            }
            currentFrame++;
        }
        else if (!released)
        {
            // It is time to throw the ball!
            released = true;
            Release();
        }
    }

    // Returns the number of joints in use
    public int GetNumberOfJoints()
    {
        return joints.Length;
    }

    public void SetInputs(int releaseFrame, Vector3[] torques)
    {
        releaseFrameInput = releaseFrame;
        torqueInputs = torques;
    }

}
