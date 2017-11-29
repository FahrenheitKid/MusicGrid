using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;
using DG.Tweening;

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
    public float blocks_range_z;
    public float blocks_range_x;

    public float block_movement_duration;

    public int lowest_level;
    public int highest_level;
    public int level_gap_limit;
    public int level_gap;
    public List<GameObject> grid_List;
    public int lastBlockHitP1;
    public int lastBlockHitP2;

    public List<GameObject> powerUps = new List<GameObject>();
    float powerUpTimer = 0.0f;

    void Start()
    {
        if (populate)
            StartCoroutine(CreateWorld());

        // 1.5 distance to sides
        // 2.12132 to diagonals

        /*
        float dist = Vector3.Distance(grid_List[0].transform.position, grid_List[6].transform.position);
        print("Distance to other: " + dist)
        */

    }


    void onOnbeatDetected()
    {
        // Color lastcolor = color;
        //color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        //rend.material.color = color;
        //moveGridBlocks(4, true);
        Debug.Log("Beat grid!!!");

    }

    public void moveRandomGridBlocks(int n_blocks, bool up) // função que move os cubos
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


    public void moveRangedAreaFrom(int n_blocks, bool up, int centerBlockID, int distance, bool isFilledArea) // função que move os cubos
    {
        int direction = 1;

        if (!up) direction = -1;
        if (n_blocks > grid_List.Count)
        {
            print("trying to move more blocks than exists!");
            return;

        }
        updateLevelGap();

        List<int> selecteds = new List<int>();

        List <int>semiSelecteds = new List<int>();

        List<int> checklist = new List<int>();

        bool no_limits = false;

        if (n_blocks == 0) no_limits = true;

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

                if (isFilledArea)
                {
                    //se dentro da distancia adiciona
                    if (distanceInX <=   distance * 1.5f && distanceInZ <= distance * 1.5f)
                    {
                        selecteds.Add(i);
                       // checklist.Add(i);
                    }
                }
                else
                {
                    //se só o "contorno" da distancia, mas está ignorando a linha e coluna do jogador por algum motivo
                    if(distanceInX <= distance * 1.5f && distanceInZ <= distance * 1.5f)
                    {

                        float newdis = (distance - 1) * 1.5f;
                        if (distanceInX >=  newdis && distanceInZ >= newdis)
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

            for(int i = 0; i < n_blocks; i++)
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
            
            if (i > n_blocks  && n_blocks > 0) break;
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

    public void moveLowestBlocks(bool up) // função que move os cubos
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


    public void updateLevelGap()
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
    IEnumerator CreateWorld()
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
        if (powerUps.Count < 2 && powerUpTimer > 2f)
        {
            SpawnPowerUp();
            powerUpTimer = 0.0f;
        }
        powerUpTimer += Time.fixedDeltaTime;
    }



    public int repeatlessRand(int min, int max, List<int> checklist)
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

    void SpawnPowerUp()
    {
        int cubeToSpawnX = Random.Range(1, (int)Mathf.Sqrt(grid_List.Count));
        int cubeToSpawnZ = Random.Range(1, (int)Mathf.Sqrt(grid_List.Count));
        int index = cubeToSpawnX * cubeToSpawnZ - 1;

        int cubeLvl = grid_List[index].GetComponent<GridBlockScript>().level;
        Vector3 spawnPos = new Vector3(1.5f * (cubeToSpawnX - 1), 0.85f + (cubeLvl + 1) * 1f, 1.5f * cubeToSpawnZ);
        Debug.Log(spawnPos);
        powerUps.Add(Instantiate(powerUpPrefab, spawnPos, Quaternion.identity));
    }

}
