using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float speed = 5;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.transform.Rotate(Vector3.up * Time.deltaTime * speed);
    }
}