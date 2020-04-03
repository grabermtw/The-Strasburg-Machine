using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningManager : MonoBehaviour
{
    public GameObject pitcher;
    private GameObject currentPitcher;
    int count =0;

    // Start is called before the first frame update
    void Start()
    {
        currentPitcher = GameObject.FindWithTag("Pitcher");
        AssignParemeterValues();
    }

    // This is called by the Floor script on the Floor GameObject whenever the ball hits the floor
    public void BallHitGround(float distance)
    {
        Debug.Log("AJ threw the ball " + distance + " meters. Good for him!");
        Debug.Log(count);
        count++;
        // Replace the current pitcher with a newly refreshed pitcher
        Destroy(currentPitcher);
        currentPitcher = Instantiate(pitcher);

        AssignParemeterValues();
    }

    // Here is where we inform the pitcher of the new values for the parameters
    private void AssignParemeterValues()
    {
        LimbManagerJoints pitcherLimbs = currentPitcher.GetComponent<LimbManagerJoints>();

        // Initialize our array to the correct number of joints
        int numJoints = pitcherLimbs.GetNumberOfJoints();
        Vector3[] torques = new Vector3[numJoints];

        // Fill our array with the values for the torque to be applied
        // For now this will just be random
        for(int i = 0; i < torques.Length; i++)
        {
            torques[i] = new Vector3(Random.Range(-100,100),Random.Range(-100,100),Random.Range(-100,100));
        }
        // Get the number of frames before the pitch is thrown
        int frames = (int) Random.Range(0,100);

        // Inform the pitcher
        pitcherLimbs.SetInputs(frames, torques);
    }

}
