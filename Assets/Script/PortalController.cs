using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public bool isInside;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform.position.x * Vector3.right + this.transform.position.y * Vector3.up + cam.transform.position.z * Vector3.forward, Vector3.up);
        if (isInside)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GameManager.instance.NextLevel();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isInside = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isInside = false;
    }

}
