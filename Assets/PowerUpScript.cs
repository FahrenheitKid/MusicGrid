using UnityEngine;

public class PowerUpScript : MonoBehaviour
{
    public bool isSlowPower = false;
    public bool isMusicPower = false;
    public bool isSingleJumpPower = false;

    private void Start()
    {
        int luck = Random.Range(0, 3);

        switch (luck)
        {
            case 0:
                isSlowPower = true;
                GetComponent<MeshRenderer>().material.color = Color.blue;
                break;

            case 1:
                isMusicPower = true;
                GetComponent<MeshRenderer>().material.color = Color.yellow;
                break;

            case 2:
                isSingleJumpPower = true;
                GetComponent<MeshRenderer>().material.color = Color.green;
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("EndWorld"))
        {
            GridMakerScript gs = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridMakerScript>();

            gs.powerUps.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}