using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform Ypivot;
    public Transform Xpivot;
    public float maxAngle;
    public float minAngle;
    public float sens;

    // Update is called once per frame
    void Update()
    {
        Vector2 movement = new Vector2
        {
            x = Input.GetAxisRaw("Mouse X"),
            y = Input.GetAxisRaw("Mouse Y")
        };
        Xpivot.transform.Rotate(Vector3.right, -movement.y * sens);
        Ypivot.transform.Rotate(Vector3.up, movement.x * sens);


    }
}
