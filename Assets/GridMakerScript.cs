using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMakerScript : MonoBehaviour {

    public GameObject block1;

    public int worldWidth = 10;
    public int worldHeight = 10;

    public float spawnSpeed = 0;

    public float gapsize_x = 0;
    public float gapsize_z = 0;

    public List<GameObject> grid_List;
    void Start()
    {
        StartCoroutine(CreateWorld());
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
		
	}
}
