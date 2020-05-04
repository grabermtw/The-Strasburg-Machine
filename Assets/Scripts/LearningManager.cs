using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class LearningManager : MonoBehaviour
{
    int NUM_AJS;
    // const int NUM_JOINTS = 10;
    /* We'll get the number of joints from the AJ prefab
    rather than define it here so that we can easily change the number of joints
    */
    int NUM_THROWS;
    int PARENTS_TO_KEEP;
    float CROSSOVER_PROBABILITY;
    float MUTATION_PROBABILITY;

    public GameObject pitcher;
    public Material greenShirt;
    int generation = 0;
    int generationProgress = 0;
    float avgFitness = 0;
    float maxFitness = 0;

    private GameObject[] AJs;
    private AJData[] AJDatas;
    private int throwNum = 0;
    private float[][] scores;
    private int num_joints;

    private List<string[]> rowData = new List<String[]>();

    // Start is called before the first frame update
    void Start()
    {
        NUM_AJS = PlayerPrefs.GetInt("numAjs");
        NUM_THROWS = PlayerPrefs.GetInt("numThrows");
        PARENTS_TO_KEEP = (int)(NUM_AJS * PlayerPrefs.GetFloat("parentsToKeep"));
        CROSSOVER_PROBABILITY = PlayerPrefs.GetFloat("crossoverProbability");
        MUTATION_PROBABILITY = PlayerPrefs.GetFloat("mutationProbability");
        AJs = new GameObject[NUM_AJS];
        AJDatas = new AJData[NUM_AJS];
        scores = new float[NUM_AJS][];
        // Get the number of joints this pitcher has
        num_joints = pitcher.GetComponent<LimbManagerJoints>().GetNumberOfJoints();

        InitializePopulation();
        // initialize jagged scores array
        for (int i=0; i<NUM_AJS; i++)
        {
            scores[i] = new float[NUM_THROWS];
        }

        // create first row of titles for csv file
        string[] rowDataTitle = new string[3];
        rowDataTitle[0] = "Generation #";
        rowDataTitle[1] = "Max Fitness";
        rowDataTitle[2] = "Average Fitness";
        rowData.Add(rowDataTitle);
    }

    void OnApplicationQuit()
    {
        // write data to csv file
        string[][] output = new string[rowData.Count][];

        for (int i = 0; i < output.Length; i++)
        {
            output[i] = rowData[i];
        }

        int length = output.GetLength(0);
        string delimiter = ",";

        StringBuilder sb = new StringBuilder();

        for (int index = 0; index < length; index++)
            sb.AppendLine(string.Join(delimiter, output[index]));

        StreamWriter outStream = System.IO.File.CreateText(Application.dataPath + "/" + "data.csv");
        outStream.WriteLine(sb);
        outStream.Close();
    }

    // This is called by the Floor script on the Floor GameObject whenever the ball hits the floor
    public void BallHitGround(int ballID, Vector3 ballPosition)
    {
        GameObject AJ = AJs[ballID];
        //euclidean distance
        //float distance = Vector3.Distance(AJ.transform.position, ballPosition);

        //z-coordinate distance, this is the distance in only the direction we want him to throw
        float distance = Math.Max(0, ballPosition.z); //- AJ.transform.position.z);
        // Debug.Log("AJ " + ballID + " threw the ball " + distance + " meters. Good for him!");
        
        //AJDatas[ballID].fitness = Math.Max(AJDatas[ballID].fitness, distance);
        scores[ballID][throwNum] = distance;
        generationProgress++;
        avgFitness += distance;
        maxFitness = Math.Max(distance, maxFitness);

        string[] rowDataTemp = new string[3];
        
        // Set is done running
        if (generationProgress >= NUM_AJS) {
            // Generation is done running
            //FitnessPrintStatement();
            if (throwNum >= NUM_THROWS - 1)
            {
                // Evaluate fitness
                // currently using median value as fitness
                for (int i=0; i<NUM_AJS; i++)
                {
                    Array.Sort(scores[i]);
                    // different case for odd and even number of throws
                    AJDatas[i].fitness = NUM_THROWS % 2 == 1 ? scores[i][NUM_THROWS/2] : (scores[i][NUM_THROWS/2 - 1] + scores[i][NUM_THROWS/2]) / 2;
                    // might as well clean scores while we're here
                    for (int j=0; j<NUM_THROWS; j++)
                    {
                        scores[i][j] = 0;
                    }
                }
                Debug.Log("Generation " + generation + ": max fitness was "+maxFitness+", average fitness was "+(avgFitness/(NUM_AJS*NUM_THROWS)));
                // Adding data to be put in csv file
                rowDataTemp[0] = generation.ToString();
                rowDataTemp[1] = maxFitness.ToString();
                rowDataTemp[2] = (avgFitness / (NUM_AJS * NUM_THROWS)).ToString();
                rowData.Add(rowDataTemp);

                generationProgress = 0;
                avgFitness = 0;
                maxFitness = 0;
                generation++;
                throwNum = 0;
                
                CreateNewGeneration();

                for (int i = 0; i < NUM_AJS; i++) 
                {
                    Destroy(AJs[i]);
                    AJs[i] = Instantiate(pitcher, new Vector3(i * 2, 0, 0), Quaternion.identity);
                    LimbManagerJoints pitcherLimbs = AJs[i].GetComponent<LimbManagerJoints>();
                    pitcherLimbs.SetInputs(AJDatas[i].releaseFrame, AJDatas[i].torques);
                    AJs[i].transform.Find("Ball").GetComponent<Ball>().ballID = i;
                }
                //update display
                Display.UpdateText(generation, throwNum);
            }
            // else move onto next set of throws
            else
            {
                generationProgress = 0;
                throwNum++;
                for (int i = 0; i < NUM_AJS; i++) {
                    Destroy(AJs[i]);

                    AJs[i] = Instantiate(pitcher, new Vector3(i * 2, 0, 0), Quaternion.identity);
                    
                    LimbManagerJoints pitcherLimbs = AJs[i].GetComponent<LimbManagerJoints>();
                    pitcherLimbs.SetInputs(AJDatas[i].releaseFrame, AJDatas[i].torques);
                    AJs[i].transform.Find("Ball").GetComponent<Ball>().ballID = i;
                }
                //update display
                Display.UpdateText(generation, throwNum);
            }
            if (generation > 0)
            {
                // Update shirt color for the top % to show they have remained from the previous generation
                for (int i = NUM_AJS - PARENTS_TO_KEEP; i < NUM_AJS; i++)
                {
                    AJs[i].transform.GetChild(8).GetComponent<Renderer>().material = greenShirt;
                }
            }
        }
    }

    private AJData CreateRandomAJ(int id) {
        Vector3[] torques = new Vector3[num_joints];

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
            
            // determine if we will mutate
            r = UnityEngine.Random.Range(0, 1);
            if (r <= MUTATION_PROBABILITY) {
                // mutate the child
                child = Mutate(child);
            }
            
            // put the new child in some temp array to be used in the next generation
            ret[i] = child;
        }

        Array.Sort(AJDatas);
        //FitnessPrintStatement()
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
        //Debug.Log("sumfitness: "+sumFitness);
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
        Vector3[] torques = new Vector3[num_joints];

        for (int i = 0; i < num_joints; i++) {            
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
        int random = (int)(UnityEngine.Random.Range(0, num_joints + 1));

        if (random == num_joints) {
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

    private void FitnessPrintStatement()
    {
        string str = "";
        for (int i = 0; i < AJDatas.Length; i++)
        {
            str += scores[i][throwNum].ToString("F3") + " ";
        }
        Debug.Log(str);
    }

    public class AJData : IComparable {
        public Vector3[] torques;
        public int releaseFrame;
        public float fitness;
        public int id;
        public static int nextId = 0;

        public AJData(Vector3[] torques, int releaseFrame) {
            this.releaseFrame = releaseFrame;
            this.torques = torques;
            this.id = nextId;
            nextId++;
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
