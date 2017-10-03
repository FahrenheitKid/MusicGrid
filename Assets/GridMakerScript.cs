using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class GridMakerScript : MonoBehaviour {

    public GameObject block1;

    public bool populate;
    public int worldWidth = 10;
    public int worldHeight = 10;

    public float spawnSpeed = 0;

    public float gapsize_x = 0;
    public float gapsize_z = 0;

    BeatObserver beatObserver;

    public List<GameObject> grid_List;
    void Start()
    {
        if(populate)
        StartCoroutine(CreateWorld());

        beatObserver = GetComponent<BeatObserver>();
    }


    void onOnbeatDetected()
    {
        // Color lastcolor = color;
        //color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        //rend.material.color = color;
        int selected = Random.Range(0, grid_List.Capacity - 1);
        Vector3 newpos = grid_List[selected].transform.position;
        newpos.y += 1;
        grid_List[selected].transform.position = newpos;
        Debug.Log("Beat grid!!!");

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
}
