using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : MonoBehaviour {


    public bool isSlowPower = false;
    public bool isMusicPower = false;
    public bool isSingleJumpPower = false;
	// Use this for initialization
	void Start () {

        int luck = Random.Range(0, 2);
        //luck = 0;
        switch(luck)
        {
            case 0: isSlowPower = true;
                break;
            case 1: isMusicPower = true;

                break;
            case 2: isSingleJumpPower = true;
                break;
            default:
                break;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
