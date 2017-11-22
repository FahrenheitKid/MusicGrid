using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAModule : MonoBehaviour {

    public GridMakerScript grid_ref;
    public PlayerScript p1_script;
    public PlayerScript p2_script;

    public float danger_trigger;

    public float total_time_in_danger;

    public float total_match_time;

    public int p1_blockID;
    public int p2_blockID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
