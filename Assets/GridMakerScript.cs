using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;
using DG.Tweening;

public class GridMakerScript : MonoBehaviour {

    public GameObject block1;

    public bool populate;
    public int worldWidth = 10;
    public int worldHeight = 10;

    public float spawnSpeed = 0;

    public float gapsize_x = 0;
    public float gapsize_z = 0;

    public float block_movement_duration;
    BeatObserver beatObserver;

    public List<GameObject> grid_List;
    void Start()
    {

       
        if (populate)
        StartCoroutine(CreateWorld());

        beatObserver = GetComponent<BeatObserver>();
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
        if(n_blocks > grid_List.Capacity)
        {
            print("trying to move more blocks than exists!");
            return;

        }

        List<int> selecteds = new List<int>();

        List<int> checklist = new List<int>();
        for (int i = 0; i < n_blocks; i++)
        {
            selecteds.Add(repeatlessRand(0, grid_List.Capacity - 1,checklist));
            checklist.Add(selecteds[i]);
           
        }

        for(int i = 0; i < selecteds.Capacity; i++)
        {
            //melhor fazer Tween ou lerp com isso
            Vector3 newpos = grid_List[selecteds[i]].transform.position;
            newpos.y += 1 * direction;
            Transform trans = grid_List[selecteds[i]].transform;

            trans.DOMoveY(newpos.y, block_movement_duration).SetEase(Ease.InOutQuad);

            GridBlockScript blockscript = grid_List[selecteds[i]].GetComponent<GridBlockScript>();
            if (!up && blockscript.level == 0)
                blockscript.level = 10;
            else if (up && blockscript.level == 10)
                blockscript.level = 0;
            else
                blockscript.level += direction;

            blockscript.updateColor();

        }
       
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
    void Update () {
        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            onOnbeatDetected();
        }
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

}
