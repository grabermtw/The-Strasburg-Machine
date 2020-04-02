using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimbManager : MonoBehaviour
{
    // Array of all the joints we'll be using
    // Make this public so that we can decide which joints we want to include in the inspector
    public Rigidbody[] joints;
    public GameObject throwingHand;
    private GameObject grasp; // The GameObject that has the hand's box colliders
    private Rigidbody[] fingers; // The transforms of the throwing hand's fingers
    private GameObject ball; // Reference for the ball we're throwing

    // Start is called before the first frame update
    void Start()
    {
        grasp = throwingHand.transform.Find("GraspColliders").gameObject;

        // Get the ball's transform
        ball = GameObject.FindWithTag("Ball");
        ball.transform.SetParent(null);

        // Get the fingers
        fingers = throwingHand.GetComponentsInChildren<Rigidbody>();

        

    }


    // This opens the hand to release the ball
    private void Release()
    {
        foreach (Rigidbody finger in fingers)
        {
            if (finger != throwingHand.GetComponent<Rigidbody>())
            {
                //finger.rotation = Quaternion.Euler(0, 0, 0);
                //finger.MoveRotation(Quaternion.Euler(0, 0, 0));
                finger.gameObject.transform.localEulerAngles = new Vector3(0,0,0);
            }
        }
        grasp.layer = 8;
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
            joint.AddRelativeTorque(new Vector3(Random.Range(-10,10), Random.Range(-10,10), Random.Range(-10,10)));
           // joint.MoveRotation(Quaternion.Euler(joint.rotation.eulerAngles + new Vector3(Random.Range(-10,10), Random.Range(-10,10), Random.Range(-10,10))));
        }

    }

}
