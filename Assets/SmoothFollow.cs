// Smooth Follow from Standard Assets
// Converted to C# because I fucking hate UnityScript and it's inexistant C# interoperability
// If you have C# code and you want to edit SmoothFollow's vars ingame, use this instead.
using UnityEngine;
using System.Collections;

public class SmoothFollow : MonoBehaviour
{

    // The target we are following
    public Transform target;
    // The distance in the x-z plane to the target
    public float distance = 10.0f;
    // the height we want the camera to be above the target
    public float height = 5.0f;
    // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;
    public float offset;

    public GameObject p1;
    public GameObject p2;

    GameObject g;

    public bool PlayerMediumPointAsTarget;
    // Place the script in the Camera-Control group in the component menu
    [AddComponentMenu("Camera-Control/Smooth Follow")]
    // Place the script in the Camera-Control group in the component menu
    private void Start()
    {
        g = new GameObject();
    }

    void LateUpdate()
    {
        g.transform.position = GetPoint(offset);

        if (PlayerMediumPointAsTarget)
            target = g.transform;
        // Early out if we don't have a target
        if (!target) return;


        
        // Calculate the current rotation angles
        float wantedRotationAngle = target.eulerAngles.y;
        float wantedHeight = target.position.y + height;

        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        var currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        // Always look at the target
        transform.LookAt(target);
    }


    public Vector3 GetPoint(float offset)
    {
        //get the positions of our transforms
        Vector3 pos1 = p1.transform.position;
        Vector3 pos2 = p2.transform.position;

        //get the direction between the two transforms -->
        Vector3 dir = (pos2 - pos1).normalized;

        //get a direction that crosses our [dir] direction
        //NOTE! : this can be any of a buhgillion directions that cross our [dir] in 3D space
        //To alter which direction we're crossing in, assign another directional value to the 2nd parameter
        Vector3 perpDir = Vector3.Cross(dir, Vector3.right);

        //get our midway point
        Vector3 midPoint = (pos1 + pos2) / 2f;

        //get the offset point
        //This is the point you're looking for.
        Vector3 offsetPoint = midPoint + (perpDir * offset);

        return offsetPoint;
    }
}