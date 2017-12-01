using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class IA_Action
{
    public int number_of_blocks; // 0 - 10
    public int distance; // 1 - 4
    public bool isFilled;

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

public enum IAStates
{
    White, //100%
    Green, // 99% - 66%
    Yellow,// 66% - 33%
    Red // 33% - 0%
}

public class IAModule : MonoBehaviour
{
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

    public bool learningModeOn;
    public float learning_rate;
    public float gamma;
    public float e = 1; // Initial epsilon value for random action selection.
    public float eMin = 0.1f; // Lower bound of epsilon.
    public int annealingSteps = 2000; // Number of steps to lower e to eMin.
    public int lastState;
    public IAStates currentState;
    public bool isCurrentPlayer1;
    public int playerLastSafeBlockDistance;
    public int playercurrentSafeBlockDistance;

    public IA_Action[] actionTable = new IA_Action[80];

    public utils.MatrixData r_Matrix = new utils.MatrixData(4, 80); // (colunas, linhas)
    public utils.MatrixData q_Matrix = new utils.MatrixData(4, 80); // (colunas, linhas)

    // Use this for initialization
    private void Start()
    {
        learningModeOn = true;

        grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridMakerScript>();

        p1_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        p2_script = GameObject.FindGameObjectWithTag("Player 2").GetComponent<PlayerScript>();

        // linha 10 da coluna 1
        r_Matrix.rows[10].row[1] = 20;

        initializeActions();
        printAction(37);
        initializeZeroMatrix(true, true);
    }

    //get the state of the current player
    public IAStates getCurrentState()
    {
        IAStates state = IAStates.White;

        if (isCurrentPlayer1)
        {
            state = p1_script.currentState;
        }
        else
        {
            state = p2_script.currentState;
        }

        return state;
    }

    public void setCurrentState()
    {
        IAStates state = IAStates.White;

        if (isCurrentPlayer1)
        {
            state = p1_script.currentState;
        }
        else
        {
            state = p2_script.currentState;
        }

        currentState = state;
    }

    //get the current player to focus aka dying player

    public void setCurrentPlayer()
    {
        if (p1_script.deathTimer <= p2_script.deathTimer)
        {
            isCurrentPlayer1 = true;
        }
        else
        {
            isCurrentPlayer1 = false;
        }
    }

    public void printAction(int idx)
    {
        print("Action[" + idx + "] = (" + actionTable[idx].number_of_blocks + ", " + actionTable[idx].distance + ", " + actionTable[idx].isFilled + ")");
    }

    //At = [1-10,1-4, true-false]
    public void initializeActions()
    {
        int[] count_blocks = new int[10];
        for (int i = 0; i < 10; i++)
            count_blocks[i] = 0;

        int[] count_distance = new int[4];
        for (int i = 0; i < 4; i++)
            count_distance[i] = 0;

        int[] count_filled = new int[2];
        for (int i = 0; i < 2; i++)
            count_filled[i] = 0;

        //set first variable
        for (int i = 0; i < 80; i++)
        {
            if (actionTable[i] == null) actionTable[i] = new IA_Action(0, 0, true);

            //first variables
            if (count_blocks[0] < 8)
            {
                actionTable[i].number_of_blocks = 1;
                count_blocks[0]++;
            }
            else if (count_blocks[1] < 8)
            {
                actionTable[i].number_of_blocks = 2;
                count_blocks[1]++;
            }
            else if (count_blocks[2] < 8)
            {
                actionTable[i].number_of_blocks = 3;
                count_blocks[2]++;
            }
            else if (count_blocks[3] < 8)
            {
                actionTable[i].number_of_blocks = 4;
                count_blocks[3]++;
            }
            else if (count_blocks[4] < 8)
            {
                actionTable[i].number_of_blocks = 5;
                count_blocks[4]++;
            }
            else if (count_blocks[5] < 8)
            {
                actionTable[i].number_of_blocks = 6;
                count_blocks[5]++;
            }
            else if (count_blocks[6] < 8)
            {
                actionTable[i].number_of_blocks = 7;
                count_blocks[6]++;
            }
            else if (count_blocks[7] < 8)
            {
                actionTable[i].number_of_blocks = 8;
                count_blocks[7]++;
            }
            else if (count_blocks[8] < 8)
            {
                actionTable[i].number_of_blocks = 9;
                count_blocks[8]++;
            }
            else if (count_blocks[9] < 8)
            {
                actionTable[i].number_of_blocks = 10;
                count_blocks[9]++;
            }

            //second variables
            if (count_distance[0] < 20)
            {
                actionTable[i].distance = 1;
                count_distance[0]++;
            }
            else if (count_distance[1] < 20)
            {
                actionTable[i].distance = 2;
                count_distance[1]++;
            }
            else if (count_distance[2] < 20)
            {
                actionTable[i].distance = 3;
                count_distance[2]++;
            }
            else if (count_distance[3] < 20)
            {
                actionTable[i].distance = 4;
                count_distance[3]++;
            }

            //third variable
            if (count_filled[0] < 40)
            {
                actionTable[i].isFilled = true;
                count_filled[0]++;
            }
            else if (count_filled[1] < 40)
            {
                actionTable[i].isFilled = false;
                count_filled[1]++;
            }
        }
    }

    public void loadMatrixFromFile(bool q, bool r)
    {
    }

    public void initializeZeroMatrix(bool q, bool r)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                if (q)
                {
                    q_Matrix.rows[j].row[i] = 0;
                }

                if (r)
                {
                    r_Matrix.rows[j].row[i] = 0;
                }
            }
        }
    }

    public void initalizeZeroMaxiumSizeMatrix(bool q, bool r)
    {
    }

    private void OnLevelWasLoaded()
    {
        if (SceneManager.GetActiveScene().name == "Menu") return;

        grid_ref = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridMakerScript>();

        p1_script = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        p2_script = GameObject.FindGameObjectWithTag("Player 2").GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        setCurrentPlayer();
        setCurrentState();
    }
}