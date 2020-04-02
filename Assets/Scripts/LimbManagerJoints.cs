using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbManagerJoints : MonoBehaviour
{
    // Array of all the joints we'll be using
    // Make this public so that we can decide which joints we want to include in the inspector
    public Rigidbody[] joints;
    public GameObject throwingHand;
    private Transform[] fingers; // The transforms of the throwing hand's fingers
    private GameObject ball; // Reference for the ball we're throwing

    // Start is called before the first frame update
    void Start()
    {

        // Get the ball's transform
        ball = transform.Find("Ball").gameObject;
        ball.transform.SetParent(null);

        // Get the fingers
        fingers = throwingHand.GetComponentsInChildren<Transform>();
    }


    // This opens the hand to release the ball
    private void Release()
    {
        // Open hand
        foreach (Transform finger in fingers)
        {
            if (finger != throwingHand.transform)
            {
                finger.gameObject.transform.localEulerAngles = new Vector3(0,0,0);
            }
        }

        // Release the ball
        Destroy(ball.GetComponent<FixedJoint>());
    }

    // Update is called once per frame
    void Update()
    {
        // Press space to drop the ball (for now)
        if (Input.GetKey(KeyCode.Space))
        {
            Release();
        }

    }

    void FixedUpdate()
    {
        foreach (Rigidbody joint in joints)
        {
            joint.AddRelativeTorque(new Vector3(Random.Range(-50,50), Random.Range(-50,50), Random.Range(-50,50)));
        }

    }

}
