using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class ArrayClass
{
    public int[] actionArray;
}

public class IAModule : MonoBehaviour {

    public GridMakerScript grid_ref;
    public PlayerScript p1_script;
    public PlayerScript p2_script;

    public float danger_trigger;

    public float total_time_in_danger;

    public float total_match_time;

    public int p1_blockID;
    public int p2_blockID;

    public ArrayClass[] q_Matrix;
    public ArrayClass[] r_Matrix;

    public float[][] qMatrix;
    public float[][] rMatrix;

    public float learning_rate;
    public float gamma;
    public float e = 1; // Initial epsilon value for random action selection.
    public float eMin = 0.1f; // Lower bound of epsilon.
    public int annealingSteps = 2000; // Number of steps to lower e to eMin.
    public int lastState;


    // Use this for initialization
    void Start () {

        q_Matrix = new ArrayClass[3];
        r_Matrix = new ArrayClass[3];


        grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridMakerScript>();

        p1_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        p2_script = GameObject.FindGameObjectWithTag("Player 2").GetComponent<PlayerScript>();
    }
	
    public void loadMatrixFromFile(bool q, bool r)
    {



    }

    void OnLevelWasLoaded()
    {

        if (SceneManager.GetActiveScene().name == "Menu") return;


        grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridMakerScript>();

        p1_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        p2_script = GameObject.FindGameObjectWithTag("Player 2").GetComponent<PlayerScript>();
    }


        // Update is called once per frame
        void Update () {
       
    }
}
