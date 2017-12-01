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
    public PlayerScript currentPlayer;

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
    public IAStates lastState;
    public int lastActionIndex;
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
        initializeMatrixWithValue(true, true,1);

        setCurrentPlayer();
        setCurrentState();
    }

    // Update is called once per frame
    private void Update()
    {
        setCurrentPlayer();
        setCurrentState();

        if (getLowestRValue() >= 1 && getMaxRValue() > 100) normalizeRMatrix();

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
        playercurrentSafeBlockDistance = currentPlayer.safeBlockDistance;
    }

    //get the current player to focus aka dying player

    public void setCurrentPlayer()
    {
        if (p1_script.deathTimer <= p2_script.deathTimer)
        {
            isCurrentPlayer1 = true;
            currentPlayer = p1_script;
        }
        else
        {
            isCurrentPlayer1 = false;
            currentPlayer = p2_script;
        }
    }

    public void printAction(int idx)
    {
        print("Action[" + idx + "] = (" + actionTable[idx].number_of_blocks + ", " + actionTable[idx].distance + ", " + actionTable[idx].isFilled + ")");
    }

    public void printAction(IA_Action ac)
    {
        print("Action[" + getActionIndex(ac) + "] = (" + ac.number_of_blocks + ", " + ac.distance + ", " + ac.isFilled + ")");
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

    public void initializeMatrixWithValue(bool q, bool r, int value)
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                if (q)
                {
                    q_Matrix.rows[j].row[i] = value;
                }

                if (r)
                {
                    r_Matrix.rows[j].row[i] = value;
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

        p1_script.Juke_Ref.isFirstMovement = true; // nova partida reseta essa variavel
    }

    //seleciona ação dado um estado
    public int pickActionIndex(IAStates st)
    {
        if(learningModeOn)
        {
            if(getZeroPercentageInRMatrix() > 5)
            {
                //chance de pegar uma ação random
                if(Random.Range(0,100) <= getZeroPercentageInRMatrix())
                {
                    return Random.Range(0, 80);
                }
                else // senão pega a melhor ação
                {
                    return getHighestRewardAction(st);
                }
            }
            else
            {
                if (e > eMin) { e = e - ((1f - eMin) / (float)annealingSteps); }
                if (Random.Range(0f, 1f) < e)
                { 
                
                    return Random.Range(0, 80);
                }
                else // senão pega a melhor ação
                {
                    return getHighestRewardAction(st);
                }
            }
        }
        else
        {
            //seleciona a melhor ação na matriz Q
            return 0;
        }
    }

    //seleciona ação dado um estado
    public IA_Action pickAction(IAStates st)
    {
        if (learningModeOn)
        {
            if (getZeroPercentageInRMatrix() > 5)
            {
                //chance de pegar uma ação random
                if (Random.Range(0, 100) <= getZeroPercentageInRMatrix())
                {
                    return actionTable[Random.Range(0, 80)];
                    
                }
                else // senão pega a melhor ação
                {
                    return actionTable[getHighestRewardAction(st)];
                }
            }
            else
            {
                if (e > eMin) { e = e - ((1f - eMin) / (float)annealingSteps); }
                if (Random.Range(0f, 1f) < e)
                {

                    return actionTable[Random.Range(0, 80)];
                }
                else // senão pega a melhor ação
                {
                    return actionTable[getHighestRewardAction(st)];
                }
            }
        }
        else
        {
            //seleciona a melhor ação na matriz Q
            return actionTable[0];
        }
    }

    // Realiza a ação selecionada.
    //Guardando os valores de último estado, última ação e distância do jogador
    //do Bloco Seguro.
    public void doAction(int index)
    {

        lastState = currentState;
        lastActionIndex = index;
        playerLastSafeBlockDistance = currentPlayer.safeBlockDistance;
        grid_ref.moveRangedAreaFrom(actionTable[index].number_of_blocks * 10, // de 10 em 10%
            actionTable[index].distance,
            actionTable[index].isFilled);

        
    }

    public void doAction(IA_Action ac)
    {
        lastState = currentState;
        lastActionIndex = getActionIndex(ac);
        playerLastSafeBlockDistance = currentPlayer.safeBlockDistance;
        grid_ref.moveRangedAreaFrom(ac.number_of_blocks * 10,
            ac.distance,
            ac.isFilled);
    }


    public void Rewardify()
    {
        // caso o estado do player diminuia um nivel
        if ((lastState == IAStates.White && currentState == IAStates.Green) || (lastState == IAStates.Green && currentState == IAStates.Yellow))
        {
            addRValue(lastState, lastActionIndex, 50);
        }//caso player desça um pouco
        else if (playerLastSafeBlockDistance > currentPlayer.safeBlockDistance)
        {
            addRValue(lastState, lastActionIndex, 10);
        }// caso não mude de estado
        else if ((lastState == IAStates.White && currentState == IAStates.White) || (lastState == IAStates.Green && currentState == IAStates.Green))
        {
            if (playerLastSafeBlockDistance <= currentPlayer.safeBlockDistance)
            {
                addRValue(lastState, lastActionIndex, -5);
            }
        }

        // second stage of rewardify
        //caso continue no amarelo
        else if (lastState == IAStates.Yellow && currentState == IAStates.Yellow)
        {
            //caso n mude distancia do jogador para o safe block
            if(playerLastSafeBlockDistance == currentPlayer.safeBlockDistance)
            {
                addRValue(lastState, lastActionIndex, 30);
            }
            else
            {//caso mude mas continue no mesmo estado
                addRValue(lastState, lastActionIndex, 10);
            }
        }
        
        // third stage
        //caso saida do estagio vermelho
        else if (lastState == IAStates.Red && currentState == IAStates.White)
        {
            addRValue(lastState, lastActionIndex, 40);
        }
        else if(lastState == IAStates.Red && currentState == IAStates.Red)
        {
            if(playerLastSafeBlockDistance < currentPlayer.safeBlockDistance)
            {
                addRValue(lastState, lastActionIndex, 20);
            }
            else 
            {
                addRValue(lastState, lastActionIndex, -10);
            }
            
        }


        // fourth stage done outside here

        /*
            Rewardify:
            Caso o estado anterior seja Branco ou Verde, o valor R(último estado, última ação)
            ganha + 50 caso o estado atual seja agora um nível abaixo do anterior.Recebe - 5 caso a ação não cause mudança de estado. + 10 caso a distancia do safe block aumente

Caso o estado anterior seja Amarelo, o valor R(último estado, última ação) ganha + 10 caso o estado atual continue sendo amarelo.

Caso o estado anterior seja Vermelho, o valor R(último estado, última ação)
ganha + 40 caso o estado atual seja agora Branco.Ganha + 20 caso a distância do jogador do Bloco Seguro tenha diminuído
Recebe - 10 caso não cause nenhuma dessas mudanças.

A última ação realizada antes de um término de partida terá sua nota diminuída em - 30.

*/

    }


    public void setRValue(IAStates st, int action_idx, int value)
    {
        // linha 10 da coluna 1
        r_Matrix.rows[action_idx].row[(int)st] = value;
        normalizeRMatrix();
    }

    public void addRValue(IAStates st, int action_idx, int value)
    {
        // linha 10 da coluna 1
        r_Matrix.rows[action_idx].row[(int)st] += value;
        if (r_Matrix.rows[action_idx].row[(int)st] < 1) r_Matrix.rows[action_idx].row[(int)st] = 1;
        //normalizeRMatrix();
    }

    public int getRValue(IAStates st, int action_idx)
    {
        // linha 10 da coluna 1
        return r_Matrix.rows[action_idx].row[(int)st];
        
    }

    public int getMaxRValue()
    {
        int highest_value = 0;
        
        for (int i = 0; i < 4; i++) // estados / columns
        {
            for (int j = 0; j < 80; j++) // ações / rows
            {
               
                    //caso essa ação seja a maior, escolha ela
                    if (r_Matrix.rows[j].row[i] > highest_value)
                    {
                        highest_value = r_Matrix.rows[j].row[i];
                        
                    }
                


            }
        }
        return highest_value;
    }

    public int getLowestRValue()
    {
        int lowest_value = 0;

        for (int i = 0; i < 4; i++) // estados / columns
        {
            for (int j = 0; j < 80; j++) // ações / rows
            {

                //caso essa ação seja a maior, escolha ela
                if (r_Matrix.rows[j].row[i] < lowest_value)
                {
                    lowest_value = r_Matrix.rows[j].row[i];

                }



            }
        }
        return lowest_value;
    }



    public void normalizeRMatrix()
    {
        for (int i = 0; i < 4; i++) // estados / columns
        {
            for (int j = 0; j < 80; j++) // ações / rows
            {

                //caso essa ação seja a maior, escolha ela

                r_Matrix.rows[j].row[i] /= getMaxRValue();
                



            }
        }
    }


    
    int getActionIndex(IA_Action ac)
    {

        for(int i = 0; i < 80; i++)
        {
            //caso essa ação seja a maior, escolha ela
            if (actionTable[i] == ac)
                return i;
            
        }
            

        
         return 0;
    }
               
                   


            

           
        
    
    public int getHighestRewardAction(IAStates state)
    {
        int highest_value = 0;
        int action_index = 0;
        for (int i = 0; i < 4; i++) // estados / columns
        {
            for (int j = 0; j < 80; j++) // ações / rows
            {
                if(i == (int) state)
                {
                    //caso essa ação seja a maior, escolha ela
                    if(r_Matrix.rows[j].row[i] > highest_value)
                    {
                        highest_value = r_Matrix.rows[j].row[i];
                        action_index = j;
                    }
                }
                    
                
            }
        }
        return action_index;
    }

    
    public int getZerosInRMatrix()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                if (r_Matrix.rows[j].row[i] == 0)
                    count++;
                    
                
            }
        }


        //retorna a porcentagem de 0
        return count;
    }

    public int getZeroPercentageInRMatrix()
    {
        int per = 0;
        int zero_count = 0;
        int total_count = 0;
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 80; j++)
            {
                if (r_Matrix.rows[j].row[i] == 0)
                    zero_count++;

                total_count++;

            }
        }

        // total_count == 100%
        // zero_count == x%
        // x total_count = 100 * zero count

        per = (int)((100 * zero_count) / total_count);
        return per;
    }
}