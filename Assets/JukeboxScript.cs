using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class JukeboxScript : MonoBehaviour {


    public float offsetx;
    public float offsety;
    public float offsetz;
    public GameObject grid;
    public int blocks_to_move;
    public bool isMusicPower;

    public float musicPower_time;
    public float musicPower_timer;

    public float initial_gThresh;
    public float pitch_modifier;
    public float gThresh_modifier;


    void Start()
    {
        //Select the instance of AudioProcessor and pass a reference
        //to this object
        AudioProcessor processor = FindObjectOfType<AudioProcessor>();
        processor.onBeat.AddListener(onOnbeatDetected);
        processor.onSpectrum.AddListener(onSpectrum);
    }


    private void Update()
    {
        if (isMusicPower)
        {
            //speed = (float)speed * 0.5f;

            musicPower_time -= Time.deltaTime;
            if (musicPower_time <= 0)
            {
                isMusicPower = false;
                DOTween.To(() => GetComponent<AudioSource>().pitch, x => GetComponent<AudioSource>().pitch = x, 1.0f, 1);
                DOTween.To(() => GetComponent<AudioProcessor>().gThresh, x => GetComponent<AudioProcessor>().gThresh = x, initial_gThresh, 2);

            }

        }
    }
    //this event will be called every time a beat is detected.
    //Change the threshold parameter in the inspector
    //to adjust the sensitivity
    void onOnbeatDetected()
    {
        GridMakerScript gm = grid.GetComponent<GridMakerScript>();
        gm.moveLowestBlocks(true);
        //gm.moveRandomGridBlocks(blocks_to_move, true);

        //ESCOLHA DA IA AQUI
        gm.moveRangedAreaFrom(5, true, gm.lastBlockHitP1, 2, true,true,2);
       // Debug.Log("Beat!!!");
    }

    //This event will be called every frame while music is playing
    void onSpectrum(float[] spectrum)
    {
        //The spectrum is logarithmically averaged
        //to 12 bands

        for (int i = 0; i < spectrum.Length; ++i)
        {
            Vector3 start = new Vector3(i + offsetx, 0 + offsety, 0 + offsetz);
            Vector3 end = new Vector3(i + offsetx, spectrum[i] + offsety, 0 + offsetz);
            Debug.DrawLine(start, end);
        }
    }
}
