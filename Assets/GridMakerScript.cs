using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMakerScript : MonoBehaviour
{
    public GameObject block1;
    public GameObject powerUpPrefab;

    public bool populate;
    public int worldWidth = 10;
    public int worldHeight = 10;

    public float spawnSpeed = 0;

    public float gapsize_x = 0;
    public float gapsize_z = 0;
    public float blocks_range_z = 1.5f;
    public float blocks_range_x = 1.5f;

    public float block_movement_duration;

    public int lowest_level;
    public int highest_level;
    public int level_gap_limit;
    public int level_gap;
    public List<GameObject> grid_List;
    public int lastBlockHitP1;
    public int lastBlockHitP2;
    public PlayerScript p1_ref;
    public PlayerScript p2_ref;

    public List<GameObject> powerUps = new List<GameObject>();
    private float powerUpTimer = 0.0f;

    private void Start()
    {
        blocks_range_z = 1.5f;
        blocks_range_x = 1.5f;

        if (populate)
            StartCoroutine(CreateWorld());

        // 1.5 distance to sides
        // 2.12132 to diagonals

        /*
        float dist = Vector3.Distance(grid_List[0].transform.position, grid_List[6].transform.position);
        print("Distance to other: " + dist)
        */
    }

    public void moveRandomGridBlocks(int n_blocks, bool up) // função que move os cubos random no grid
    {
        int direction = 1;

        if (!up) direction = -1;
        if (n_blocks > grid_List.Count)
        {
            print("trying to move more blocks than exists!");
            n_blocks = grid_List.Count;
        }
        updateLevelGap();

        List<int> selecteds = new List<int>();

        List<int> checklist = new List<int>();

        if (selecteds.Count < n_blocks)
        {
            int difference = n_blocks - selecteds.Count;
            //escolhe cubos random para serem movidos
            for (int i = 0; i < difference; i++)
            {
                int sel = repeatlessRand(0, grid_List.Count - 1, checklist);
                selecteds.Add(sel);
                checklist.Add(sel);
            }
        }

        //move os cubos selecteds
        for (int i = 0; i < selecteds.Count; i++)
        {
            if (i > n_blocks) break;
            //melhor fazer Tween ou lerp com isso
            Vector3 newpos = grid_List[selecteds[i]].transform.position;
            newpos.y += 1 * direction;
            Transform trans = grid_List[selecteds[i]].transform;

            trans.DOMoveY(newpos.y, block_movement_duration).SetEase(Ease.InOutQuad);

            GridBlockScript blockscript = grid_List[selecteds[i]].GetComponent<GridBlockScript>();
            if (!up && blockscript.level == 0)
                blockscript.level = 0;
            else
                blockscript.level += direction;

            blockscript.updateColor();
        }

        updateLevelGap();
    }

    // função que move os cubos de acordo com varios parametros
    // n_blocks = quantidade dos blocos para mover em %. 100% = mover o maximo de blocos possiveis na área de atuação, 0% = nenhum bloco.
    // up = true para mover cubos para cima e false para baixo.
    // centerBlockID = o ID do bloco central de onde iniciará o raio da area de efeito dessa função ("epicentro")
    // distance = quantas vezes a distancia de um vizinho deve ser multiplicada. 1 significa os vizinhos imediatamente próximos, 2 a 2 de distancia, etc
    // isFilledArea = true para atuar em toda a area e false para apenas as extremidades (ainda precisa de uns ajustes)
    // neighborsHeightLimit = true se deseja limitar que os blocos imediatamente vizinhos tenham um limit de altura em relação ao bloco central
    // heighLimit = limite da altura dos imediatamente vizinhos

    public void moveRangedAreaFrom(int n_blocks_percent, bool up, int centerBlockID, int distance, bool isFilledArea, bool neighborsHeightLimit, int heightLimit)
    {
        int n_blocks = 0;

        if (n_blocks_percent == 100)
        {
            n_blocks = -1;
        }
        else
        {
            int max_blocks = 0;

            int count = 0;
            //check the maximum blocks possible given the function parameters
            {
                for (int i = 0; i < grid_List.Count; i++)
                {
                    if (i == centerBlockID) continue;

                    Vector3 difference = grid_List[i].transform.position - grid_List[centerBlockID].transform.position;

                    float distanceInX = Mathf.Abs(difference.x);
                    float distanceInY = Mathf.Abs(difference.y);
                    float distanceInZ = Mathf.Abs(difference.z);

                    GridBlockScript blockscript = grid_List[i].GetComponent<GridBlockScript>();

                    //se tem limite
                    if (neighborsHeightLimit)
                    {
                        //se são imediatamente vizinhos
                        if (distanceInX <= 1 * blocks_range_x && distanceInZ <= 1 * blocks_range_z)
                        {
                            int dif = 0;
                            dif = blockscript.level - grid_List[centerBlockID].GetComponent<GridBlockScript>().level;
                            dif = Mathf.Abs(dif);
                            //se vai passar do height limit, ignora esse step para nao selecionar esse bloco
                            if (dif >= heightLimit) continue;
                        }
                    }

                    if (isFilledArea)
                    {
                        //se dentro da distancia adiciona
                        if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                        {
                            count++;
                            // checklist.Add(i);
                        }
                    }
                    else
                    {
                        //se só o "contorno" da distancia, mas está ignorando a linha e coluna do jogador por algum motivo
                        if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                        {
                            float newdisX = (distance - 1) * blocks_range_x;
                            float newdisZ = (distance - 1) * blocks_range_z;
                            if (distanceInX >= newdisX && distanceInZ >= newdisZ)
                            {
                                count++;
                            }

                            // checklist.Add(i);
                        }
                    }
                }
            }

            max_blocks = count;

            // regra de trÊs

            // 100 --- max_blocks
            // n_blocks_percent --- x
            // 100x = max_blocks * n_blocks_percent

            n_blocks = (max_blocks * n_blocks_percent) / 100;
        }

        print(n_blocks_percent + "% = " + n_blocks + " blocos");

        int direction = 1;

        if (!up) direction = -1;
        if (n_blocks > grid_List.Count)
        {
            print("trying to move more blocks than exists!");
            n_blocks = grid_List.Count;
        }
        updateLevelGap();

        List<int> selecteds = new List<int>();

        List<int> semiSelecteds = new List<int>();

        List<int> checklist = new List<int>();

        bool no_limits = false;

        if (n_blocks == -1) no_limits = true;

        if (no_limits) // caso sem limites de cubos para mover
        {
            //int difference = n_blocks - selecteds.Count;
            //escolhe cubos random para serem movidos
            for (int i = 0; i < grid_List.Count; i++)
            {
                if (i == centerBlockID) continue;

                Vector3 difference = grid_List[i].transform.position - grid_List[centerBlockID].transform.position;

                float distanceInX = Mathf.Abs(difference.x);
                float distanceInY = Mathf.Abs(difference.y);
                float distanceInZ = Mathf.Abs(difference.z);

                GridBlockScript blockscript = grid_List[i].GetComponent<GridBlockScript>();

                //se tem limite
                if (neighborsHeightLimit)
                {
                    //se são imediatamente vizinhos
                    if (distanceInX <= 1 * blocks_range_x && distanceInZ <= 1 * blocks_range_z)
                    {
                        int dif = 0;
                        dif = blockscript.level - grid_List[centerBlockID].GetComponent<GridBlockScript>().level;
                        dif = Mathf.Abs(dif);
                        //se vai passar do height limit, ignora esse step para nao selecionar esse bloco
                        if (dif >= heightLimit) continue;
                    }
                }

                if (isFilledArea)
                {
                    //se dentro da distancia adiciona
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        selecteds.Add(i);
                        // checklist.Add(i);
                    }
                }
                else
                {
                    //se só o "contorno" da distancia, mas está ignorando a linha e coluna do jogador por algum motivo
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        float newdisX = (distance - 1) * blocks_range_x;
                        float newdisZ = (distance - 1) * blocks_range_z;
                        if (distanceInX >= newdisX && distanceInZ >= newdisZ)
                        {
                            selecteds.Add(i);
                        }

                        // checklist.Add(i);
                    }
                }
            }
        }
        else
        {
            //int difference = n_blocks - selecteds.Count;
            //escolhe cubos random para serem movidos
            for (int i = 0; i < grid_List.Count; i++)
            {
                if (i == centerBlockID) continue;

                Vector3 difference = grid_List[i].transform.position - grid_List[centerBlockID].transform.position;

                float distanceInX = Mathf.Abs(difference.x);
                float distanceInY = Mathf.Abs(difference.y);
                float distanceInZ = Mathf.Abs(difference.z);

                GridBlockScript blockscript = grid_List[i].GetComponent<GridBlockScript>();

                //se tem limite
                if (neighborsHeightLimit)
                {
                    //se são imediatamente vizinhos
                    if (distanceInX <= 1 * blocks_range_x && distanceInZ <= 1 * blocks_range_z)
                    {
                        int dif = 0;
                        dif = blockscript.level - grid_List[centerBlockID].GetComponent<GridBlockScript>().level;
                        dif = Mathf.Abs(dif);

                        //se vai passar do height limit, ignora esse step para nao selecionar esse bloco
                        if (dif >= heightLimit) continue;
                    }
                }

                if (isFilledArea)
                {
                    //se dentro da distancia adiciona
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        semiSelecteds.Add(i);
                        //checklist.Add(i);
                    }
                }
                else
                {
                    //se só o "contorno" da distancia, mas está ignorando a linha e coluna do jogador por algum motivo
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        float newdisX = (distance - 1) * blocks_range_x;
                        float newdisZ = (distance - 1) * blocks_range_z;
                        if (distanceInX >= newdisX && distanceInZ >= newdisZ)
                        {
                            semiSelecteds.Add(i);
                            //checklist.Add(i);
                        }
                    }
                }
            }

            // escolhe uma quantidade random dos semiSelecteds

            for (int i = 0; i < n_blocks; i++)
            {
                int index = 0;
                if (semiSelecteds.Count < n_blocks)
                {
                    for (int j = 0; j < semiSelecteds.Count; j++)
                    {
                        selecteds.Add(semiSelecteds[j]);
                    }
                    break;
                }
                else
                {
                    index = repeatlessRand(0, semiSelecteds.Count, checklist);
                    checklist.Add(index);
                    selecteds.Add(semiSelecteds[index]);
                }
            }
        }

        print("moveu tantos " + selecteds.Count);
        //move os cubos selecteds
        for (int i = 0; i < selecteds.Count; i++)
        {
            GridBlockScript blockscript = grid_List[selecteds[i]].GetComponent<GridBlockScript>();

            if (i > n_blocks && n_blocks > 0) break;
            //melhor fazer Tween ou lerp com isso
            Vector3 newpos = grid_List[selecteds[i]].transform.position;
            newpos.y += 1 * direction;
            Transform trans = grid_List[selecteds[i]].transform;

            trans.DOMoveY(newpos.y, block_movement_duration).SetEase(Ease.InOutQuad);

            if (!up && blockscript.level == 0)
                blockscript.level = 0;
            else
                blockscript.level += direction;

            blockscript.updateColor();
        }

        updateLevelGap();
    }

    //versão da função só podendo alterar os parametros que a IA pode escolher, o resto fica o valor default
    public void moveRangedAreaFrom(int n_blocks_percent, int distance, bool isFilledArea)
    {
        bool up = true;
        int centerBlockID;
        // seleciona o player mais proximo de morrer
        centerBlockID = (p1_ref.deathTimer >= p2_ref.deathTimer) ? lastBlockHitP1 : lastBlockHitP2;

        //evita que movam blocos mt alto proximo do player
        bool neighborsHeightLimit = true;
        int heightLimit = 2;

        int n_blocks = 0;

        //check the maximum blocks possible given the function parameters
        {
            int max_blocks = 0;

            int count = 0;

            {
                for (int i = 0; i < grid_List.Count; i++)
                {
                    if (i == centerBlockID) continue;

                    Vector3 difference = grid_List[i].transform.position - grid_List[centerBlockID].transform.position;

                    float distanceInX = Mathf.Abs(difference.x);
                    float distanceInY = Mathf.Abs(difference.y);
                    float distanceInZ = Mathf.Abs(difference.z);

                    GridBlockScript blockscript = grid_List[i].GetComponent<GridBlockScript>();

                    //se tem limite
                    if (neighborsHeightLimit)
                    {
                        //se são imediatamente vizinhos
                        if (distanceInX <= 1 * blocks_range_x && distanceInZ <= 1 * blocks_range_z)
                        {
                            int dif = 0;
                            dif = blockscript.level - grid_List[centerBlockID].GetComponent<GridBlockScript>().level;
                            dif = Mathf.Abs(dif);
                            //se vai passar do height limit, ignora esse step para nao selecionar esse bloco
                            if (dif >= heightLimit) continue;
                        }
                    }

                    if (isFilledArea)
                    {
                        //se dentro da distancia adiciona
                        if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                        {
                            count++;
                            // checklist.Add(i);
                        }
                    }
                    else
                    {
                        //se só o "contorno" da distancia, mas está ignorando a linha e coluna do jogador por algum motivo
                        if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                        {
                            float newdisX = (distance - 1) * blocks_range_x;
                            float newdisZ = (distance - 1) * blocks_range_z;
                            if (distanceInX >= newdisX && distanceInZ >= newdisZ)
                            {
                                count++;
                            }

                            // checklist.Add(i);
                        }
                    }
                }
            }

            max_blocks = count;

            // regra de trÊs

            // 100 --- max_blocks
            // n_blocks_percent --- x
            // 100x = max_blocks * n_blocks_percent

            n_blocks = (max_blocks * n_blocks_percent) / 100;
        }

        //  print(n_blocks_percent + "% = " + n_blocks + " blocos");

        int direction = 1;

        if (!up) direction = -1;
        if (n_blocks > grid_List.Count)
        {
            //  print("trying to move more blocks than exists!");
            n_blocks = grid_List.Count;
        }
        updateLevelGap();

        List<int> selecteds = new List<int>();

        List<int> semiSelecteds = new List<int>();

        List<int> checklist = new List<int>();

        bool no_limits = false;

        if (n_blocks_percent == 100) no_limits = true;

        if (no_limits) // caso sem limites de cubos para mover
        {
            //int difference = n_blocks - selecteds.Count;
            //escolhe cubos random para serem movidos
            for (int i = 0; i < grid_List.Count; i++)
            {
                if (i == centerBlockID) continue;

                Vector3 difference = grid_List[i].transform.position - grid_List[centerBlockID].transform.position;

                float distanceInX = Mathf.Abs(difference.x);
                float distanceInY = Mathf.Abs(difference.y);
                float distanceInZ = Mathf.Abs(difference.z);

                GridBlockScript blockscript = grid_List[i].GetComponent<GridBlockScript>();

                //se tem limite
                if (neighborsHeightLimit)
                {
                    //se são imediatamente vizinhos
                    if (distanceInX <= 1 * blocks_range_x && distanceInZ <= 1 * blocks_range_z)
                    {
                        int dif = 0;
                        dif = blockscript.level - grid_List[centerBlockID].GetComponent<GridBlockScript>().level;
                        dif = Mathf.Abs(dif);
                        //se vai passar do height limit, ignora esse step para nao selecionar esse bloco
                        if (dif >= heightLimit) continue;
                    }
                }

                if (isFilledArea)
                {
                    //se dentro da distancia adiciona
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        selecteds.Add(i);
                        // checklist.Add(i);
                    }
                }
                else
                {
                    //se só o "contorno" da distancia, mas está ignorando a linha e coluna do jogador por algum motivo
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        float newdisX = (distance - 1) * blocks_range_x;
                        float newdisZ = (distance - 1) * blocks_range_z;
                        if (distanceInX >= newdisX && distanceInZ >= newdisZ)
                        {
                            selecteds.Add(i);
                        }

                        // checklist.Add(i);
                    }
                }
            }
        }
        else
        {
            //int difference = n_blocks - selecteds.Count;
            //escolhe cubos random para serem movidos
            for (int i = 0; i < grid_List.Count; i++)
            {
                if (i == centerBlockID) continue;

                Vector3 difference = grid_List[i].transform.position - grid_List[centerBlockID].transform.position;

                float distanceInX = Mathf.Abs(difference.x);
                float distanceInY = Mathf.Abs(difference.y);
                float distanceInZ = Mathf.Abs(difference.z);

                GridBlockScript blockscript = grid_List[i].GetComponent<GridBlockScript>();

                //se tem limite
                if (neighborsHeightLimit)
                {
                    //se são imediatamente vizinhos
                    if (distanceInX <= 1 * blocks_range_x && distanceInZ <= 1 * blocks_range_z)
                    {
                        int dif = 0;
                        dif = blockscript.level - grid_List[centerBlockID].GetComponent<GridBlockScript>().level;
                        dif = Mathf.Abs(dif);

                        //se vai passar do height limit, ignora esse step para nao selecionar esse bloco
                        if (dif >= heightLimit) continue;
                    }
                }

                if (isFilledArea)
                {
                    //se dentro da distancia adiciona
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        semiSelecteds.Add(i);
                        //checklist.Add(i);
                    }
                }
                else
                {
                    //se só o "contorno" da distancia, mas está ignorando a linha e coluna do jogador por algum motivo
                    if (distanceInX <= distance * blocks_range_x && distanceInZ <= distance * blocks_range_z)
                    {
                        float newdisX = (distance - 1) * blocks_range_x;
                        float newdisZ = (distance - 1) * blocks_range_z;
                        if (distanceInX >= newdisX && distanceInZ >= newdisZ)
                        {
                            semiSelecteds.Add(i);
                            //checklist.Add(i);
                        }
                    }
                }
            }

            // escolhe uma quantidade random dos semiSelecteds

            for (int i = 0; i < n_blocks; i++)
            {
                int index = 0;
                if (semiSelecteds.Count < n_blocks)
                {
                    for (int j = 0; j < semiSelecteds.Count; j++)
                    {
                        selecteds.Add(semiSelecteds[j]);
                    }
                    break;
                }
                else
                {
                    index = repeatlessRand(0, semiSelecteds.Count, checklist);
                    checklist.Add(index);
                    selecteds.Add(semiSelecteds[index]);
                }
            }
        }

        //print("moveu tantos " + selecteds.Count);
        //move os cubos selecteds
        for (int i = 0; i < selecteds.Count; i++)
        {
            GridBlockScript blockscript = grid_List[selecteds[i]].GetComponent<GridBlockScript>();

            if (i > n_blocks && n_blocks > 0) break;
            //melhor fazer Tween ou lerp com isso
            Vector3 newpos = grid_List[selecteds[i]].transform.position;
            newpos.y += 1 * direction;
            Transform trans = grid_List[selecteds[i]].transform;

            trans.DOMoveY(newpos.y, block_movement_duration).SetEase(Ease.InOutQuad);

            if (!up && blockscript.level == 0)
                blockscript.level = 0;
            else
                blockscript.level += direction;

            blockscript.updateColor();
        }

        updateLevelGap();
    }

    public void moveLowestBlocks(bool up) // função que move os cubos mais baixos de acordo com o gap_level_limit
    {
        int direction = 1;

        if (!up) direction = -1;

        updateLevelGap();

        List<int> selecteds = new List<int>();

        List<int> checklist = new List<int>();

        bool needToSelect = false;
        if (level_gap >= level_gap_limit)
            needToSelect = true;

        if (needToSelect)
        {
            for (int i = 0; i < grid_List.Count; i++)
            {
                if (grid_List[i].GetComponent<GridBlockScript>().level <= lowest_level)
                {
                    selecteds.Add(i);
                    // checklist.Add(i);
                }
            }
        }

        //move os cubos selecteds
        for (int i = 0; i < selecteds.Count; i++)
        {
            //melhor fazer Tween ou lerp com isso
            Vector3 newpos = grid_List[selecteds[i]].transform.position;
            newpos.y += 1 * direction;
            Transform trans = grid_List[selecteds[i]].transform;

            trans.DOMoveY(newpos.y, block_movement_duration).SetEase(Ease.InOutQuad);

            GridBlockScript blockscript = grid_List[selecteds[i]].GetComponent<GridBlockScript>();
            if (!up && blockscript.level == 0)
                blockscript.level = 0;
            else
                blockscript.level += direction;

            blockscript.updateColor();
        }

        updateLevelGap();
    }

    public void updateLevelGap() // função que atualiza o valor do gap entre bloco mais alto e bloco mais baixo
    {
        for (int i = 0; i < grid_List.Count; i++)
        {
            GridBlockScript g = grid_List[i].GetComponent<GridBlockScript>();

            if (i == 0)
            {
                lowest_level = g.level;
                highest_level = g.level;
            }
            if (g.level <= lowest_level)
                lowest_level = g.level;

            if (g.level >= highest_level)
                highest_level = g.level;
        }

        level_gap = highest_level - lowest_level;
    }

    private IEnumerator CreateWorld() // função de criação de grid
    {
        for (int x = 0; x < worldWidth; x++)
        {
            yield return new WaitForSeconds(spawnSpeed);

            for (int z = 0; z < worldHeight; z++)
            {
                yield return new WaitForSeconds(spawnSpeed);

                GameObject block = Instantiate(block1, Vector3.zero, block1.transform.rotation) as GameObject;
                block.transform.parent = transform;
                block.transform.position = new Vector3(x + x * gapsize_x, 0, z + z * gapsize_z);
                grid_List.Add(block);
            }
        }
    }

    // Update is called once per frame

    private void Update()
    {
        if (powerUps.Count < 2 && powerUpTimer > 3f)
        {
            SpawnPowerUp();
            powerUpTimer = 0.0f;
        }
        powerUpTimer += Time.fixedDeltaTime;
    }

    public int repeatlessRand(int min, int max, List<int> checklist) // função utility para gerar random ignorando valores de uma lista de exceções
    {
        int rand = Random.Range(min, max);
        bool safetest = false;

        while (!safetest)
        {
            for (int i = 0; i < checklist.Count; i++)
            {
                if (rand == checklist[i])
                {
                    rand = Random.Range(min, max);
                    i = 0;
                }
            }

            safetest = true;
            for (int i = 0; i < checklist.Count; i++)
            {
                if (rand == checklist[i])
                {
                    safetest = false;
                }
            }
        }

        return rand;
    }

    //spawna os power ups na fase
    private void SpawnPowerUp()
    {
        int cubeToSpawnX = Random.Range(1, (int)Mathf.Sqrt(grid_List.Count));
        int cubeToSpawnZ = Random.Range(1, (int)Mathf.Sqrt(grid_List.Count));
        int index = cubeToSpawnX * cubeToSpawnZ - 1;

        int cubeLvl = grid_List[index].GetComponent<GridBlockScript>().level;
        Vector3 spawnPos = new Vector3(1.5f * (cubeToSpawnX - 1), 0.85f + (cubeLvl + 2) * 1f, 1.5f * cubeToSpawnZ);
        //Debug.Log(spawnPos);
        powerUps.Add(Instantiate(powerUpPrefab, spawnPos, Quaternion.identity));
    }
}