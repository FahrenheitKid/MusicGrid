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

    public float block_movement_duration;

    public int lowest_level;
    public int highest_level;
    public int level_gap_limit;
    public int level_gap;
    public List<GameObject> grid_List;

    List<GameObject> powerUps = new List<GameObject>();
    float powerUpTimer = 0.0f;

    void Start()
    {
        if (populate)
            StartCoroutine(CreateWorld());
    }


    void onOnbeatDetected()
    {
        // Color lastcolor = color;
        //color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        //rend.material.color = color;
        moveGridBlocks(4, true);
        Debug.Log("Beat grid!!!");

    }

    public void moveGridBlocks(int n_blocks, bool up) // função que move os cubos
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

        if (selecteds.Count < n_blocks)
        {
            int difference = n_blocks - selecteds.Count;
            //escolhe cubos random para serem movidos
            for (int i = 0; i < difference; i++)
            {

                selecteds.Add(repeatlessRand(0, grid_List.Count - 1, checklist));
                checklist.Add(selecteds[i]);

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
        Vector3 spawnPos = new Vector3(1.5f * (cubeToSpawnX - 1), 0.85f + cubeLvl * 1f, 1.5f * cubeToSpawnZ);
        powerUps.Add(Instantiate(powerUpPrefab, spawnPos, Quaternion.identity));
    }

}
