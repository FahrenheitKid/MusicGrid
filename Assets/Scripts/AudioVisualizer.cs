using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVisualizer : MonoBehaviour
{
    public Vector3 posStarter = new Vector3(-8.0f, 1.0f, 5.0f);
    public GameObject parent;
    public int cubeNumbers;
    GameObject[] cubes;
    Material[] cubesMat;
    public Color cubeColor;
    public GameObject prefab;
    public float distance;
    public float spectrumScale;
    public float startScale;
    float[] spectrum = new float[512];
    float[] freqBand = new float[8];
    float[] bandBuffer = new float[8];
    float[] bufferDecrease = new float[8];
    public FFTWindow fftWindow;
    int count = 0, sampleCount;
    float average = 0;
    public bool x = true, y = false, z = false;



    // Use this for initialization
    void Start()
    {
        cubes = new GameObject[cubeNumbers];
        cubesMat = new Material[cubeNumbers];
        //baseAudio = GameObject.Find("Plane").GetComponent<AudioSource>();
        for (int i = 0; i < cubeNumbers; i++)
        {
            Vector3 pos = posStarter;
            if (x)
                pos.x = pos.x + (distance * (i + 1));
            if (y)
                pos.y = pos.y + (distance * (i + 1));
            if (z)
                pos.z = pos.z + (distance * (i + 1));

            cubes[i] = Instantiate(prefab, pos, new Quaternion(), parent.transform);
            cubes[i].transform.Rotate(0, 35f, 0);
            cubesMat[i] = cubes[i].GetComponentInChildren<MeshRenderer>().material;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.GetSpectrumData(spectrum, 0, fftWindow);
        for (int i = 0; i < cubeNumbers; i++)
        {
            cubes[i].transform.localScale = new Vector3(cubes[i].transform.localScale.x, (bandBuffer[i] * spectrumScale + startScale), cubes[i].transform.localScale.z);
            Color color = cubeColor;
            color *= bandBuffer[i] + 0.5f;
            cubesMat[i].color = color;
            Color emission = color;
            emission *= Mathf.LinearToGammaSpace(0.1f);
            cubesMat[i].SetColor("_EmissionColor", emission);
        }

        count = 0;
        for (int i = 0; i < 8; i++)
        {
            sampleCount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                sampleCount += 2;
            }
            for (int j = 0; j < sampleCount; j++)
            {
                average += spectrum[count] * (count + 1);
                count++;
            }
            average /= count;
            freqBand[i] = average * 10;

            if (freqBand[i] > bandBuffer[i])
            {
                bandBuffer[i] = freqBand[i];
                bufferDecrease[i] = 0.005f;
            }
            else
            {
                bandBuffer[i] -= bufferDecrease[i];
                bufferDecrease[i] *= 1.2f;
            }

        }
    }
}
/*
 *  intensity = freqBand[i]* spectrumScale;
            lerpY = Mathf.Lerp(cubes[i].transform.localScale.x, intensity, Time.deltaTime);
            Vector3 newScale = new Vector3(cubes[i].transform.localScale.x, lerpY, cubes[i].transform.localScale.z);
 */
