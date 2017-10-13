using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class GridBlockScript : MonoBehaviour {

    public Color currentColor;
    public int currentColor_index;
    public Renderer rend;
    public int level;
    public bool isMoving;

    public List<Color> colors;
	// Use this for initialization
	void Start () {
        rend = GetComponent<MeshRenderer>();
        currentColor = colors[0];
        currentColor_index = 0;
        rend.material.color = currentColor;
        updateColor();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void updateColor()
    {
        if (currentColor_index < colors.Count - 2)
            currentColor_index++;
        else
            currentColor_index = 0;

        currentColor = colors[currentColor_index];

        Color emission = currentColor;
        rend.material.color = currentColor;
        emission *= Mathf.LinearToGammaSpace(0.5f);
        rend.material.SetColor("_EmissionColor", emission);
    }
}
