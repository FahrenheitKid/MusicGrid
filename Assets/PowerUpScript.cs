using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : MonoBehaviour {

    public bool isSlowPower = false;
    public bool isMusicPower = false;
    public bool isSingleJumpPower = false;
	
	void Start () {

        int luck = Random.Range(0, 2);
        //luck = 1;
        switch (luck)
        {
            case 0:
                isSlowPower = true;
                GetComponent<MeshRenderer>().material.color = Color.blue;
                break;
            case 1:
                isMusicPower = true;
                GetComponent<MeshRenderer>().material.color = Color.green;
                break;
            case 2:
                isSingleJumpPower = true;
                GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;
        }
    }
	
}
