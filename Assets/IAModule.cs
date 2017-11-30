using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System;

[System.Serializable]
public class IA_Action 
{
    int number_of_blocks; // 0 - 10
    int distance; // 1 - 4
    bool isFilled;

    public static bool operator ==(IA_Action obj1, IA_Action obj2)
    {
        if (ReferenceEquals(obj1, obj2))
        {
            return true;
        }

        if (ReferenceEquals(obj1, null))
        {
            return false;
        }
        if (ReferenceEquals(obj2, null))
        {
            return false;
        }

        return (obj1.number_of_blocks == obj2.number_of_blocks
                && obj1.distance == obj2.distance
                && obj1.isFilled == obj2.isFilled);
    }

    // this is second one '!='
    public static bool operator !=(IA_Action obj1, IA_Action obj2)
    {
        return !(obj1 == obj2);
    }

    public IA_Action(int n_blocks, int dis, bool filled)
    {
        if (n_blocks < 0) n_blocks = 0;
        if (n_blocks > 10) n_blocks = 10;

        if (distance < 1) distance = 1;
        if (distance > 4) distance = 4;

        number_of_blocks = n_blocks; // 0 - 10
        distance = dis; // 1 - 4
         isFilled = filled;
    }
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


    

    public float[][] qMatrix;
    public float[][] rMatrix;

    public float learning_rate;
    public float gamma;
    public float e = 1; // Initial epsilon value for random action selection.
    public float eMin = 0.1f; // Lower bound of epsilon.
    public int annealingSteps = 2000; // Number of steps to lower e to eMin.
    public int lastState;


    public utils.MatrixData r_Matrix = new utils.MatrixData(4, 80); // (colunas, linhas)
    public utils.MatrixData q_Matrix = new utils.MatrixData(4, 80); // (colunas, linhas)

    private IA_Action[,] actionTable = new IA_Action[4,80];

    // Use this for initialization
    void Start () {

      


        grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridMakerScript>();

        p1_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        p2_script = GameObject.FindGameObjectWithTag("Player 2").GetComponent<PlayerScript>();

        // linha 10 da coluna 1
        r_Matrix.rows[10].row[1] = 20;

        initializeZeroMatrix(true, true);
    }
	



    public void loadMatrixFromFile(bool q, bool r)
    {



    }

    public void initializeZeroMatrix(bool q, bool r)
    {
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < 80; j++)
            {

                if(q)
                {
                    q_Matrix.rows[j].row[i] = 0;
                }

                if(r)
                {
                    
                    r_Matrix.rows[j].row[i] = 0;
                }
            }
        }
    }

    public void initalizeZeroMaxiumSizeMatrix(bool q, bool r)
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
