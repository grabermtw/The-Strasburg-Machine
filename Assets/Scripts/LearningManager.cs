using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearningManager : MonoBehaviour
{
    const int NUM_AJS = 10;
    const int NUM_JOINTS = 4;
    const int PARENTS_TO_KEEP = (int)(NUM_AJS * .2);
    const float CROSSOVER_PROBABILITY = 0.95f;
    const float MUTATION_PROBABILITY = 0.05f;

    public GameObject pitcher;
    int generation = 0;
    int generationProgress = 0;
    float avgFitness = 0;

    private GameObject[] AJs = new GameObject[NUM_AJS];
    private AJData[] AJDatas = new AJData[NUM_AJS];

    // Start is called before the first frame update
    void Start()
    {
        InitializePopulation();
    }

    // This is called by the Floor script on the Floor GameObject whenever the ball hits the floor
    public void BallHitGround(int ballID, Vector3 ballPosition)
    {
        GameObject AJ = AJs[ballID];
        float distance = Vector3.Distance(AJ.transform.position, ballPosition);

        // Debug.Log("AJ " + ballID + " threw the ball " + distance + " meters. Good for him!");
        
        AJDatas[ballID].fitness = distance;
        generationProgress++;
        avgFitness += distance / NUM_AJS;
        
        // Generation is done running
        if (generationProgress >= NUM_AJS) {
            generation++;
            Debug.Log("Generation " + generation + " concluded with an average fitness of " + avgFitness + ".");
            generationProgress = 0;
            avgFitness = 0;

            CreateNewGeneration();

            for (int i = 0; i < NUM_AJS; i++) {
                Destroy(AJs[i]);

                AJs[i] = Instantiate(pitcher, new Vector3(i * 2, 0, 0), Quaternion.identity);
                
                LimbManagerJoints pitcherLimbs = AJs[i].GetComponent<LimbManagerJoints>();
                pitcherLimbs.SetInputs(AJDatas[i].releaseFrame, AJDatas[i].torques);
                AJs[i].transform.Find("Ball").GetComponent<Ball>().ballID = i;
            }
        }
    }

    private AJData CreateRandomAJ(int id) {
        Vector3[] torques = new Vector3[NUM_JOINTS];

        // Fill our array with random values for each torque to be applied
        for(int i = 0; i < torques.Length; i++)
        {
            torques[i] = new Vector3(UnityEngine.Random.Range(-100,100),UnityEngine.Random.Range(-100,100),UnityEngine.Random.Range(-100,100));
        }
        // Get the number of frames before the pitch is thrown
        int frame = (int) UnityEngine.Random.Range(0,100);

        return new AJData(torques, frame);
    }

    private void InitializePopulation() {        
        for (int i = 0; i < NUM_AJS; i++) {
            AJs[i] = Instantiate(pitcher, new Vector3(i * 2, 0, 0), Quaternion.identity);
            AJDatas[i] = CreateRandomAJ(i);
     
            LimbManagerJoints pitcherLimbs = AJs[i].GetComponent<LimbManagerJoints>();
            pitcherLimbs.SetInputs(AJDatas[i].releaseFrame, AJDatas[i].torques);
            AJs[i].transform.Find("Ball").GetComponent<Ball>().ballID = i;
        }
    }

    private void CreateNewGeneration() {
        // for each AJ in population, calculate their fitness  (probly store fitness in an array parallel to
        // AJ array 		i.e. AJ[0]'s fitness is in fitness[0]
        
        // make arraylist for returning the new generation
        AJData[] ret = new AJData[NUM_AJS];
        
        // find the PARENTS_TO_KEEP top fittest individuals and move them on to the next generation
        // for the other POP_SIZE - PARENTS_TO_KEEP individuals we will need to breed
        for (int i = 0; i < NUM_AJS - PARENTS_TO_KEEP; i++) {
            // choose two parents
            AJData p1 = ChooseParent();
            AJData p2 = ChooseParent();
            AJData child;
            
            // determine if we will crossover
            double r = UnityEngine.Random.Range(0, 1);
            if (r <= CROSSOVER_PROBABILITY) {
                // crossover the two parents
                child = Crossover(p1, p2);
            } else {
                child = p1;
            }
            
            // determine if we will crossover
            r = UnityEngine.Random.Range(0, 1);
            if (r <= MUTATION_PROBABILITY) {
                // mutate the child
                child = Mutate(child);
            }
            
            // put the new child in some temp array to be used in the next generation
            ret[i] = child;
        }

        Array.Sort(AJDatas);
        for (int i = 0; i < NUM_AJS - PARENTS_TO_KEEP; i++) {
            AJDatas[i] = ret[i];
        }
    }

    private AJData ChooseParent() {
        // Using Roulette Wheel Selection for selecting parents
        float sumFitness = 0;

        for (int i = 0; i < NUM_AJS; i++) {
            sumFitness += AJDatas[i].fitness;
        }

        float r = UnityEngine.Random.Range(0, sumFitness);
        
        // Roulette-style choosing
        float partialSum = 0;
        int j = 0;
        while (partialSum < r) {
            partialSum += AJDatas[j].fitness;
            j++;
        }
        
        // chosen parent is AJ[i]
        return AJDatas[j - 1];
    }

    private AJData Crossover(AJData p1, AJData p2) {
        // single point crossover
        // -----------------------------------
        // take the first i parameters from p1 and give them to the child
        // take the i through last parameters from p2 and give them to the child
        // return new AJData(p1.torques, p2.releaseFrame);
        
        // uniform crossover
        // -----------------------------------
        // each parameter from the child is chosen by randomly choosing p1 or p2
        Vector3[] torques = new Vector3[NUM_JOINTS];

        for (int i = 0; i < NUM_JOINTS; i++) {            
            torques[i] = (UnityEngine.Random.Range(0, 1) > 0.5) ? p1.torques[i] : p2.torques[i];
        }

        return new AJData(torques, (UnityEngine.Random.Range(0, 1) > 0.5) ? p1.releaseFrame : p2.releaseFrame);
        
        // Whole Arithmetic Recombination
        // -----------------------------------
        // basically just average out the parents' weights
        // return TODO VORSTEG / BEN
    }

    private AJData Mutate(AJData c) {
        // 1) Randomize one of the parameters or
        int random = (int)(UnityEngine.Random.Range(0, NUM_JOINTS + 1));

        if (random == 4) {
            return new AJData(c.torques, (int) UnityEngine.Random.Range(0,100));
        }
        else {
            Vector3[] torques = c.torques;
            torques[random] = new Vector3(UnityEngine.Random.Range(-100,100),UnityEngine.Random.Range(-100,100),UnityEngine.Random.Range(-100,100));
            return new AJData(torques, c.releaseFrame);
        }

        // 2) Add or subtract a small number from every parameter
        // TODO VORSTEG / BEN
    }

    public class AJData : IComparable {
        public Vector3[] torques;
        public int releaseFrame;
        public float fitness;

        public AJData(Vector3[] torques, int releaseFrame) {
            this.releaseFrame = releaseFrame;
            this.torques = torques;
        }

        public int CompareTo(object obj) {
            if (obj == null) return 1;
        
            AJData otherAJ = obj as AJData;
            if (otherAJ != null) 
                return this.fitness.CompareTo(otherAJ.fitness);
            else
                throw new ArgumentException("Object is not an AJData");
        }
    }
}
