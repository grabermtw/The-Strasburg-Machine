using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private LearningManager manager;

    private int count;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<LearningManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Handle when the ball hits the ground
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ball") && other.gameObject.GetComponent<FixedJoint>() == null)
        {
            // float distance = other.gameObject.transform.position.z;
            Vector3 ballPosition = other.gameObject.transform.position;
            int ballID = other.gameObject.GetComponent<Ball>().ballID;
            // Get rid of this ball
            Destroy(other.gameObject);

            // Inform the manager of the distance that the ball traveled in the z direction            
            manager.BallHitGround(ballID, ballPosition);
        }
    }

    // For the unlikely event that AJ manages to place the ball on the ground and then let go of it
    // (this has happened)
    void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Ball") && other.gameObject.GetComponent<FixedJoint>() == null)
        {
            // float distance = other.gameObject.transform.position.z;
            Vector3 ballPosition = other.gameObject.transform.position;
            int ballID = other.gameObject.GetComponent<Ball>().ballID;
            // Get rid of this ball
            Destroy(other.gameObject);

            // Inform the manager of the distance that the ball traveled in the z direction            
            manager.BallHitGround(ballID, ballPosition);
        }
    }
}
