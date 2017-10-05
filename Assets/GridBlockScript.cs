using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GridBlockScript : MonoBehaviour {

    public Color currentColor;

    public Renderer rend;
    public int level;
    public bool isMoving;

    public List<Color> colors;
	// Use this for initialization
	void Start () {
        rend.material.color = currentColor;
        updateColor();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateColor()
    {
        if(level >= 0 && level < colors.Capacity)
        currentColor = colors[level];

        Color emission = currentColor;
        rend.material.color = currentColor;
        emission *= Mathf.LinearToGammaSpace(0.5f);
        rend.material.SetColor("_EmissionColor", emission);
    }
}
